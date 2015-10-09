using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using yycms.union.wechat;
using yycms.entity;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Configuration;

namespace yycms.service.Queues
{
    /// <summary>
    /// 微信同步素材
    /// </summary>
    public class WechatSync : IQueue
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

            var NewsID = long.Parse(body);

            var news = DB.yy_News.Find(NewsID);

            if (news == null) { return; }

            var plt = DB.yy_Platforms.Where(x => x.UserID == news.UserID && x.Code == 11).FirstOrDefault();

            if (plt == null) { return; }

            freshToken(plt);

            var m = new Material(plt.Access_token);

            #region 新闻内容里的图片上传到微信服务器并替换地址
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(news.Info);
            var imgs = doc.DocumentNode.SelectNodes("//img");
            if (imgs != null)
            {
                var temp = ConfigurationManager.AppSettings["AdminImagesPath"];
                if (!Directory.Exists(temp)) { Directory.CreateDirectory(temp); }
                for (var i = 0; i < imgs.Count; i++)
                {
                    try
                    {
                        if (imgs[i].HasAttributes && imgs[i].Attributes["src"] != null)
                        {
                            var src = imgs[i].Attributes["src"].Value;

                            if (src.IndexOf("http://") >= 0)
                            {
                                var savePath = temp + System.IO.Path.GetFileName(src).Replace("!middle", "");

                                using (var wc = new WebClient())
                                {
                                    if (!File.Exists(savePath))
                                    {
                                        wc.DownloadFile(src, savePath);
                                    }
                                }

                                src = savePath;
                            }
                            else
                            {
                                src = temp + src;
                            }

                            var wechat_pic_url = m.Image_Upload(src);

                            imgs[i].Attributes["src"].Value = wechat_pic_url;
                        }
                    }
                    catch
                    {

                    }
                    Thread.Sleep(1);
                }
            }
            #endregion

            var ImagePath = ConfigurationManager.AppSettings["AdminImagesPath"] + news.DefaultImg;

            if (news.DefaultImg.Contains("!middle"))
            {
                news.DefaultImg = news.DefaultImg.Replace("!middle", "");

                File.Move(ImagePath, ImagePath.Replace("!middle", ""));

                ImagePath = ImagePath.Replace("!middle", "");

                DB.SaveChanges();
            }

            var mediaItem = m.Add(news.Title, "", news.Summary, 0, doc.DocumentNode.OuterHtml, "", ImagePath);

            if (mediaItem["media_id"] != null)
            {
                news.WechatMediaID = mediaItem["media_id"].Value<String>();

                var detailItem = m.Detail(news.WechatMediaID);

                if (detailItem != null && 
                    detailItem["news_item"] != null && 
                    detailItem["news_item"][0]!=null)
                {
                    news.WechatNewsUrl = detailItem["news_item"][0]["url"].Value<String>();
                }

                DB.SaveChanges();
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
