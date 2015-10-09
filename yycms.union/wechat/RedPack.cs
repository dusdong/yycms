using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Xml.Linq;

namespace yycms.union.wechat
{
   public class RedPack:SDK
    {
       public RedPack(String _Accecc_Token)
        {
            base.Access_Token = _Accecc_Token;
        }

       /// <summary>
       /// 发放红包
       /// </summary>
       /// <returns></returns>
       public String send(String mch_billno, String mch_id, String wxappid, String nick_name,
           String send_name, String re_openid, String total_amount, String min_value, String max_value,
           String total_num, String wishing, String client_ip, String act_name, String act_id,
           String remark, String logo_imgurl, String share_content, String share_url, String share_imgurl,
           String nonce_str, String mch_key, String mch_certPath)
       {
           #region 拼接参数及生成签名
           var param = new SortedList<String, String>();
           if (!String.IsNullOrEmpty(nonce_str)) { param.Add("nonce_str", nonce_str); }
           if (!String.IsNullOrEmpty(mch_billno)) { param.Add("mch_billno", mch_billno); }
           if (!String.IsNullOrEmpty(mch_id)) { param.Add("mch_id", mch_id); }
           if (!String.IsNullOrEmpty(wxappid)) { param.Add("wxappid", wxappid); }
           if (!String.IsNullOrEmpty(nick_name)) { param.Add("nick_name", nick_name); }
           if (!String.IsNullOrEmpty(send_name)) { param.Add("send_name", send_name); }
           if (!String.IsNullOrEmpty(re_openid)) { param.Add("re_openid", re_openid); }
           if (!String.IsNullOrEmpty(total_amount)) { param.Add("total_amount", total_amount); }
           if (!String.IsNullOrEmpty(min_value)) { param.Add("min_value", min_value); }
           if (!String.IsNullOrEmpty(max_value)) { param.Add("max_value", max_value); }
           if (!String.IsNullOrEmpty(total_num)) { param.Add("total_num", total_num); }
           if (!String.IsNullOrEmpty(wishing)) { param.Add("wishing", wishing); }
           if (!String.IsNullOrEmpty(client_ip)) { param.Add("client_ip", client_ip); }
           if (!String.IsNullOrEmpty(act_name)) { param.Add("act_name", act_name); }
           if (!String.IsNullOrEmpty(remark)) { param.Add("remark", remark); }
           if (!String.IsNullOrEmpty(logo_imgurl)) { param.Add("logo_imgurl", logo_imgurl); }
           if (!String.IsNullOrEmpty(share_content)) { param.Add("share_content", share_content); }
           if (!String.IsNullOrEmpty(share_url)) { param.Add("share_url", share_url); }
           if (!String.IsNullOrEmpty(share_imgurl)) { param.Add("share_imgurl", share_imgurl); }
           if (!String.IsNullOrEmpty(act_id)) { param.Add("act_id", act_id); }
           StringBuilder StringA = new StringBuilder();
           foreach (var kv in param)
           {
               StringA.Append(kv.Key + "=" + kv.Value + "&");
           }
           String stringSignTemp = StringA.ToString() + "key=" + mch_key;
           String sign = MD5(stringSignTemp).ToUpper();
           param.Add("sign", sign);
           #endregion

           #region 生成请求字符串
           var RequestBody = new StringBuilder();
           RequestBody.AppendLine("<xml>");
           foreach (var v in param)
           {
               RequestBody.Append("<" + v.Key + ">");
               RequestBody.Append(v.Value);
               RequestBody.AppendLine("</" + v.Key + ">");
           }
           RequestBody.AppendLine("</xml>");
           #endregion

           var ResponseStr = Request_XML(redpack_send, RequestBody.ToString(), mch_id, mch_certPath);

           try
           {
               if (!String.IsNullOrEmpty(ResponseStr))
               {
                   var Res = XElement.Load(new StringReader(ResponseStr));

                   if (Res.Element("return_code").Value.Equals("SUCCESS"))
                   {
                       if (Res.Element("result_code").Value.Equals("SUCCESS"))
                       {
                           String send_listid = Res.Element("send_listid").Value;

                           return send_listid;
                       }
                   }
               }

               return String.Empty;
           }
           catch
           {
               return String.Empty;
           }
       }

        protected String MD5(String str)
        {
            var md5 = new MD5CryptoServiceProvider();
            var bs = Encoding.UTF8.GetBytes(str);
            bs = md5.ComputeHash(bs);
            var s = new StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToUpper());
            }
            var password = s.ToString();
            return password;
        }

    }
}
