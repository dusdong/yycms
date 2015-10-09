using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace yycms.admin
{
    public class GlobalHub : Hub
    {
        /// <summary>
        /// 通知
        /// </summary>
        /// <param name="code">消息代码</param>
        /// <param name="msg">消息内容</param>
        public void Notify(int code,String msg)
        {
            Clients.All.Notify(code, msg);
        }

        /// <summary>
        /// 编译代码通知
        /// </summary>
        /// <param name="code">通知代码</param>
        /// <param name="msg">编译结果</param>
        public void CompileEvent(int code, String msg)
        {
            Clients.All.CompileEvent(code, msg);
        }
    }
}