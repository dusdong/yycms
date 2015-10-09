using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using yycms.service.PlugIn;
using Microsoft.AspNet.SignalR.Client;
using System.Configuration;

namespace yycms.service.Queues
{
    public class CodeCompile : IQueue
    {
        public bool Enable
        {
            get
            {
                return true;
            }
        }

        public string Path
        {
            get
            {
                return this.GetType().Name;
            }
        }

        public void ReceiveCompleted(String body)
        {
            if (String.IsNullOrEmpty(body))
            {
                return;
            }

            try
            {
                var result = new PageBuilder().BuildPage(body, 0);

                NSoup.Nodes.Document doc = NSoup.NSoupClient.Parse(result);

                result = doc.OuterHtml();

                Signal_Excute("GlobalHub", x => { x.Invoke("CompileEvent", 0, result); });
            }
            catch (Exception ex)
            {
                Signal_Excute("GlobalHub", x => { x.Invoke("CompileEvent", 1, ex.Message); });
            }
        }

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

    }
}
