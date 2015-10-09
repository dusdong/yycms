using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using yycms.entity;
using yycms.service.PlugIn;
using System.Configuration;
using System.IO;
using System.Net;
using System.Threading;

namespace yycms.service.Queues
{
    public class NewsSync : IQueue
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
            var news = new List<yy_Spider_News>();

            if (String.IsNullOrEmpty(body))
            {
                var spiders = DB.yy_Spider.Where(x => x.SpiderMode == 1).Select(x => x.ID).ToList();

                if (spiders.Count < 1) { return; }

                foreach (var v in spiders)
                {
                    var _news = DB.yy_Spider_News.Where(x => x.SpiderID == v && x.IsSync == 0).ToList();

                    if (_news.Count > 0)
                    {
                        news.AddRange(_news);
                    }
                }
            }

            else
            {
                var IDs = body.Split(new String[1] { "," }, StringSplitOptions.RemoveEmptyEntries);

                long ID = 0;

                foreach(var v in IDs)
                {
                    if(!long.TryParse(v,out ID)) { ID = 0; }

                    if (ID < 1) { continue; }

                    var item = DB.yy_Spider_News.Find(v);

                    if(item!=null)
                    {
                        news.Add(item);
                    }
                }
            }

            if (news.Count < 1) { return; }

            var crtDic = "/Images/" + DateTime.Now.Ticks.ToString() + "/";

            var temp = ConfigurationManager.AppSettings["AdminImagesPath"] + crtDic;

            if (!Directory.Exists(temp)) { Directory.CreateDirectory(temp); }

            foreach (var v in news)
            {
                #region 新闻实体
                var spiderItem = DB.yy_Spider.Where(x => x.ID == v.SpiderID).FirstOrDefault();
                var newsItem = new yy_News()
                {
                    Title = v.Title,
                    DefaultImg = v.DefaultImage,
                    KeyWords = v.KeyWords,
                    Summary = v.Summary,
                    Info = v.Info,
                    TypeIDs = spiderItem.TypeIDs,
                    UserID = spiderItem.UserID,
                    TargetPlatforms = spiderItem.TargetPlatforms,
                    CreateDate = DateTime.Now,
                    IsShow = 1,
                    CanReply = 1,
                    ImgList = "",
                    LookCount = 0,
                    Recommend = 0,
                    ShowIndex = 0,
                    WechatMediaID = "",
                    WechatNewsUrl = ""
                };
                #endregion

                #region 替换默认图片
                if (String.IsNullOrEmpty(newsItem.DefaultImg))
                {
                    newsItem.DefaultImg = "/Images/users.jpg";
                }
                else
                {
                    var dftName = Guid.NewGuid() + System.IO.Path.GetExtension(newsItem.DefaultImg).Replace("!middle", "");
                    var dftIMG = temp + dftName;
                    using (var wb = new WebClient())
                    {
                        wb.DownloadFile(newsItem.DefaultImg,dftIMG);
                    }
                    newsItem.DefaultImg = crtDic + dftName;
                }
                #endregion

                #region 新闻内容里的图片上传到微信服务器并替换地址
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(newsItem.Info);
                var imgs = doc.DocumentNode.SelectNodes("//img");
                if (imgs != null && imgs.Count > 0)
                {
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

                                    imgs[i].Attributes["src"].Value = crtDic + System.IO.Path.GetFileName(src);
                                }
                            }
                        }
                        catch
                        {

                        }
                        Thread.Sleep(1);
                    }

                    newsItem.Info = doc.DocumentNode.OuterHtml;
                }
                #endregion

                DB.yy_News.Add(newsItem);

                v.IsSync = 1;

                DB.SaveChanges();

                MQueue.Send("WechatSync", newsItem.ID.ToString());
            }
            
        }
    }
}
