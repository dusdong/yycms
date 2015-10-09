using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yycms.service
{
    public interface IQueue
    {
        /// <summary>
        /// 消息路径
        /// </summary>
        String Path { get; }


        bool Enable { get; }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="context"></param>
        void ReceiveCompleted(String body);
    }
}
