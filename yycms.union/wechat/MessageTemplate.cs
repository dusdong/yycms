using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yycms.union.wechat
{
   public class MessageTemplate:SDK
    {
       public MessageTemplate(String _Accecc_Token)
        {
            base.Access_Token = _Accecc_Token;
        }


       /// <summary>
       /// 发送模板消息
       /// </summary>
       /// <param name="OpenID">用户的OpenID</param>
       /// <param name="TemplateID">模板消息ID</param>
       /// <param name="url">URL置空，则在发送后，点击模板消息会进入一个空白页面（ios），或无法点击（android）。</param>
       /// <param name="topcolor">上边框颜色</param>
       /// <param name="TemplateObject">具体替换的模板内容</param>
       /// <returns></returns>
       public String Send(String OpenID, String TemplateID, String url, String topcolor,Object TemplateObject)
       {
           var ReqStr = JsonConvert.SerializeObject(new
           {
               touser = OpenID,
               template_id = TemplateID,
               url = url,
               topcolor = topcolor,
               data = TemplateObject
           });

           var res = Request(TemplateMsg_Send, ReqStr);

           if (res != null && res.HasValues && res["errcode"].Value<int>() ==0)
           {
               return res["msgid"].Value<String>();
           }

           return String.Empty;
       }
    }
}
