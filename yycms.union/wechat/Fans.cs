using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yycms.union.wechat
{
   public class Fans:SDK
    {
        public Fans(String _Accecc_Token)
        {
            base.Access_Token = _Accecc_Token;
        }

        #region 查询粉丝
        public FansModel Batch_Get(String next_openid)
        {
            var res = Get("https://api.weixin.qq.com/cgi-bin/user/get?access_token=" + Access_Token + "&next_openid=" + next_openid);

            var result = JsonConvert.DeserializeObject<FansModel>(res);

            return result;
        }
        #endregion

        public FansInfo Detail(String OPENID)
        {
            var res = Get("https://api.weixin.qq.com/cgi-bin/user/info?access_token="+ Access_Token + "&openid="+ OPENID + "&lang=zh_CN");

            var result = JsonConvert.DeserializeObject<FansInfo>(res);

            return result;
        }

        public JObject SendMsg(String openID,String content)
        {
            var ReqStr = JsonConvert.SerializeObject(new
            {
                touser = openID,
                msgtype = "text",
                text = new
                {
                    content = content
                }
            });

            var res = Request("https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token=" + Access_Token, ReqStr);

            return res;
        }

        public JObject SendByImage(String openID, String media_id)
        {
            var ReqStr = JsonConvert.SerializeObject(new
            {
                touser = openID,
                msgtype = "image",
                image = new
                {
                    media_id = media_id
                }
            });

            var res = Request("https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token=" + Access_Token, ReqStr);

            return res;
        }

        public JObject SendByNews(String openID, Object _articles)
        {
            var ReqStr = JsonConvert.SerializeObject(new
            {
                touser = openID,
                msgtype = "news",
                news = new
                {
                    articles = _articles
                }
            });

            var res = Request("https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token=" + Access_Token, ReqStr);

            return res;
        }
    }
}
