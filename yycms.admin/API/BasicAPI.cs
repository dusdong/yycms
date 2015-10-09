using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Messaging;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using yycms.entity;

namespace yycms.admin.API
{
    public class BasicAPI : ApiController
    {
        #region 数据库
        private DBConnection _DB;
        protected DBConnection DB
        {
            get { if (_DB == null) { _DB = new DBConnection(); } return _DB; }
        }
        #endregion

        #region 用户
        private yy_User _User;
        protected new yy_User User
        {
            get
            {
                if (_User != null) { return _User; }

                String UserStr = String.Empty;

                try
                {
                    UserStr = Request.Headers.GetCookies().FirstOrDefault()
                        .Cookies.FirstOrDefault().Value;

                    UserStr = HttpUtility.UrlDecode(UserStr);
                }
                catch
                {

                }

                if (!String.IsNullOrEmpty(UserStr))
                {
                    _User = JsonConvert.DeserializeObject<yy_User>(UserStr);
                }
                else
                {
                    throw new WebException("无效的用户或没有获取调用权限", WebExceptionStatus.Success);
                }

                return _User;
            }
        }
        #endregion

        #region request
        private HttpRequestBase _request;

        protected HttpRequestBase request
        {
            get
            {
                if (_request == null)
                {
                    HttpContextBase context = (HttpContextBase)Request.Properties["MS_HttpContext"];
                    _request = context.Request;
                }

                return _request;
            }
        }
        #endregion

        protected HttpResponseMessage ResponseMessage(Object _Content, CookieHeaderValue Cookies = null, HttpStatusCode _Code = HttpStatusCode.OK)
        {
            var result = new HttpResponseMessage(HttpStatusCode.OK);

            result.Content = new StringContent(JsonConvert.SerializeObject(_Content), Encoding.UTF8, "application/json");

            if (Cookies != null)
            {
                result.Headers.AddCookies(new CookieHeaderValue[] { Cookies });
            }

            return result;
        }

        /// <summary>
        /// 返回JSON对象到客户端
        /// </summary>
        /// <param name="_ResponseContent">JSON字符串</param>
        /// <returns></returns>
        protected HttpResponseMessage Message(String _ResponseContent = "ok")
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_ResponseContent,
                    Encoding.UTF8,
                    "application/json")
            };
        }

        /// <summary>
        /// 返回JSON对象到客户端
        /// </summary>
        /// <param name="_ResponseContent">JSON字符串</param>
        /// <returns></returns>
        protected HttpResponseMessage Message(Object _ResponseContent)
        {
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(_ResponseContent),
                    Encoding.UTF8,
                    "application/json")
            };
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

        protected T GetValue<T>(JToken json, T defaultValue = default(T))
        {
            if (json != null)
            {
                return json.Value<T>();
            }

            return defaultValue;
        }

        #region datetime转换为unixtime
        /// <summary>
        /// datetime转换为unixtime
        /// </summary>
        protected int ConvertDateTimeToUnixTime(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));

            return (int)(time - startTime).TotalSeconds;
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            DB.Dispose();
            base.Dispose(disposing);
        }


        #region 发送消息
        protected void MessageQueue_Send(String queueName, String data)
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

        /// <summary>
        /// 刷新会话令牌
        /// </summary>
        /// <param name="_Mechant"></param>
        protected yy_Platforms User_Platforms_Wechat()
        {
            var _Mechant = DB.yy_Platforms.Where(x => x.Code == 11 && x.UserID == User.ID).FirstOrDefault();

            #region 如果商家Access_token过期就刷新，调用接口需要用这个
            if (String.IsNullOrEmpty(_Mechant.Access_token) || _Mechant.Access_token_Expires_in < DateTime.Now)
            {
                var _res = new WebClient().DownloadString(
                    String.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}",
                    _Mechant.APPKey, _Mechant.APPSecret));

                var _resObj = JsonConvert.DeserializeObject<JObject>(_res);

                if (_resObj["access_token"] != null)
                {
                    _Mechant.Access_token = _resObj["access_token"].Value<String>();
                    _Mechant.Access_token_Expires_in = DateTime.Now.AddSeconds(_resObj["expires_in"].Value<int>());
                    DB.SaveChanges();
                }
            }
            #endregion
            #region 如果商家jsapi_ticket过期就刷新，网页里用jsAPi需要用这个
            if (String.IsNullOrEmpty(_Mechant.jsapi_ticket) || _Mechant.jsapi_ticket_Expires_in < DateTime.Now)
            {
                var _res = new WebClient().DownloadString(String.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=jsapi",
                    _Mechant.Access_token));
                var _resObj = JsonConvert.DeserializeObject<JObject>(_res);
                if (_resObj["ticket"] != null)
                {
                    _Mechant.jsapi_ticket = _resObj["ticket"].Value<String>();
                    _Mechant.jsapi_ticket_Expires_in = DateTime.Now.AddSeconds(_resObj["expires_in"].Value<int>());
                    DB.SaveChanges();
                }
            }
            #endregion
            #region 如果商家api_ticket过期就刷新,网页里用js发券，做签名时需要用到这个
            if (String.IsNullOrEmpty(_Mechant.api_ticket) || _Mechant.api_ticket_Expires_in < DateTime.Now)
            {
                var _res = new WebClient().DownloadString(String.Format("https://api.weixin.qq.com/cgi-bin/ticket/getticket?access_token={0}&type=wx_card",
                    _Mechant.Access_token));
                var _resObj = JsonConvert.DeserializeObject<JObject>(_res);
                if (_resObj["ticket"] != null)
                {
                    _Mechant.api_ticket = _resObj["ticket"].Value<String>();
                    _Mechant.api_ticket_Expires_in = DateTime.Now.AddSeconds(_resObj["expires_in"].Value<int>());
                    DB.SaveChanges();
                }
            }
            #endregion

            return _Mechant;
        }
    }
}