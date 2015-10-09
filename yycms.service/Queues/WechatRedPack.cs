using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yycms.entity;
using yycms.union.wechat;

namespace yycms.service.Queues
{
   public class WechatRedPack:IQueue
    {
       public string Path
       {
           get { return this.GetType().Name; }
       }

        public bool Enable
        {
            get { return true; }
        }

        #region 数据库操作对象
        DBConnection DB = new DBConnection();
        #endregion

        public void ReceiveCompleted(String body)
        {
            if (String.IsNullOrEmpty(body)) { return; }

            //var rp = new RedPack(body.Value<String>("Access_token"));

            //#region 微信发红包固定的订单号格式
            //var yy_RedPack_Order_Count = long.Parse(
            //    SqlHelper.ExecuteScalar(
            //    DB.Database.Connection.ConnectionString,
            //    "SELECT COUNT(1) FROM yy_RedPack_Order WITH(NOLOCK)").ToString());
            //var mch_billNo = body.Value<String>("WechatPayID") +
            //               DateTime.Now.ToString("yyyyMMdd") +
            //               (yy_RedPack_Order_Count + DateTime.Now.Millisecond).ToString();
            //#endregion

            //#region 发红包
            //var sendResult = rp.send(mch_billNo,
            //   body.Value<String>("WechatPayID"),
            //   body.Value<String>("APPID"),
            //   body.Value<String>("nick_name"),
            //   body.Value<String>("send_name"),
            //   body.Value<String>("userOpenID"),
            //   body.Value<String>("money"),
            //   body.Value<String>("money"),
            //   body.Value<String>("money"),
            //   body.Value<String>("total_num"),
            //   body.Value<String>("wishing"),
            //   body.Value<String>("UserHostAddress"),
            //   body.Value<String>("act_name"),
            //   body.Value<String>("act_id"),
            //   body.Value<String>("remark"),
            //   body.Value<String>("logo_imgurl"),
            //   body.Value<String>("share_content"),
            //   body.Value<String>("share_url"),
            //   body.Value<String>("share_imgurl"),
            //   body.Value<String>("nonce_str"),
            //   body.Value<String>("WechatPayKey"),
            //   body.Value<String>("WechatPayCert"));
            //#endregion

            //Console.WriteLine("Redpack_Send：" + sendResult);

            //#region 记录到本地
            //if (!String.IsNullOrEmpty(sendResult))
            //{
            //    String Cmd = String.Format("UPDATE yy_RedPack_Order SET mch_billno='{0}',detail_id='{1}',status='2'" +
            //        " WHERE ID={2}",
            //        mch_billNo,
            //        sendResult,
            //        body.Value<long>("RedpackOrderID"));

            //    SqlHelper.ExecuteScalar(DB.Database.Connection.ConnectionString, Cmd);
            //}
            //#endregion
        }
    }
}
