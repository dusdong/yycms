using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Messaging;

namespace yycms.service.PlugIn
{
    public class MQueue
    {
        #region 发送消息
       public static void Send(String queueName, String data)
        {
            var QueenBasePath = ConfigurationManager.AppSettings["MessageQueueServer"];

            //新建消息循环队列或连接到已有的消息队列
            var Path = QueenBasePath + queueName;

            MessageQueue mq;

            if (MessageQueue.Exists(Path))
            {
                mq = new MessageQueue(Path);
            }
            else
            {
                mq = MessageQueue.Create(Path);
            }

            mq.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });

            mq.Send(data);
        }
        #endregion
    }
}
