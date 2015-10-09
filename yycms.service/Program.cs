using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Messaging;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;
using yycms.entity;
using yycms.service.PlugIn;

namespace yycms.service
{
    class Program
    {
        /// <summary>
        /// 消息处理器
        /// </summary>
        static List<Type> Queens
        {
            get
            {
                return Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(x => x.GetInterfaces().Contains(typeof(IQueue)))
                    .ToList();
            }
        }

        static void Main(string[] args)
        {
            #region 启动消息处理器
            var QueenBasePath = ConfigurationManager.AppSettings["MessageQueueServer"];
            foreach (var t in Queens)
            {
                try
                {
                    //创建实例
                    var QueenInstance = Activator.CreateInstance(t) as IQueue;

                    if (!QueenInstance.Enable) { continue; }

                    //新建消息循环队列或连接到已有的消息队列
                    var path = QueenBasePath + QueenInstance.Path;
                    MessageQueue mq;
                    if (MessageQueue.Exists(path))
                    {
                        mq = new MessageQueue(path);
                    }
                    else
                    {
                        mq = MessageQueue.Create(path);
                    }

                    mq.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

                    //mq.Formatter = new JsonFormater();

                    mq.ReceiveCompleted += (sender, e) =>
                    {
                        var m = mq.EndReceive(e.AsyncResult);

                        String body = String.Empty;
                        try
                        {
                            body = m.Body.ToString();
                        }
                        catch { body = String.Empty; }
                        try
                        {
                            QueenInstance.ReceiveCompleted(body);
                            mq.BeginReceive();
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    };
                    mq.BeginReceive();
                }
                catch (Exception ex)
                {
                    new Log().Add(t.Name, JsonConvert.SerializeObject(ex), EventLogEntryType.Error);

                    Console.WriteLine(ex.Message);
                }

                Thread.Sleep(1);
            }
            #endregion

            Console.ForegroundColor = ConsoleColor.Green;

            String ServiceName = ConfigurationManager.AppSettings["ServiceName"];

            if (String.IsNullOrEmpty(ServiceName))
            {
                ServiceName = "yueyaCms.Service";
            }

            Host host = HostFactory.New((config) =>
            {
                config.Service<Engine>();
                //服务的描述
                config.SetDescription("玥雅CMS核心服务。");
                //服务的显示名称
                config.SetDisplayName(ServiceName);
                //服务名称
                config.SetServiceName(ServiceName);
                //以本地服务方式运行
                config.RunAsLocalService();
            });

            host.Run();

            Console.ReadLine();

        }

        
    }
}
