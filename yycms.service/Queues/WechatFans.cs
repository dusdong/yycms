using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using yycms.entity;
using yycms.union.wechat;
using System.Net;
using Newtonsoft.Json;
using System.Data;
using System.Threading;

namespace yycms.service.Queues
{
    /// <summary>
    /// 微信同步粉丝
    /// </summary>
    public class WechatFans : IQueue
    {
        public bool Enable
        {
            get
            {
                return true;
            }
        }

        public string Path
        {
            get
            {
                return this.GetType().Name;
            }
        }

        #region 数据库操作对象
        DBConnection DB = new DBConnection();
        #endregion

        public void ReceiveCompleted(String body)
        {
            if (String.IsNullOrEmpty(body)) { return; }

            var UserID = long.Parse(body);

            var plt = DB.yy_Platforms.Where(x => x.UserID == UserID && x.Code == 11).FirstOrDefault();

            if (plt == null) { return; }

            freshToken(plt);

            var f = new Fans(plt.Access_token);

            #region TB
            var TB = new DataTable() { TableName = "yy_Fans_Wechat" };
            TB.Columns.Add("ID", typeof(long));
            TB.Columns.Add("UserID", typeof(long));
            TB.Columns.Add("openid", typeof(string));
            TB.Columns.Add("nickname", typeof(string));
            TB.Columns.Add("language", typeof(string));
            TB.Columns.Add("sex", typeof(long));
            TB.Columns.Add("province", typeof(string));
            TB.Columns.Add("city", typeof(string));
            TB.Columns.Add("country", typeof(string));
            TB.Columns.Add("headimgurl", typeof(string));
            TB.Columns.Add("Latitude", typeof(string));
            TB.Columns.Add("Longitude", typeof(string));
            TB.Columns.Add("Precision", typeof(string));
            TB.Columns.Add("remark", typeof(string));
            TB.Columns.Add("unionid", typeof(string));
            TB.Columns.Add("LOCATIONUpdateTime", typeof(DateTime));
            TB.Columns.Add("CreateDate", typeof(DateTime));
            #endregion

            var next_openid = String.Empty;

            while (true)
            {
                var fs = f.Batch_Get(next_openid);

                if (fs.count < 1) { break; }

                foreach (var v in fs.data.openid)
                {
                    var HasOpenID = SqlHelper.ExecuteScalar(DB.Database.Connection.ConnectionString, CommandType.Text, "SELECT ID FROM yy_Fans_Wechat WITH(NOLOCK) WHERE UserID = " + UserID + " AND openid='" + v + "'");

                    if (HasOpenID != null) { continue; }

                    var d = f.Detail(v);

                    TB.Rows.Add(
                        null, 
                        UserID,
                        d.openid, 
                        d.nickname, 
                        d.language,
                        d.sex, 
                        d.province, 
                        d.city, 
                        d.country,
                        d.headimgurl, 
                        d.language, 
                        "", 
                        "", 
                        "", 
                        "",
                        DateTime.Now, 
                        DateTime.Now);
                }

                next_openid = fs.next_openid;

                if (String.IsNullOrEmpty(next_openid)) { break; }

                Thread.Sleep(1);
            }

            if (TB.Rows.Count > 0)
            {
                SqlHelper.TableValuedToDB(DB.Database.Connection.ConnectionString, TB);
            }
        }

        /// <summary>
        /// 刷新会话令牌
        /// </summary>
        /// <param name="_Mechant"></param>
        public void freshToken(yy_Platforms _Mechant)
        {
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
        }
    }
}
