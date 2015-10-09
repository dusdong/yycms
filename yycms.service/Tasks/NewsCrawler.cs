using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NSoup;
using NSoup.Nodes;
using PanGu;
using Quartz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Security;
using yycms.entity;
using yycms.service;
using yycms.service.PlugIn;

namespace yueyaCMS.Service.Task
{
    /// <summary>
    /// 爬虫抓取服务 1/分钟/次
    /// </summary>
    [DisallowConcurrentExecution]
    public class NewsCrawler : ITask
    {
        enum SpiderStatus
        {
            WaitRun = 0,
            Running = 1,
            Complate = 2
        }

        public int WithIntervalInSeconds
        {
            get { return 60; }
        }

        #region 数据库操作对象
        DBConnection DB = new DBConnection();
        #endregion

        public void Run(Quartz.IJobExecutionContext context)
        {
            //获取可用蜘蛛
            //待执行状态
            //开启状态
            //间隔时间符合条件
            var SpiderCollection = DB.Database.SqlQuery<yy_Spider>("SELECT * FROM yy_Spider WITH(NOLOCK) WHERE Status = 0 AND IsShow = 1 AND DATEADD(second,ExecutionInterval,LastStartTime) < GETDATE()").ToList();

            if (SpiderCollection.Count > 0)
            {
                foreach (var v in SpiderCollection)
                {
                    try
                    {
                        //蜘蛛开始执行任务
                        Start(v);
                    }
                    catch (Exception ex)
                    {
                        Add(ex);
                    }
                    finally
                    {
                        
                    }
                }
            }

            //状态重置
            DB.Database.ExecuteSqlCommand("UPDATE yy_Spider SET Status=0");

            MQueue.Send("NewsSync", null);
        }

        void Start(yy_Spider Spider)
        {
            var _SpiderID = Spider.ID;

            var Urls = JsonConvert.DeserializeObject<String[]>(Spider.SourceUrls);

            var RuleConfig = JsonConvert.DeserializeObject<JObject>(Spider.RuleConfig);

            #region 1，更新爬虫状态为[执行中]
            Spider.Status = (int)SpiderStatus.Running;
            SqlHelper.ExecuteNonQuery(DB.Database.Connection.ConnectionString, CommandType.Text,
                "UPDATE yy_Spider SET Status=@Status,LastStartTime=getdate() WHERE ID=@ID",
                new SqlParameter("@Status", (int)SpiderStatus.Running),
                new SqlParameter("@ID", _SpiderID));
            #endregion

            Boolean BreakAll = false;

            foreach (var SourceUrl in Urls)
            {
                var NewsCollection = DataList_Convert(_SpiderID, SourceUrl, RuleConfig, Spider.UserID);

                #region 2，遍历信息源
                foreach (var NewsItem in NewsCollection)
                {
                    NewsItem.Identifer = NewsItem.Title.GetHashCode();

                    var ExistsNewsItem =DB.yy_Spider_News.Where(a => a.Identifer == NewsItem.Identifer).FirstOrDefault();

                    #region 如果已经存在的信息源属于当前爬虫，并且发布时间大于等于爬虫最后执行时间，就结束任务
                    if (ExistsNewsItem != null &&
                        ExistsNewsItem.SpiderID == Spider.ID &&
                        ExistsNewsItem.CreateDate >= Spider.LastStartTime)
                    {
                        BreakAll = true;
                        break;
                    }
                    #endregion

                    #region 如果当前信息源不存在，就添加
                    if (ExistsNewsItem == null)
                    {
                       DB.yy_Spider_News.Add(NewsItem);
                       DB.SaveChanges();
                       continue;
                    }
                    #endregion

                    #region 如果信息源存在，进行数据整合
                    else
                    {
                        Boolean Merged = false;

                        #region 关键词、详情
                        if (String.IsNullOrEmpty(ExistsNewsItem.KeyWords) && !String.IsNullOrEmpty(NewsItem.KeyWords))
                        {
                            ExistsNewsItem.KeyWords = NewsItem.KeyWords;
                            ExistsNewsItem.Info = NewsItem.Info;
                            Merged = true;
                        }
                        else if (!String.IsNullOrEmpty(ExistsNewsItem.KeyWords) && !String.IsNullOrEmpty(NewsItem.KeyWords))
                        {
                            var KeyWordsCount = ExistsNewsItem.KeyWords.Split(',').Length;
                            var NewKeyWordsCount = NewsItem.KeyWords.Split(',').Length;

                            if (NewKeyWordsCount > KeyWordsCount)
                            {
                                ExistsNewsItem.KeyWords = NewsItem.KeyWords;
                                ExistsNewsItem.Info = NewsItem.Info;
                                Merged = true;
                            }
                        }
                        #endregion

                        #region 图片
                        if (String.IsNullOrEmpty(ExistsNewsItem.DefaultImage) && !String.IsNullOrEmpty(NewsItem.DefaultImage))
                        {
                            ExistsNewsItem.DefaultImage = NewsItem.DefaultImage;
                            Merged = true;
                        }
                        #endregion

                        #region 描述
                        if (String.IsNullOrEmpty(ExistsNewsItem.Summary) && !String.IsNullOrEmpty(NewsItem.Summary))
                        {
                            ExistsNewsItem.Summary = NewsItem.Summary;
                            Merged = true;
                        }
                        #endregion

                        if (Merged)
                        {
                            SqlHelper.ExecuteNonQuery(DB.Database.Connection.ConnectionString, 
                                CommandType.Text,
                                "UPDATE yy_Spider_News SET KeyWords=@KeyWords,Info=@Info,Summary=@Summary,DefaultImage=@DefaultImage,LastStartTime=getdate() WHERE ID=@ID",
                                new SqlParameter("@KeyWords", ExistsNewsItem.KeyWords),
                                new SqlParameter("@Info", ExistsNewsItem.Info),
                                new SqlParameter("@Summary", ExistsNewsItem.Summary),
                                new SqlParameter("@DefaultImage", ExistsNewsItem.DefaultImage),
                                new SqlParameter("@ID", ExistsNewsItem.ID));
                        }
                    }
                    #endregion

                    Thread.Sleep(1);
                }
                #endregion

                if (BreakAll) { break; }

                Thread.Sleep(1);
            }

            #region 3，更新爬虫状态[已完成]
            if (Spider.Status == (int)SpiderStatus.Running)
            {
                SqlHelper.ExecuteNonQuery(DB.Database.Connection.ConnectionString, CommandType.Text,
                    "UPDATE yy_Spider SET Status=@Status,LastStartTime=@LastStartTime WHERE ID=@ID",
                                new SqlParameter("@Status", (int)SpiderStatus.Complate),
                                new SqlParameter("@LastStartTime", DateTime.Now),
                                new SqlParameter("@ID", _SpiderID));
            }
            #endregion
        }

        List<yy_Spider_News> DataList_Convert(Int64 _SpiderID, String _Url, JObject _Config,long _UserID)
        {
            String Html = Get(_Url);

            var doc = NSoupClient.Parse(Html);
            
            String SourceListMatch = _Config["SourceListMatch"].ToString();

            var SourceList = doc.Select(SourceListMatch);

            var Result = new List<yy_Spider_News>();

            if (SourceList.Count < 1) { return Result; }

            for (int i = 0; i < SourceList.Count; i++)
            {
                var v = SourceList[i];

                var Item = new yy_Spider_News() { SpiderID = _SpiderID, UserID = _UserID, IsSync = 0 };

                #region 转换
                try
                {
                    #region Title
                    var TitleMatch = _Config["TitleMatch"].ToString();
                    var _TitleEntity = v.Select(TitleMatch).FirstOrDefault();
                    if (_TitleEntity != null)
                    {
                        var _Title = _TitleEntity.OwnText();
                        var _TitleReplace = string.Empty;
                        if (_Config["TitleReplace"] != null)
                        {
                            _TitleReplace = _Config["TitleReplace"].ToString(); 
                        }
                        Item.Title = Str_Replace(_Title, _TitleReplace);
                    }
                    else
                    {
                        Item.Title = "";
                    }
                    #endregion

                    #region DefaultImg
                    var DefaultImgMatch = _Config["DefaultImgMatch"].ToString();
                    var _DefaultImageEntity = v.Select(DefaultImgMatch).FirstOrDefault();
                    if (_DefaultImageEntity != null)
                    {
                        var _DefaultImage = _DefaultImageEntity.Attributes["src"].ToString();
                        var _DefaultImgReplace = string.Empty;
                        if (_Config["DefaultImgReplace"] != null)
                        {
                            _DefaultImgReplace = _Config["DefaultImgReplace"].ToString();
                        }
                        Item.DefaultImage = Str_Replace(_DefaultImage, _DefaultImgReplace);
                    }
                    else { Item.DefaultImage = ""; }
                    #endregion

                    #region Summary
                    var SummaryMatch = _Config["SummaryMatch"].ToString();
                    var _SummaryEntity = v.Select(SummaryMatch).FirstOrDefault();
                    if (_SummaryEntity != null)
                    {
                        String _Summary = _SummaryEntity.OwnText();
                        String _SummaryReplace = string.Empty;
                        if (_Config["SummaryReplace"] != null)
                        {
                            _SummaryReplace = _Config["SummaryReplace"].ToString();
                        }
                        Item.Summary = Str_Replace(_Summary, _SummaryReplace);
                    }
                    else { Item.Summary = ""; }
                    #endregion

                    #region SourceFrom
                    var SourceFromMatch = _Config["SourceFromMatch"].ToString();
                    var _SourceUrlEntity = v.Select(SourceFromMatch).FirstOrDefault();
                    if (_SourceUrlEntity != null)
                    {
                        String SourceFrom = _SourceUrlEntity.Attributes["href"].ToString();
                        String SourceFromReplace = string.Empty;
                        if (_Config["SourceFromReplace"] != null)
                        {
                            SourceFromReplace = _Config["SourceFromReplace"].ToString();
                        }
                        Item.SourceUrl = Str_Replace(SourceFrom, SourceFromReplace);
                    }
                    else { Item.SourceUrl = ""; }
                    #endregion

                    #region Info
                    var InfoMatch = _Config["InfoMatch"].ToString();
                    var InfoSource = Get(Item.SourceUrl);
                    var InfoDoc =NSoupClient.Parse(InfoSource);
                    var InfoNode = InfoDoc.Select(InfoMatch).FirstOrDefault();
                    if (InfoNode != null)
                    {
                        var InfoStr = InfoNode.OuterHtml();
                        var InfoReplace = string.Empty;
                        if (_Config["InfoReplace"] != null)
                        {
                            InfoReplace = _Config["InfoReplace"].ToString();
                        } 
                        InfoStr = Str_Replace(InfoStr, InfoReplace);
                        Item.Info = InfoStr;
                    }
                    else { Item.Info = ""; }
                    #endregion

                    #region KeyWords
                    Segment.Init(Environment.CurrentDirectory + "\\Segment.xml");
                    Segment segment = new Segment();
                    ICollection<WordInfo> words = segment.DoSegment(Item.Info);
                    var Words = words.Where(a =>
                        a.Word.Length > 1 &&
                        (a.OriginalWordType == WordType.English ||
                        a.OriginalWordType == WordType.SimplifiedChinese ||
                        a.OriginalWordType == WordType.TraditionalChinese ||
                        a.OriginalWordType == WordType.Synonym)
                        ).OrderBy(a => a.Rank).Select(a => a.Word).Distinct().Take(20).ToArray();

                    Item.KeyWords = String.Join(",", Words);
                    #endregion

                    #region ReleaseTime
                    var ReleaseTimeMatch = _Config["ReleaseTimeMatch"].ToString();
                    var _CreateDateEntity = InfoDoc.Select(ReleaseTimeMatch).FirstOrDefault();
                    if (_CreateDateEntity != null)
                    {
                        String _CreateDate = _CreateDateEntity.OwnText();
                        String _ReleaseTimeReplace = string.Empty;
                        if (_Config["ReleaseTimeReplace"] != null)
                        {
                            _ReleaseTimeReplace = _Config["ReleaseTimeReplace"].ToString();
                        } 
                        Item.CreateDate = DateTime.Parse(Str_Replace(_CreateDate, _ReleaseTimeReplace));
                    }
                    else { Item.CreateDate = DateTime.Now; }
                    #endregion

                    #region 如果默认图片为空，从详情中匹配
                    if (String.IsNullOrEmpty(Item.DefaultImage))
                    {
                        Regex regImg = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>",
                            RegexOptions.IgnoreCase);
                        MatchCollection matches = regImg.Matches(Item.Info);
                        foreach (Match match in matches)
                        {
                            if (!String.IsNullOrEmpty(match.Groups["imgUrl"].Value))
                            {
                                Item.DefaultImage = match.Groups["imgUrl"].Value;
                            }
                            break;
                        }
                    }
                    #endregion
                }
                catch 
                {
                    Item = null; 
                }
                #endregion

                if (Item != null)
                {
                    Result.Add(Item);
                }

                Thread.Sleep(1);
            }

            #region 记录历史，重新计算蜘蛛质量
            DB.Database.ExecuteSqlCommand("INSERT INTO yy_Spider_Log ([SpiderID],[TotalUrl],[SuccessUrl])VALUES(" + _SpiderID + "," + SourceList.Count + "," + Result.Count + ")");
            DB.Database.ExecuteSqlCommand(string.Format(@"UPDATE [dbo].[yy_Spider] SET 
            Quality = (SELECT SUM(TotalUrl) / SUM(SuccessUrl) * 100 FROM [dbo].[yy_Spider_Log] 
            WITH(NOLOCK) WHERE SpiderID={0}),LastStartTime=getdate() WHERE ID = {0}", _SpiderID));
            #endregion

            return Result;
        }

        String Str_Replace(String _txt, String _exp)
        {
            if (String.IsNullOrEmpty(_exp))
            {
                return _txt;
            }

            var arr = _exp.Split(new String[1] { "," }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var v in arr)
            {
                var ar = v.Split(new String[1] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                if (ar.Length == 1) 
                {
                    _txt = _txt.Replace(ar[0], "");
                }

                else if (ar.Length == 2)
                {
                    _txt = _txt.Replace(ar[0], ar[1]); 
                }
            }

            return _txt.Trim();
        }

        String Get(String _Url)
        {
            using (var wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;

                return wc.DownloadString(_Url);
            }
        }

        void Add(Exception ex)
        {
            var _Error = new yy_SysError()
            {
                HelpLink = ex.HelpLink ?? "",
                Message = ex.Message ?? "",
                Source = ex.Source ?? "",
                StackTrace = ex.StackTrace ?? "",
                TargetSite = ex.TargetSite.Name ?? ""
            };

            _Error.CreateDate = DateTime.Now;

            _Error.LogKey =
                MD5(_Error.Message +
                _Error.HelpLink +
                _Error.Source +
                _Error.StackTrace +
                _Error.TargetSite);

            String Insert_Cmd = "INSERT INTO yy_SysError ([LogKey],[Message],[Source],[StackTrace],[HelpLink],[TargetSite],[CreateDate])" +
                " VALUES (@LogKey,@Message,@Source,@StackTrace,@HelpLink,@TargetSite,@CreateDate)";

            SqlHelper.ExecuteNonQuery(DB.Database.Connection.ConnectionString, CommandType.Text, Insert_Cmd,
                new SqlParameter("@LogKey", _Error.LogKey),
                new SqlParameter("@Message", _Error.Message),
                new SqlParameter("@Source", _Error.Source),
                new SqlParameter("@StackTrace", _Error.StackTrace),
                new SqlParameter("@HelpLink", _Error.HelpLink),
                new SqlParameter("@TargetSite", _Error.TargetSite),
                new SqlParameter("@CreateDate", _Error.CreateDate));
        }

        String MD5(String _text)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                byte[] result = md5.ComputeHash(Encoding.UTF8.GetBytes(_text));

                return Encoding.UTF8.GetString(result);
            }
        }
    }
}