using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using yycms.admin.Models;
using yycms.entity;

namespace yycms.admin.API
{
    /// <summary>
    /// 模板页
    /// </summary>
    [BasicAuthen]
    public class PageController : BasicAPI
    {
        #region  模板页列表
        /// <summary>
        /// 获取模板页列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseData<yy_Page>> Get(PageFilterEntity value)
        {
            //查询的表名称
            Type Table = typeof(yy_Page);

            var FormData = await Request.Content.ReadAsAsync<Dictionary<String, String>>();

            #region where condition
            
            //筛选条件
            var Where = String.Empty;

            var WhereBuild = new List<string>();

            #region 广告标题
            if (!String.IsNullOrEmpty(value.Title))
            {
                WhereBuild.Add("Title like '%" + value.Title + "%'");
            }
            #endregion

            #region 广告分类
            if (value.TypeID > 0)
            {
                WhereBuild.Add("TypeID =" + value.TypeID);
            }
            #endregion

            #region 根据时间过滤
            #region 大于等于 开始时间 && 小于等于 结束时间
            if (!String.IsNullOrEmpty(value.StartTime) && !String.IsNullOrEmpty(value.EndTime))
            {
                DateTime st, et;

                if (DateTime.TryParse(value.StartTime, out st) && DateTime.TryParse(value.EndTime, out et))
                {
                    WhereBuild.Add(" CreateDate >= '" + st.ToString("yyyy-MM-dd") + " 00:00:00' AND CreateDate <= '" + et.ToString("yyyy-MM-dd") + " 23:59:59'");
                }
            }
            #endregion
            #region 大于等于开始时间
            else if (!String.IsNullOrEmpty(value.StartTime))
            {
                DateTime st;
                if (DateTime.TryParse(value.StartTime, out st))
                {
                    WhereBuild.Add(" CreateDate >= '" + st.ToString("yyyy-MM-dd") + " 00:00:00'");
                }
            }
            #endregion
            #region 小于等于结束时间
            else if (!String.IsNullOrEmpty(value.EndTime))
            {
                DateTime et;
                if (DateTime.TryParse(value.EndTime, out et))
                {
                    WhereBuild.Add(" CreateDate <= '" + et.ToString("yyyy-MM-dd") + " 23:59:59'");
                }
            }
            #endregion
            #endregion

            #region 模板页类型
            if (value.PageKind > 0)
            {
                WhereBuild.Add("PageKind =" + value.PageKind);
            }
            #endregion

            #region 是否集成了一键生成
            if (value.CanBuild > 0)
            {
                WhereBuild.Add("CanBuild =" + value.CanBuild);
            }
            #endregion

            if (WhereBuild.Count > 0)
            {
                Where = " WHERE " + String.Join(" AND ", WhereBuild);
            }
            #endregion

            #region columns
            var Columns = Table.GetProperties();
            #endregion

            #region OrderBy
            //排序规则
            String OrderBy = " ID DESC ";
            if (!String.IsNullOrEmpty(value.OrderBy))
            {
                OrderBy = " " + value.OrderBy + " " + (value.IsDesc ? "DESC" : "ASC");
            }
            #endregion

            #region 拼接sql语句
            String colsStr = " ID,PageName,BuildType,TypeID,PageKind,SavePath,ExtensionName,Summary,'' AS PageCode,CanBuild,CreateDate,LastUpdateTime";
            #region  查询数据
            String QuertCMD = String.Format(@"SELECT TOP {0} * FROM (
                                SELECT ROW_NUMBER() OVER (ORDER BY {4}) AS RowNumber," + colsStr + @" FROM {2} WITH(NOLOCK) {3} 
                                ) A WHERE RowNumber > {0} * ({1}-1)", value.PageSize, value.PageIndex + 1, "[" + Table.Name + "]", Where, OrderBy);
            #endregion
            #region 查询总数
            String DataCountCMD = @"SELECT COUNT(1) FROM [" + Table.Name + "] WITH(NOLOCK) " + Where;
            #endregion
            #endregion

            #region 执行查询并返回数据
            var DataCount = DB.Database.SqlQuery<int>(DataCountCMD).FirstOrDefault();
            return new ResponseData<yy_Page>(value.PageSize,
                value.PageIndex,
                DataCount,
                (DataCount % value.PageSize == 0 ? DataCount / value.PageSize : DataCount / value.PageSize + 1),
                DB.Database.SqlQuery<yy_Page>(QuertCMD).ToList());
            #endregion
        }
        #endregion

        #region 模板页详情
        /// <summary>
        /// 获取模板页详情
        /// </summary>
        /// <param name="id">模板页ID</param>
        /// <returns></returns>
        [HttpGet]

        public yy_Page Get(int id)
        {
            //模板页详情
            return DB.yy_Page.Find(id);
        }
        #endregion

        #region 添加模板页
        /// <summary>
        /// 添加模板页
        /// </summary>
        /// <param name="value">模板页实体</param>
        [HttpPost]
        public ResponseItem Post(yy_Page value)
        {
            try
            {
                value.PageCode = HttpUtility.UrlDecode(value.PageCode);
                DB.yy_Page.Add(value);
                DB.SaveChanges();

                return new ResponseItem(0, "添加模板页成功。");
            }
            catch (Exception ex)
            {
                return new ResponseItem(2, ex.Message);
            }
        }
        #endregion

        #region 修改模板页
        /// <summary>
        /// 修改模板页
        /// </summary>
        /// <param name="value">模板页详情</param>
        [HttpPut]
        public ResponseItem Put(yy_Page value)
        {
            var _Entity = DB.yy_Page.Find(value.ID);

            if (_Entity != null)
            {
                _Entity.BuildType = value.BuildType;
                _Entity.CanBuild = value.CanBuild;
                _Entity.CreateDate = value.CreateDate;
                _Entity.ExtensionName = value.ExtensionName;
                _Entity.LastUpdateTime = value.LastUpdateTime;
                _Entity.PageCode = HttpUtility.UrlDecode(value.PageCode);
                _Entity.PageKind = value.PageKind;
                _Entity.PageName = value.PageName;
                _Entity.SavePath = value.SavePath;
                _Entity.Summary = value.Summary;
                _Entity.TypeID = value.TypeID;
                DB.SaveChanges();
                return new ResponseItem(0, "");
            }

            return new ResponseItem(2, "不存在的模板页。");
        }
        #endregion

        #region 删除模板页
        /// <summary>
        /// 删除模板页
        /// </summary>
        /// <param name="id">模板页ID</param>
        [HttpDelete]
        public ResponseItem Delete(int id)
        {
            DB.Database.ExecuteSqlCommand("DELETE yy_Page WHERE ID = " + id);
            return new ResponseItem(0, "");
        }
        #endregion

        #region 根据ID批量删除模板页
        /// <summary>
        /// 根据ID批量删除模板页
        /// </summary>
        /// <param name="ids">模板页ID集合，用英文逗号链接。</param>
        [HttpDelete]
        public ResponseItem DeleteByIDs(String ids)
        {
            var IDs = ids.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var v in IDs)
            {
                long id = 0;

                if(long.TryParse(v,out id))
                { 
                    DB.Database.ExecuteSqlCommand("DELETE yy_Page WHERE ID = " + id); 
                }
            }

            return new ResponseItem(0, "");
        }
        #endregion

        #region 生成模板页时数据的筛选配置
        /// <summary>
        /// 生成模板页时数据的筛选配置
        /// </summary>
        /// <param name="value">筛选配置实体。</param>
        [HttpPost]
        public ResponseItem Config(yy_Page_Build_Config value)
        {
            var configItem = DB.yy_Page_Build_Config
                .Where(x => x.PageTypeID == value.PageTypeID &&
                    x.BuildType == value.BuildType).FirstOrDefault();

            if (configItem == null)
            {
                DB.yy_Page_Build_Config.Add(value);
                DB.SaveChanges();
            }
            else 
            {
                configItem.StartTime = value.StartTime;
                configItem.EndTime = value.EndTime;
                configItem.PageID = value.PageID;
                configItem.DataTypeIDs = value.DataTypeIDs;
                DB.SaveChanges();
            }

            return new ResponseItem(0, "");
        }
        #endregion

        #region 发布生成任务
        /// <summary>
        /// 发布模板页生成任务
        /// </summary>
        /// <param name="typeid">模板包ID。</param>
        /// <param name="ids">模板页ID集合。</param>
        [HttpPost]
        public ResponseItem AddTask(long typeid, String ids)
        {
            var _IDs = ids.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            if (_IDs.Length < 1) { return new ResponseItem(0, ""); ; }

            var IDs = _IDs.Select(x => long.Parse(x)).ToList();

            var TypeConfig = DB.yy_Page_Build_Config.Where(x => x.PageTypeID == typeid).ToList();

            var QueryCMDs = new String[5]
            {
                "",
                "SELECT ID FROM yy_News WITH(NOLOCK)",
                "SELECT ID FROM yy_Product WITH(NOLOCK)",
                "SELECT ID FROM yy_Video WITH(NOLOCK)",
                "SELECT ID FROM yy_Photo WITH(NOLOCK)"
            };

            foreach (var v in IDs)
            {
                var DetailPage = TypeConfig.Where(x => x.PageID == v).FirstOrDefault();

                var PageName = DB.Database.SqlQuery<String>("SELECT PageName from yy_Page WITH(NOLOCK) where ID = " + v).FirstOrDefault();

                if (DetailPage!=null)
                {
                    var DataIDs = new List<long>();

                    #region 根据分类筛选出ID
                    var TypeIDs = DetailPage.DataTypeIDs.Split(new String[] { "," },StringSplitOptions.RemoveEmptyEntries);
                    if (TypeIDs.Length > 0)
                    {
                        foreach (var vv in TypeIDs)
                        {
                            var _IDResult = DB.Database.
                                SqlQuery<long>(QueryCMDs[DetailPage.BuildType] + " WHERE TypeIDs like '%," + vv + ",%'").ToList();

                            DataIDs.AddRange(_IDResult);
                        }
                    }
                    #endregion

                    #region 全部分类
                    else 
                    {
                        var _IDResult = DB.Database.
                              SqlQuery<long>(QueryCMDs[DetailPage.BuildType]).ToList();

                        DataIDs.AddRange(_IDResult);
                    }
                    #endregion

                    if (DataIDs.Count < 1) 
                    {
                        continue; 
                    }

                    Add(PageName, typeid, v, JsonConvert.SerializeObject(DataIDs), DataIDs.Count);
                }
                else 
                {
                    Add(PageName, typeid, v, JsonConvert.SerializeObject(new long[] { 0 }), 1);
                }

                Thread.Sleep(1);
            }

            MessageQueue_Send("PageBuild", null);

            return new ResponseItem(0, "");
        }

        /// <summary>
        /// 重新运行生成任务
        /// </summary>
        /// <param name="taskid">任务ID。</param>
        [HttpPost]
        public ResponseItem RetryTask(long taskid) 
        {
            DB.Database.ExecuteSqlCommand("UPDATE yy_Page_Build_Task SET BuildHistory='',BuildCount = 0,Status=0,LastUpdateTime=GETDATE() WHERE ID = " + taskid);
            
            MessageQueue_Send("PageBuild", null);

            return new ResponseItem(0, "");
        }

        Boolean Add(String _FullName, Int64 _TypeID, Int64 _PageID, String _BuildEntity, Int64 _TotalCount)
        {
            String Fields = "[FullName],[TypeID],[PageID],[BuildEntity],[TotalCount],[BuildCount],[Status],[BuildHistory],[CreateDate],[LastUpdateTime]";
            String Values = "@FullName,@TypeID,@PageID,@BuildEntity,@TotalCount,0,0,'',@CreateDate,@LastUpdateTime";
            String Insert_Cmd = "INSERT INTO [yy_Page_Build_Task](" + Fields + ") VALUES (" + Values + ")";
            String CurrentDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Boolean result = DB.Database.ExecuteSqlCommand(Insert_Cmd,
                new SqlParameter("@FullName", _FullName),
                new SqlParameter("@TypeID", _TypeID),
                new SqlParameter("@PageID", _PageID),
                new SqlParameter("@BuildEntity", _BuildEntity),
                new SqlParameter("@TotalCount", _TotalCount),
                new SqlParameter("@CreateDate", CurrentDateTime),
                new SqlParameter("@LastUpdateTime", CurrentDateTime)) > 0;
            return result;
        }
        #endregion

        #region  生成任务列表
        /// <summary>
        /// 生成任务列表
        /// </summary>
        /// <returns>生成任务列表</returns>
        [HttpPost]
        public async Task<ResponseData<yy_Page_Build_Task>> Task_Get(TaskFilterEntity value)
        {
            //查询的表名称
            Type Table = typeof(yy_Page_Build_Task);

            var FormData = await Request.Content.ReadAsAsync<Dictionary<String, String>>();

            #region where condition
            //筛选条件
            var Where = String.Empty;

            var WhereBuild = new List<string>();

            #region 生成任务标题
            if (!String.IsNullOrEmpty(value.Title))
            {
                WhereBuild.Add("Title like '%" + value.Title + "%'");
            }
            #endregion

            #region 生成任务分类
            if (value.TypeID > 0)
            {
                WhereBuild.Add("TypeID =" + value.TypeID);
            }
            #endregion

            #region 页面ID
            if (value.PageID > 0)
            {
                WhereBuild.Add("PageID =" + value.PageID);
            }
            #endregion

            #region 根据时间过滤
            #region 大于等于 开始时间 && 小于等于 结束时间
            if (!String.IsNullOrEmpty(value.StartTime) && !String.IsNullOrEmpty(value.EndTime))
            {
                DateTime st, et;

                if (DateTime.TryParse(value.StartTime, out st) && DateTime.TryParse(value.EndTime, out et))
                {
                    WhereBuild.Add(" CreateDate >= '" + st.ToString("yyyy-MM-dd") + " 00:00:00' AND CreateDate <= '" + et.ToString("yyyy-MM-dd") + " 23:59:59'");
                }
            }
            #endregion
            #region 大于等于开始时间
            else if (!String.IsNullOrEmpty(value.StartTime))
            {
                DateTime st;
                if (DateTime.TryParse(value.StartTime, out st))
                {
                    WhereBuild.Add(" CreateDate >= '" + st.ToString("yyyy-MM-dd") + " 00:00:00'");
                }
            }
            #endregion
            #region 小于等于结束时间
            else if (!String.IsNullOrEmpty(value.EndTime))
            {
                DateTime et;
                if (DateTime.TryParse(value.EndTime, out et))
                {
                    WhereBuild.Add(" CreateDate <= '" + et.ToString("yyyy-MM-dd") + " 23:59:59'");
                }
            }
            #endregion
            #endregion

            if (WhereBuild.Count > 0)
            {
                Where = " WHERE " + String.Join(" AND ", WhereBuild);
            }
            #endregion

            #region columns
            var Columns = Table.GetProperties();
            #endregion

            #region OrderBy
            //排序规则
            String OrderBy = " ID DESC ";
            if (!String.IsNullOrEmpty(value.OrderBy))
            {
                OrderBy = " " + value.OrderBy + " " + (value.IsDesc ? "DESC" : "ASC");
            }
            #endregion

            #region 拼接sql语句
            String colsStr = " ID,FullName,TypeID,PageID ,'' AS BuildEntity,TotalCount,BuildCount,Status,BuildHistory,CreateDate,LastUpdateTime";
            #region  查询数据
            String QuertCMD = String.Format(@"SELECT TOP {0} * FROM (
                                SELECT ROW_NUMBER() OVER (ORDER BY {4}) AS RowNumber," + colsStr + @" FROM {2} WITH(NOLOCK) {3} 
                                ) A WHERE RowNumber > {0} * ({1}-1)", value.PageSize, value.PageIndex + 1, "[" + Table.Name + "]", Where, OrderBy);
            #endregion
            #region 查询总数
            String DataCountCMD = @"SELECT COUNT(1) FROM [" + Table.Name + "] WITH(NOLOCK) " + Where;
            #endregion
            #endregion

            #region 执行查询并返回数据
            var DataCount = DB.Database.SqlQuery<int>(DataCountCMD).FirstOrDefault();
            return new ResponseData<yy_Page_Build_Task>(value.PageSize,
                value.PageIndex,
                DataCount,
                (DataCount % value.PageSize == 0 ? DataCount / value.PageSize : DataCount / value.PageSize + 1),
                DB.Database.SqlQuery<yy_Page_Build_Task>(QuertCMD).ToList());
            #endregion
        }
        #endregion

        #region 模板页编译
        /// <summary>
        /// 模板页编译
        /// </summary>
        /// <param name="value">单个模板页实体。</param>
        /// <returns></returns>
        [HttpPost]
        public ResponseItem Build(yy_Page value)
        {
            if (String.IsNullOrEmpty(value.PageCode)) 
            {
                return new ResponseItem(1, "页面源码不能为空。");
            }
            try
            {
                MessageQueue_Send("CodeCompile", value.PageCode);

                return new ResponseItem(0, String.Empty);
            }
            catch(Exception ex)
            {
                return new ResponseItem(2, ex.Message + ex.StackTrace);
            }
        }
        #endregion
    }
}
