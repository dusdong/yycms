using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web;
using yycms.entity;
using yycms.service.PlugIn;

namespace yycms.service.Queues
{
    /// <summary>
    ///页面静态生成服务 
    ///1，执行任务，更新状态为1
    ///2，执行完成，更新状态为2
    ///3，发生错误，更新状态为3
    /// </summary>
    public class PageBuild : IQueue
    {
        public string Path
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable
        {
            get { return true; }
        }

        public void ReceiveCompleted(String body)
        {
            var Tasks = DB.yy_Page_Build_Task.Where(a => a.Status == 0).ToList();

            if (Tasks == null || Tasks.Count < 1) { return; }

            var Builder = new PageBuilder();

            StringBuilder sb = new StringBuilder();

            foreach (var v in Tasks)
            {
                sb.AppendLine(v.FullName);

                try
                {
                    //状态变更
                    ChangeStatus(v.ID, 1);

                    //获取模板ID集合
                    Int64[] IDs = JsonConvert.DeserializeObject<Int64[]>(v.BuildEntity);

                    if (v.BuildCount > 0 && v.BuildCount >= IDs.Length) { v.BuildCount = IDs.Length - 1; }

                    for (Int64 vv = v.BuildCount; vv < IDs.Length; vv++)
                    {
                        try
                        {
                            Builder.Build(v.PageID, IDs[vv], (_PageIndex, _PageCount) =>
                            {
                                ChangeProcess(v.ID, 1, _PageCount);

                                return true;
                            });

                            ChangeStatus(v.ID, 2);

                            sb.AppendLine("OK");
                        }

                        catch (Exception _ex)
                        {
                            String errorInfo = JsonConvert.SerializeObject(new
                                {
                                    PageId = v.PageID,
                                    DBID = IDs[vv],
                                    Msg = _ex.Message,
                                    Source = _ex.Source,
                                    StackTrace = _ex.StackTrace,
                                    Time = DateTime.Now.ToString()
                                });
                            sb.AppendLine(errorInfo);
                            AddLog(v.ID,errorInfo+ ",");
                            ChangeStatus(v.ID, 3);
                            ChangeProcess(v.ID, 1);
                        }

                        Thread.Sleep(1);
                    }
                    
                    Signal_Excute("GlobalHub", x => 
                    {
                        x.Invoke("notify", 0, "页面["+v.FullName + "]生成完成。");
                    });
                }

                catch (Exception ex)
                {
                    String errorInfo = JsonConvert.SerializeObject(new
                    {
                        PageId = v.PageID,
                        DBID = 0,
                        Msg = ex.Message,
                        Source = ex.Source,
                        StackTrace = ex.StackTrace,
                        Time = DateTime.Now.ToString()
                    });

                    sb.AppendLine(errorInfo);

                    AddLog(v.ID, errorInfo + ",");

                    ChangeStatus(v.ID, 3);

                    ChangeProcess(v.ID, 1);
                }
            }

            SendMail("站点生成完成！", sb.ToString());
        }

        #region 数据库操作对象
        DBConnection DB = new DBConnection();
        #endregion

        #region 辅助方法
        public Boolean ChangeStatus(Int64 _ID, Int32 _Status)
       {
           String Insert_Cmd = "UPDATE [yy_Page_Build_Task] SET Status=@Status,LastUpdateTime=@LastUpdateTime WHERE ID=@ID";

           Boolean result = SqlHelper.ExecuteNonQuery(DB.Database.Connection.ConnectionString, CommandType.Text, Insert_Cmd,
               new SqlParameter("@ID", _ID),
               new SqlParameter("@Status", _Status),
               new SqlParameter("@LastUpdateTime", DateTime.Now)) > 0;

           return result;
       }
        public Boolean ChangeProcess(Int64 _ID, Int32 _ComplatedCount)
       {
           String Insert_Cmd = "UPDATE [yy_Page_Build_Task] SET BuildCount+=@BuildCount,LastUpdateTime=@LastUpdateTime WHERE ID=@ID";

           Boolean result = SqlHelper.ExecuteNonQuery(DB.Database.Connection.ConnectionString, CommandType.Text, Insert_Cmd,
               new SqlParameter("@ID", _ID),
               new SqlParameter("@BuildCount", _ComplatedCount),
               new SqlParameter("@LastUpdateTime", DateTime.Now)) > 0;

           return result;
       }
        public Boolean ChangeProcess(Int64 _ID, Int64 _ComplatedCount, Int64 _TotalCount)
       {
           String Insert_Cmd = "UPDATE [yy_Page_Build_Task] SET BuildCount+=@BuildCount,TotalCount=@TotalCount,LastUpdateTime=@LastUpdateTime WHERE ID=@ID";

           Boolean result = SqlHelper.ExecuteNonQuery(DB.Database.Connection.ConnectionString, CommandType.Text, Insert_Cmd,
               new SqlParameter("@ID", _ID),
               new SqlParameter("@BuildCount", _ComplatedCount),
               new SqlParameter("@TotalCount", _TotalCount),
               new SqlParameter("@LastUpdateTime", DateTime.Now)) > 0;

           return result;
       }
        public Boolean AddLog(Int64 _ID, String _ErrorMsg)
       {
           String Insert_Cmd = "UPDATE [yy_Page_Build_Task] SET BuildHistory=@BuildHistory+BuildHistory WHERE ID=@ID";

           Boolean result = SqlHelper.ExecuteNonQuery(DB.Database.Connection.ConnectionString, CommandType.Text, Insert_Cmd,
               new SqlParameter("@ID", _ID),
               new SqlParameter("@BuildHistory", _ErrorMsg)) > 0;

           return result;
       }
        #endregion

        #region 互动发送消息
        /// <summary>
        /// 互动发送消息
        /// </summary>
        /// <param name="HubName"></param>
        /// <param name="HubAction"></param>
        public void Signal_Excute(String HubName, Action<IHubProxy> HubAction)
        {
            var HubUrl = ConfigurationManager.AppSettings["AdminSiteUrl"] + "/signalr";

            var Connection = new HubConnection(HubUrl);

            var HubItem = Connection.CreateHubProxy(HubName);

            Connection.Start().ContinueWith(task =>
            {
                if (!task.IsFaulted)
                {
                    HubAction(HubItem);
                }
            }).Wait();

            Connection.Stop();
        }
        #endregion

        Boolean SendMail(String _Title, String _Content)
        {
            var st = DB.yy_SiteSetting.FirstOrDefault();
            var Biz_AdminEmail = st.Email;
            var MailAccount = st.MailAccount;
            var MailUrl = st.MailServer;
            var MailPassword = st.MailPassword;
            var MailPort = st.MailPort;
            var msg = new MailMessage(MailAccount, Biz_AdminEmail);

            msg.SubjectEncoding = Encoding.UTF8;
            msg.BodyEncoding = Encoding.UTF8;
            msg.IsBodyHtml = true;
            msg.Subject = _Title;
            msg.Body = _Content;

            SmtpClient client = new SmtpClient(MailUrl,MailPort);
            client.Credentials = new System.Net.NetworkCredential(MailAccount, MailPassword);
            client.EnableSsl = true;

            try
            {
                client.Send(msg);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
