using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yycms.union.wechat
{
   public class QRCode:SDK
    {

       public QRCode(String _Accecc_Token) { base.Access_Token = _Accecc_Token; }

       /// <summary>
       /// 创建商家二维码
       /// </summary>
       /// <param name="MechantID">商家ID</param>
       /// <returns></returns>
       public String Create(long MechantID) 
       {
           var ReqStr = JsonConvert.SerializeObject(new 
           {
                action_name = "QR_LIMIT_SCENE",
                action_info = new
                {
                    scene = new 
                    {
                        scene_id = MechantID
                    }
                }
            });

           var res = Request(QRCode_Create, ReqStr);

           if (res != null && res.HasValues && res["ticket"]!= null)
           {
               return QRCode_Show + res["ticket"].Value<String>();
           }

           return String.Empty;
       }
    }
}
