using HtmlAgilityPack;
using Newtonsoft.Json;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using yycms.entity;
using System.Text.RegularExpressions;

namespace yycms.service.PlugIn
{
    public class PageBuilder
    {
        #region 数据库
        private DBConnection _DB;
        protected DBConnection DB
        {
            get { if (_DB == null) { _DB = new DBConnection(); } return _DB; }
        }
        #endregion

        /// <summary>
        /// 根据制定模版ID生成页面
        /// </summary>
        public void Build(Int64 PageID, Int64 DataID)
        {
            Build(PageID, DataID, null);
        }

        /// <summary>
        /// 根据制定模版ID生成页面
        /// </summary>
        /// <param name="PageID">模板ID</param>
        /// <param name="DataID">数据ID（用于搭配模板列表中的SQL语句）</param>
        /// <param name="_OnComplated">每次生成完成后的事件（仅用于分页模板）,最后返回是否继续运行</param>
        public void Build(Int64 PageID, Int64 DataID, Func<Int64, Int64, Boolean> _OnComplated)
        {
            #region 生成
            String PageCode = GetPageCode(PageID);

            if (!String.IsNullOrEmpty(PageCode))
            {
                var doc = HtmlDocument(PageCode);

                var pagers = SelectNodes(doc, "//yypager");

                if (pagers != null && pagers.Count > 0)
                {
                    foreach (var p in pagers)
                    {
                        #region 必须属性检查
                        HtmlAttributeCollection Attrs = p.Attributes;
                        if (p.Attributes["total"] == null) { return; }
                        if (p.Attributes["modal"] == null) { return; }
                        #endregion

                        #region 初始化 数据总数、每页数量、总页数
                        Int64 DataCount = 0, PageSize = 0, PageCount = 0;
                        if (!Int64.TryParse(Attrs["pagesize"].Value, out PageSize)) { return; }
                        try
                        {
                            DataCount = Int64.Parse(SqlHelper.ExecuteScalar(DB.Database.Connection.ConnectionString,
                                CommandType.Text, Attrs["total"].Value).ToString());
                        }
                        catch { return; }
                        PageCount = DataCount % PageSize == 0 ? DataCount / PageSize : DataCount / PageSize + 1;
                        #endregion

                        #region 根据分页总数循环生成
                        for (Int32 i = 1; i < PageCount; i++)
                        {
                            #region View
                            String View = String.Empty;
                            if (p.Attributes["view"] != null)
                            {
                                View = doc.GetElementbyId(p.Attributes["view"].Value).InnerHtml;
                            }
                            else
                            {
                                View = p.InnerHtml;
                            }
                            #endregion

                            #region Modal
                            var Modal = new List<Dictionary<String, String>>();

                            //当前页的分页语句
                            String ModalCmd = String.Format(p.Attributes["modal"].Value, PageSize, i);

                            using (var Reader = SqlHelper.ExecuteReader(DB.Database.Connection.ConnectionString, 
                                CommandType.Text, ModalCmd))
                            {
                                var Columns = Reader.GetSchemaTable().Rows.Cast<DataRow>().Select(row => row["ColumnName"] as String).ToList();

                                while (Reader.Read())
                                {
                                    var ModalItem = new Dictionary<String, String>();

                                    for (Int32 j = 0; j < Columns.Count; j++)
                                    {
                                        ModalItem.Add(Columns[j], Reader[Columns[j]].ToString());
                                    }

                                    Modal.Add(ModalItem);
                                }
                            }
                            #endregion

                            #region Content
                            String Content = V8Build(View, new
                            {
                                Modal = Modal,
                                Global = new 
                                { 
                                    PageIndex = i, 
                                    PageCount = PageCount 
                                }
                            });
                            #endregion

                            #region Render
                            if (p.Attributes["view"] != null)
                            {
                                doc.GetElementbyId(p.Attributes["view"].Value).InnerHtml = Content;
                            }
                            else
                            {
                                p.ParentNode.InsertAfter(doc.CreateTextNode(Content), p);
                            }

                            String _PageSource = BuildPage(doc.DocumentNode.OuterHtml, DataID);
                            #endregion

                            #region 生成页面
                            SavePage(_PageSource, PageID, i, DataID);
                            doc = HtmlDocument(PageCode);
                            #endregion

                            if (_OnComplated != null)
                            {
                                if (_OnComplated.Invoke(i, PageCount) == false)
                                {
                                    break;
                                }
                            }
                            Thread.Sleep(1);
                        }
                        #endregion
                    }
                }

                #region 单页模版生成
                else
                {
                    String _PageSource = BuildPage(PageCode, DataID);
                    SavePage(_PageSource, PageID, 1, DataID);
                    if (_OnComplated != null)
                    {
                        _OnComplated.Invoke(1, 1);
                    }
                }
                #endregion
            }
            #endregion
        }

        /// <summary>
        /// 解析单个页面源码
        /// </summary>
        public String BuildPage(String PageCode, Int64 DataID)
        {
            #region 替换组件类型的yytag
            HtmlDocument doc = HtmlDocument(PageCode);
            var Tags = SelectNodes(doc, "//yytag");
            if (Tags == null) { return doc.DocumentNode.OuterHtml; }
            for (Int32 i = 0; i < Tags.Count; i++)
            {
                if (!Tags[i].HasAttributes) { continue; }

                if (Tags[i].Attributes["id"] != null)
                {
                    var ComponentID = long.Parse(Tags[i].Attributes["id"].Value);

                    Tags[i].InnerHtml = GetPageCode(ComponentID);
                }
            }
            #endregion

            PageCode = ReferenceTagCode(doc.DocumentNode.OuterHtml);
            doc = HtmlDocument(PageCode);
            Tags = SelectNodes(doc, "//yytag");
            if (Tags == null) { return doc.DocumentNode.OuterHtml; }

            for (Int32 i = 0; i < Tags.Count; i++)
            {
                if (!Tags[i].HasAttributes || Tags[i].Attributes["modal"] == null) { continue; }
                #region View
                String View = String.Empty;
                #region 如果循环的html标签不在yytag里
                if (Tags[i].Attributes["view"] != null)
                {
                    View = doc.GetElementbyId(Tags[i].Attributes["view"].Value).InnerHtml;
                }
                #endregion

                #region 如果循环的html标签在yytag里
                else
                {
                    View = Tags[i].InnerHtml;
                }
                #endregion
                #endregion

                if (!String.IsNullOrEmpty(View))
                {
                    #region Modal
                    var Modal = new List<Dictionary<String, String>>();

                    using (var Reader = SqlHelper.ExecuteReader(DB.Database.Connection.ConnectionString, CommandType.Text,
                        String.Format(Tags[i].Attributes["modal"].Value, DataID)))
                    {
                        var Columns = Reader.GetSchemaTable().Rows.Cast<DataRow>().Select(row => row["ColumnName"] as String).ToList();

                        while (Reader.Read())
                        {
                            var ModalItem = new Dictionary<String, String>();

                            for (Int32 j = 0; j < Columns.Count; j++)
                            {
                                ModalItem.Add(Columns[j], Reader[Columns[j]].ToString());
                            }

                            Modal.Add(ModalItem);
                        }
                    }
                    #endregion

                    #region Content
                    String Content = V8Build(View, new { Modal = Modal });
                    #endregion

                    #region Render
                    #region 如果模板在指定ID的元素里,就替换InnerHtml
                    if (Tags[i].Attributes["view"] != null)
                    {
                        doc.GetElementbyId(Tags[i].Attributes["view"].Value).InnerHtml = Content;
                    }
                    #endregion

                    #region 如果模板在YYTag标签里,就替换到当前YYTag后面
                    else
                    {
                        HtmlTextNode nnode = doc.CreateTextNode(Content);
                        Tags[i].ParentNode.InsertAfter(nnode, Tags[i]);
                    }
                    #endregion
                    #endregion
                }

                #region ClearTag
                Tags[i].ParentNode.RemoveChild(Tags[i]);
                #endregion
            }

            String ResultCode = ReferenceTagCode(doc.DocumentNode.OuterHtml);

            return ResultCode;
        }

        /// <summary>
        /// 输出页面
        /// </summary>
        protected void SavePage(String PageSource, Int64 PageID, Int64 PageIndex, Int64 DataID)
        {
            var doc = HtmlDocument(PageSource);

            #region 清理yytag，yypager标签
            var yytag = SelectNodes(doc, "//yytag");
            if (yytag != null)
            {
                for (Int32 i = 0; i < yytag.Count; i++)
                {
                    yytag[i].Remove();
                }
            }

            var yypager = SelectNodes(doc, "//yypager");
            if (yypager != null)
            {
                for (Int32 i = 0; i < yypager.Count; i++)
                {
                    yypager[i].Remove();
                }
            }
            #endregion

            #region 合并脚本
            /*var jsList = SelectNodes(doc, "//script");
            if (jsList != null) 
            {
                var MergeJsPath = new List<String>();
                for (Int32 i = 0; i < jsList.Count;i++ )
                {
                    if (!jsList[i].HasAttributes || jsList[i].Attributes["src"] == null)
                    {
                        #region 压缩脚本
                        jsList[i].InnerHtml = new Minifier().MinifyJavaScript(jsList[i].InnerHtml);
                        #endregion

                        if (i >= jsList.Count - 1)
                        {
                            HtmlTextNode nnode = doc.CreateTextNode("<script type=\"text/javascript\" src=\"/Res.ashx?s=" + String.Join(",", MergeJsPath.ToArray()) + "&t=text/javascript&v=1\"></script>");
                            jsList[0].ParentNode.InsertAfter(nnode, jsList[0]);
                        }
                        continue; 
                    }
                    MergeJsPath.Add("~" + jsList[i].Attributes["src"].Value);
                    
                    if (i >= jsList.Count - 1)
                    {
                        HtmlTextNode nnode = doc.CreateTextNode("<script type=\"text/javascript\" src=\"/Res.ashx?s=" + String.Join(",", MergeJsPath.ToArray()) + "&t=text/javascript&v=1\"></script>");
                        jsList[0].ParentNode.InsertAfter(nnode, jsList[0]);
                    }
                }
                for (Int32 i = 0; i < jsList.Count; i++)
                {
                    if (jsList[i].HasAttributes && jsList[i].Attributes["src"] != null)
                    {
                        jsList[i].ParentNode.RemoveChild(jsList[i]);
                    }
                }
            }*/
            #endregion

            #region 合并样式
            /*
            var styleList = SelectNodes(doc, "//link");
            if (styleList!=null) 
            {
                var MergeCSSPath = new List<String>();
                for (Int32 i = 0; i < styleList.Count; i++)
                {
                    if (!styleList[i].HasAttributes) { continue; }

                    String _Href = styleList[i].Attributes["href"].Value;

                    if (!String.IsNullOrEmpty(_Href) && Path.GetExtension(_Href).ToLower().Equals(".css"))
                    {
                        MergeCSSPath.Add("~" + styleList[i].Attributes["href"].Value);
                    }
                    if (i >= styleList.Count - 1)
                    {
                        HtmlTextNode nnode = doc.CreateTextNode("<link rel=\"stylesheet\" type=\"text/css\" href=\"/Res.ashx?s=" + String.Join(",", MergeCSSPath.ToArray()) + "&t=text/css&v=1\"/>");
                        styleList[0].ParentNode.InsertAfter(nnode, styleList[0]);
                    }
                }

                for (Int32 i = 0; i < styleList.Count; i++)
                {
                    if (!styleList[i].HasAttributes) { continue; }
                    String _Href = styleList[i].Attributes["href"].Value;
                    if (!String.IsNullOrEmpty(_Href) && Path.GetExtension(_Href).ToLower().Equals(".css"))
                    {
                        styleList[i].ParentNode.RemoveChild(styleList[i]);
                    }
                }
            }
            */
            #endregion

            #region 压缩样式
            /*
             var cssList = SelectNodes(doc, "//style");
             if (cssList != null)
             {
                 for (Int32 i = 0; i < cssList.Count; i++)
                 {
                     cssList[i].InnerHtml = new Minifier().MinifyStyleSheet(cssList[i].InnerHtml);
                 }
             }*/
            #endregion

            //重新初始化PageSource字符串
            PageSource = doc.DocumentNode.OuterHtml;

            #region 删除换行、空格
            //PageSource = Regex.Replace(PageSource, "\\n", "");
            //PageSource = Regex.Replace(PageSource, "\\r", "");
            #endregion

            //删除注释
            String SavePath = GetPageSavePath(PageID, PageIndex, DataID);

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(SavePath, false, System.Text.Encoding.UTF8))
            {
                sw.Write(PageSource);
            }

            string SaveFileName = System.IO.Path.GetFileNameWithoutExtension(SavePath).ToLower();

            if (SaveFileName.Equals("news_1"))
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(SavePath.ToLower().Replace("news_1", "news"), false, System.Text.Encoding.UTF8))
                {
                    sw.Write(PageSource);
                }
            }

            else if (SaveFileName.Equals("product_1"))
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(SavePath.ToLower().Replace("product_1", "product"), false, System.Text.Encoding.UTF8))
                {
                    sw.Write(PageSource);
                }
            }

            else if (SaveFileName.Equals("video_1"))
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(SavePath.ToLower().Replace("video_1", "video"), false, System.Text.Encoding.UTF8))
                {
                    sw.Write(PageSource);
                }
            }

            else if (SaveFileName.Equals("photo_1"))
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(SavePath.ToLower().Replace("photo_1", "photo"), false, System.Text.Encoding.UTF8))
                {
                    sw.Write(PageSource);
                }
            }
        }
        /// <summary>
        /// 获取模板的保存路径及名称，如果保存路径不存在就创建文件夹，最终确保返回页面的保存路径
        /// </summary>
        protected String GetPageSavePath(Int64 PageID, Int64 PageIndex, Int64 DataID)
        {
            String SavePath = String.Empty, ExtensionName = String.Empty, CreateDate = String.Empty;

                var PageInfo = (from a in DB.yy_Page
                                where a.ID == PageID
                                select new
                                {
                                    SavePath = a.SavePath,
                                    ExtensionName = a.ExtensionName,
                                    CreateDate = a.CreateDate
                                }).FirstOrDefault();
                if (PageInfo != null)
                {
                    SavePath = PageInfo.SavePath;
                    CreateDate = PageInfo.CreateDate.ToString("yyyyMMddHHmmss");
                    ExtensionName = PageInfo.ExtensionName;
                }

            if (String.IsNullOrEmpty(SavePath)) { SavePath = "/"; }
            else if (SavePath.IndexOf("~") == 0) { SavePath = SavePath.Substring(1); }

            if (String.IsNullOrEmpty(ExtensionName)) { ExtensionName = "{0}.html"; }
            if (String.IsNullOrEmpty(CreateDate)) { CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss"); }

            String ServerSavePath = String.Empty;
            try
            {
                String PathService = ConfigurationManager.AppSettings["SkinBaseDirectory"];

                ServerSavePath = PathService + SavePath;
            }
            catch { return null; }

            if (!Directory.Exists(ServerSavePath)) { Directory.CreateDirectory(ServerSavePath); }
            String SaveFilePath = String.Format(ExtensionName, PageID, CreateDate, PageIndex, DataID);
            if (String.IsNullOrEmpty(Path.GetExtension(SaveFilePath))) { SavePath += ".html"; }
            return ServerSavePath + "/" + SaveFilePath;
        }

        /// <summary>
        /// 获取制定模版的代码
        /// </summary>
        protected String GetPageCode(Int64 pageID)
        {
            return
                SqlHelper.ExecuteScalar(DB.Database.Connection.ConnectionString, CommandType.Text,
                "SELECT PageCode FROM yy_Page WITH(NOLOCK) WHERE ID=" + pageID).ToString();
        }
        /// <summary>
        /// 根据html代码返回Html文档对象
        /// </summary>
        protected HtmlDocument HtmlDocument(String htmlCode)
        {
            HtmlDocument doc = new HtmlDocument();

            doc.LoadHtml(htmlCode);

            return doc;
        }
        /// <summary>
        /// 查找指定元素
        /// </summary>
        protected HtmlNodeCollection SelectNodes(HtmlDocument doc, String xpath)
        {
            return doc.DocumentNode.SelectNodes(xpath);
        }

        /// <summary>
        /// 递归替换所有引用的YYTag，获取他们对应的代码，到当前文档中
        /// </summary>
        protected String ReferenceTagCode(String PageCode)
        {
            HtmlDocument doc = this.HtmlDocument(PageCode);
            HtmlNodeCollection Tags = this.SelectNodes(doc, "//yytag");

            bool arg_4D_0;

            if (Tags != null)
            {
                arg_4D_0 = ((
                    from x in Tags
                    where x.Attributes["id"] != null
                    select x).Count<HtmlNode>() >= 1);
            }
            else
            {
                arg_4D_0 = false;
            }
            string result;
            if (!arg_4D_0)
            {
                result = PageCode;
            }
            else
            {
                for (int i = 0; i < Tags.Count; i++)
                {
                    if (Tags[i].HasAttributes)
                    {
                        if (Tags[i].Attributes["id"] != null)
                        {
                            int TagId = 0;
                            if (int.TryParse(Tags[i].Attributes["id"].Value, out TagId))
                            {
                                string pageCode = this.GetPageCode((long)TagId);
                                HtmlTextNode nnode = doc.CreateTextNode(pageCode);
                                Tags[i].ParentNode.InsertAfter(nnode, Tags[i]);
                                Tags[i].ParentNode.RemoveChild(Tags[i]);
                            }
                        }
                    }
                }
                result = this.ReferenceTagCode(doc.DocumentNode.OuterHtml);
            }
            return result;
        }

        #region V8Build JS模板编译
        public static String Juicer = Encoding.UTF8.GetString(
            Convert.FromBase64String("IChmdW5jdGlvbiAoKSB7IHZhciBlID0gZnVuY3Rpb24gKCkgeyB2YXIgdCA9IFtdLnNsaWNlLmNhbGwoYXJndW1lbnRzKTsgdC5wdXNoKGUub3B0aW9ucyksIHRbMF0ubWF0Y2goL15ccyojKFtcdzpcLVwuXSspXHMqJC9pZ20pICYmIHRbMF0ucmVwbGFjZSgvXlxzKiMoW1x3OlwtXC5dKylccyokL2lnbSwgZnVuY3Rpb24gKGUsIG4pIHsgdmFyIHIgPSBkb2N1bWVudCwgaSA9IHIgJiYgci5nZXRFbGVtZW50QnlJZChuKTsgdFswXSA9IGkgPyBpLnZhbHVlIHx8IGkuaW5uZXJIVE1MIDogZSB9KTsgaWYgKGFyZ3VtZW50cy5sZW5ndGggPT0gMSkgcmV0dXJuIGUuY29tcGlsZS5hcHBseShlLCB0KTsgaWYgKGFyZ3VtZW50cy5sZW5ndGggPj0gMikgcmV0dXJuIGUudG9faHRtbC5hcHBseShlLCB0KSB9LCB0ID0geyBlc2NhcGVoYXNoOiB7ICI8IjogIiZsdDsiLCAiPiI6ICImZ3Q7IiwgIiYiOiAiJmFtcDsiLCAnIic6ICImcXVvdDsiLCAiJyI6ICImI3gyNzsiLCAiLyI6ICImI3gyZjsiIH0sIGVzY2FwZXJlcGxhY2U6IGZ1bmN0aW9uIChlKSB7IHJldHVybiB0LmVzY2FwZWhhc2hbZV0gfSwgZXNjYXBpbmc6IGZ1bmN0aW9uIChlKSB7IHJldHVybiB0eXBlb2YgZSAhPSAic3RyaW5nIiA/IGUgOiBlLnJlcGxhY2UoL1smPD4iXS9pZ20sIHRoaXMuZXNjYXBlcmVwbGFjZSkgfSwgZGV0ZWN0aW9uOiBmdW5jdGlvbiAoZSkgeyByZXR1cm4gdHlwZW9mIGUgPT0gInVuZGVmaW5lZCIgPyAiIiA6IGUgfSB9LCBuID0gZnVuY3Rpb24gKGUpIHsgaWYgKHR5cGVvZiBjb25zb2xlICE9ICJ1bmRlZmluZWQiKSB7IGlmIChjb25zb2xlLndhcm4pIHsgY29uc29sZS53YXJuKGUpOyByZXR1cm4gfSBpZiAoY29uc29sZS5sb2cpIHsgY29uc29sZS5sb2coZSk7IHJldHVybiB9IH0gdGhyb3cgZSB9LCByID0gZnVuY3Rpb24gKGUsIHQpIHsgZSA9IGUgIT09IE9iamVjdChlKSA/IHt9IDogZTsgaWYgKGUuX19wcm90b19fKSByZXR1cm4gZS5fX3Byb3RvX18gPSB0LCBlOyB2YXIgbiA9IGZ1bmN0aW9uICgpIHsgfSwgciA9IE9iamVjdC5jcmVhdGUgPyBPYmplY3QuY3JlYXRlKHQpIDogbmV3IChuLnByb3RvdHlwZSA9IHQsIG4pOyBmb3IgKHZhciBpIGluIGUpIGUuaGFzT3duUHJvcGVydHkoaSkgJiYgKHJbaV0gPSBlW2ldKTsgcmV0dXJuIHIgfTsgZS5fX2NhY2hlID0ge30sIGUudmVyc2lvbiA9ICIwLjYuNC1zdGFibGUiLCBlLnNldHRpbmdzID0ge30sIGUudGFncyA9IHsgb3BlcmF0aW9uT3BlbjogIntAIiwgb3BlcmF0aW9uQ2xvc2U6ICJ9IiwgaW50ZXJwb2xhdGVPcGVuOiAiXFwkeyIsIGludGVycG9sYXRlQ2xvc2U6ICJ9Iiwgbm9uZWVuY29kZU9wZW46ICJcXCRcXCR7Iiwgbm9uZWVuY29kZUNsb3NlOiAifSIsIGNvbW1lbnRPcGVuOiAiXFx7IyIsIGNvbW1lbnRDbG9zZTogIlxcfSIgfSwgZS5vcHRpb25zID0geyBjYWNoZTogITAsIHN0cmlwOiAhMCwgZXJyb3JoYW5kbGluZzogITAsIGRldGVjdGlvbjogITAsIF9tZXRob2Q6IHIoeyBfX2VzY2FwZWh0bWw6IHQsIF9fdGhyb3c6IG4sIF9fanVpY2VyOiBlIH0sIHt9KSB9LCBlLnRhZ0luaXQgPSBmdW5jdGlvbiAoKSB7IHZhciB0ID0gZS50YWdzLm9wZXJhdGlvbk9wZW4gKyAiZWFjaFxccyooW159XSo/KVxccyphc1xccyooXFx3Kj8pXFxzKigsXFxzKlxcdyo/KT8iICsgZS50YWdzLm9wZXJhdGlvbkNsb3NlLCBuID0gZS50YWdzLm9wZXJhdGlvbk9wZW4gKyAiXFwvZWFjaCIgKyBlLnRhZ3Mub3BlcmF0aW9uQ2xvc2UsIHIgPSBlLnRhZ3Mub3BlcmF0aW9uT3BlbiArICJpZlxccyooW159XSo/KSIgKyBlLnRhZ3Mub3BlcmF0aW9uQ2xvc2UsIGkgPSBlLnRhZ3Mub3BlcmF0aW9uT3BlbiArICJcXC9pZiIgKyBlLnRhZ3Mub3BlcmF0aW9uQ2xvc2UsIHMgPSBlLnRhZ3Mub3BlcmF0aW9uT3BlbiArICJlbHNlIiArIGUudGFncy5vcGVyYXRpb25DbG9zZSwgbyA9IGUudGFncy5vcGVyYXRpb25PcGVuICsgImVsc2UgaWZcXHMqKFtefV0qPykiICsgZS50YWdzLm9wZXJhdGlvbkNsb3NlLCB1ID0gZS50YWdzLmludGVycG9sYXRlT3BlbiArICIoW1xcc1xcU10rPykiICsgZS50YWdzLmludGVycG9sYXRlQ2xvc2UsIGEgPSBlLnRhZ3Mubm9uZWVuY29kZU9wZW4gKyAiKFtcXHNcXFNdKz8pIiArIGUudGFncy5ub25lZW5jb2RlQ2xvc2UsIGYgPSBlLnRhZ3MuY29tbWVudE9wZW4gKyAiW159XSo/IiArIGUudGFncy5jb21tZW50Q2xvc2UsIGwgPSBlLnRhZ3Mub3BlcmF0aW9uT3BlbiArICJlYWNoXFxzKihcXHcqPylcXHMqaW5cXHMqcmFuZ2VcXCgoW159XSs/KVxccyosXFxzKihbXn1dKz8pXFwpIiArIGUudGFncy5vcGVyYXRpb25DbG9zZSwgYyA9IGUudGFncy5vcGVyYXRpb25PcGVuICsgImluY2x1ZGVcXHMqKFtefV0qPylcXHMqLFxccyooW159XSo/KSIgKyBlLnRhZ3Mub3BlcmF0aW9uQ2xvc2U7IGUuc2V0dGluZ3MuZm9yc3RhcnQgPSBuZXcgUmVnRXhwKHQsICJpZ20iKSwgZS5zZXR0aW5ncy5mb3JlbmQgPSBuZXcgUmVnRXhwKG4sICJpZ20iKSwgZS5zZXR0aW5ncy5pZnN0YXJ0ID0gbmV3IFJlZ0V4cChyLCAiaWdtIiksIGUuc2V0dGluZ3MuaWZlbmQgPSBuZXcgUmVnRXhwKGksICJpZ20iKSwgZS5zZXR0aW5ncy5lbHNlc3RhcnQgPSBuZXcgUmVnRXhwKHMsICJpZ20iKSwgZS5zZXR0aW5ncy5lbHNlaWZzdGFydCA9IG5ldyBSZWdFeHAobywgImlnbSIpLCBlLnNldHRpbmdzLmludGVycG9sYXRlID0gbmV3IFJlZ0V4cCh1LCAiaWdtIiksIGUuc2V0dGluZ3Mubm9uZWVuY29kZSA9IG5ldyBSZWdFeHAoYSwgImlnbSIpLCBlLnNldHRpbmdzLmlubGluZWNvbW1lbnQgPSBuZXcgUmVnRXhwKGYsICJpZ20iKSwgZS5zZXR0aW5ncy5yYW5nZXN0YXJ0ID0gbmV3IFJlZ0V4cChsLCAiaWdtIiksIGUuc2V0dGluZ3MuaW5jbHVkZSA9IG5ldyBSZWdFeHAoYywgImlnbSIpIH0sIGUudGFnSW5pdCgpLCBlLnNldCA9IGZ1bmN0aW9uIChlLCB0KSB7IHZhciBuID0gdGhpcywgciA9IGZ1bmN0aW9uIChlKSB7IHJldHVybiBlLnJlcGxhY2UoL1tcJFwoXClcW1xdXCtcXlx7XH1cP1wqXHxcLl0vaWdtLCBmdW5jdGlvbiAoZSkgeyByZXR1cm4gIlxcIiArIGUgfSkgfSwgaSA9IGZ1bmN0aW9uIChlLCB0KSB7IHZhciBpID0gZS5tYXRjaCgvXnRhZzo6KC4qKSQvaSk7IGlmIChpKSB7IG4udGFnc1tpWzFdXSA9IHIodCksIG4udGFnSW5pdCgpOyByZXR1cm4gfSBuLm9wdGlvbnNbZV0gPSB0IH07IGlmIChhcmd1bWVudHMubGVuZ3RoID09PSAyKSB7IGkoZSwgdCk7IHJldHVybiB9IGlmIChlID09PSBPYmplY3QoZSkpIGZvciAodmFyIHMgaW4gZSkgZS5oYXNPd25Qcm9wZXJ0eShzKSAmJiBpKHMsIGVbc10pIH0sIGUucmVnaXN0ZXIgPSBmdW5jdGlvbiAoZSwgdCkgeyB2YXIgbiA9IHRoaXMub3B0aW9ucy5fbWV0aG9kOyByZXR1cm4gbi5oYXNPd25Qcm9wZXJ0eShlKSA/ICExIDogbltlXSA9IHQgfSwgZS51bnJlZ2lzdGVyID0gZnVuY3Rpb24gKGUpIHsgdmFyIHQgPSB0aGlzLm9wdGlvbnMuX21ldGhvZDsgaWYgKHQuaGFzT3duUHJvcGVydHkoZSkpIHJldHVybiBkZWxldGUgdFtlXSB9LCBlLnRlbXBsYXRlID0gZnVuY3Rpb24gKHQpIHsgdmFyIG4gPSB0aGlzOyB0aGlzLm9wdGlvbnMgPSB0LCB0aGlzLl9faW50ZXJwb2xhdGUgPSBmdW5jdGlvbiAoZSwgdCwgbikgeyB2YXIgciA9IGUuc3BsaXQoInwiKSwgaSA9IHJbMF0gfHwgIiIsIHM7IHJldHVybiByLmxlbmd0aCA+IDEgJiYgKGUgPSByLnNoaWZ0KCksIHMgPSByLnNoaWZ0KCkuc3BsaXQoIiwiKSwgaSA9ICJfbWV0aG9kLiIgKyBzLnNoaWZ0KCkgKyAiLmNhbGwoe30sICIgKyBbZV0uY29uY2F0KHMpICsgIikiKSwgIjwlPSAiICsgKHQgPyAiX21ldGhvZC5fX2VzY2FwZWh0bWwuZXNjYXBpbmciIDogIiIpICsgIigiICsgKCFuIHx8IG4uZGV0ZWN0aW9uICE9PSAhMSA/ICJfbWV0aG9kLl9fZXNjYXBlaHRtbC5kZXRlY3Rpb24iIDogIiIpICsgIigiICsgaSArICIpIiArICIpIiArICIgJT4iIH0sIHRoaXMuX19yZW1vdmVTaGVsbCA9IGZ1bmN0aW9uICh0LCByKSB7IHZhciBpID0gMDsgdCA9IHQucmVwbGFjZShlLnNldHRpbmdzLmZvcnN0YXJ0LCBmdW5jdGlvbiAoZSwgdCwgbiwgcikgeyB2YXIgbiA9IG4gfHwgInZhbHVlIiwgciA9IHIgJiYgci5zdWJzdHIoMSksIHMgPSAiaSIgKyBpKys7IHJldHVybiAiPCUgfmZ1bmN0aW9uKCkge2Zvcih2YXIgIiArIHMgKyAiIGluICIgKyB0ICsgIikgeyIgKyAiaWYoIiArIHQgKyAiLmhhc093blByb3BlcnR5KCIgKyBzICsgIikpIHsiICsgInZhciAiICsgbiArICI9IiArIHQgKyAiWyIgKyBzICsgIl07IiArIChyID8gInZhciAiICsgciArICI9IiArIHMgKyAiOyIgOiAiIikgKyAiICU+IiB9KS5yZXBsYWNlKGUuc2V0dGluZ3MuZm9yZW5kLCAiPCUgfX19KCk7ICU+IikucmVwbGFjZShlLnNldHRpbmdzLmlmc3RhcnQsIGZ1bmN0aW9uIChlLCB0KSB7IHJldHVybiAiPCUgaWYoIiArIHQgKyAiKSB7ICU+IiB9KS5yZXBsYWNlKGUuc2V0dGluZ3MuaWZlbmQsICI8JSB9ICU+IikucmVwbGFjZShlLnNldHRpbmdzLmVsc2VzdGFydCwgZnVuY3Rpb24gKGUpIHsgcmV0dXJuICI8JSB9IGVsc2UgeyAlPiIgfSkucmVwbGFjZShlLnNldHRpbmdzLmVsc2VpZnN0YXJ0LCBmdW5jdGlvbiAoZSwgdCkgeyByZXR1cm4gIjwlIH0gZWxzZSBpZigiICsgdCArICIpIHsgJT4iIH0pLnJlcGxhY2UoZS5zZXR0aW5ncy5ub25lZW5jb2RlLCBmdW5jdGlvbiAoZSwgdCkgeyByZXR1cm4gbi5fX2ludGVycG9sYXRlKHQsICExLCByKSB9KS5yZXBsYWNlKGUuc2V0dGluZ3MuaW50ZXJwb2xhdGUsIGZ1bmN0aW9uIChlLCB0KSB7IHJldHVybiBuLl9faW50ZXJwb2xhdGUodCwgITAsIHIpIH0pLnJlcGxhY2UoZS5zZXR0aW5ncy5pbmxpbmVjb21tZW50LCAiIikucmVwbGFjZShlLnNldHRpbmdzLnJhbmdlc3RhcnQsIGZ1bmN0aW9uIChlLCB0LCBuLCByKSB7IHZhciBzID0gImoiICsgaSsrOyByZXR1cm4gIjwlIH5mdW5jdGlvbigpIHtmb3IodmFyICIgKyBzICsgIj0iICsgbiArICI7IiArIHMgKyAiPCIgKyByICsgIjsiICsgcyArICIrKykge3siICsgInZhciAiICsgdCArICI9IiArIHMgKyAiOyIgKyAiICU+IiB9KS5yZXBsYWNlKGUuc2V0dGluZ3MuaW5jbHVkZSwgZnVuY3Rpb24gKGUsIHQsIG4pIHsgcmV0dXJuICI8JT0gX21ldGhvZC5fX2p1aWNlcigiICsgdCArICIsICIgKyBuICsgIik7ICU+IiB9KTsgaWYgKCFyIHx8IHIuZXJyb3JoYW5kbGluZyAhPT0gITEpIHQgPSAiPCUgdHJ5IHsgJT4iICsgdCwgdCArPSAnPCUgfSBjYXRjaChlKSB7X21ldGhvZC5fX3Rocm93KCJKdWljZXIgUmVuZGVyIEV4Y2VwdGlvbjogIitlLm1lc3NhZ2UpO30gJT4nOyByZXR1cm4gdCB9LCB0aGlzLl9fdG9OYXRpdmUgPSBmdW5jdGlvbiAoZSwgdCkgeyByZXR1cm4gdGhpcy5fX2NvbnZlcnQoZSwgIXQgfHwgdC5zdHJpcCkgfSwgdGhpcy5fX2xleGljYWxBbmFseXplID0gZnVuY3Rpb24gKHQpIHsgdmFyIG4gPSBbXSwgciA9IFtdLCBpID0gIiIsIHMgPSBbImlmIiwgImVhY2giLCAiXyIsICJfbWV0aG9kIiwgImNvbnNvbGUiLCAiYnJlYWsiLCAiY2FzZSIsICJjYXRjaCIsICJjb250aW51ZSIsICJkZWJ1Z2dlciIsICJkZWZhdWx0IiwgImRlbGV0ZSIsICJkbyIsICJmaW5hbGx5IiwgImZvciIsICJmdW5jdGlvbiIsICJpbiIsICJpbnN0YW5jZW9mIiwgIm5ldyIsICJyZXR1cm4iLCAic3dpdGNoIiwgInRoaXMiLCAidGhyb3ciLCAidHJ5IiwgInR5cGVvZiIsICJ2YXIiLCAidm9pZCIsICJ3aGlsZSIsICJ3aXRoIiwgIm51bGwiLCAidHlwZW9mIiwgImNsYXNzIiwgImVudW0iLCAiZXhwb3J0IiwgImV4dGVuZHMiLCAiaW1wb3J0IiwgInN1cGVyIiwgImltcGxlbWVudHMiLCAiaW50ZXJmYWNlIiwgImxldCIsICJwYWNrYWdlIiwgInByaXZhdGUiLCAicHJvdGVjdGVkIiwgInB1YmxpYyIsICJzdGF0aWMiLCAieWllbGQiLCAiY29uc3QiLCAiYXJndW1lbnRzIiwgInRydWUiLCAiZmFsc2UiLCAidW5kZWZpbmVkIiwgIk5hTiJdLCBvID0gZnVuY3Rpb24gKGUsIHQpIHsgaWYgKEFycmF5LnByb3RvdHlwZS5pbmRleE9mICYmIGUuaW5kZXhPZiA9PT0gQXJyYXkucHJvdG90eXBlLmluZGV4T2YpIHJldHVybiBlLmluZGV4T2YodCk7IGZvciAodmFyIG4gPSAwOyBuIDwgZS5sZW5ndGg7IG4rKykgaWYgKGVbbl0gPT09IHQpIHJldHVybiBuOyByZXR1cm4gLTEgfSwgdSA9IGZ1bmN0aW9uICh0LCBpKSB7IGkgPSBpLm1hdGNoKC9cdysvaWdtKVswXTsgaWYgKG8obiwgaSkgPT09IC0xICYmIG8ocywgaSkgPT09IC0xICYmIG8ociwgaSkgPT09IC0xKSB7IGlmICh0eXBlb2Ygd2luZG93ICE9ICJ1bmRlZmluZWQiICYmIHR5cGVvZiB3aW5kb3dbaV0gPT0gImZ1bmN0aW9uIiAmJiB3aW5kb3dbaV0udG9TdHJpbmcoKS5tYXRjaCgvXlxzKj9mdW5jdGlvbiBcdytcKFwpIFx7XHMqP1xbbmF0aXZlIGNvZGVcXVxzKj9cfVxzKj8kL2kpKSByZXR1cm4gdDsgaWYgKHR5cGVvZiBnbG9iYWwgIT0gInVuZGVmaW5lZCIgJiYgdHlwZW9mIGdsb2JhbFtpXSA9PSAiZnVuY3Rpb24iICYmIGdsb2JhbFtpXS50b1N0cmluZygpLm1hdGNoKC9eXHMqP2Z1bmN0aW9uIFx3K1woXCkgXHtccyo/XFtuYXRpdmUgY29kZVxdXHMqP1x9XHMqPyQvaSkpIHJldHVybiB0OyBpZiAodHlwZW9mIGUub3B0aW9ucy5fbWV0aG9kW2ldID09ICJmdW5jdGlvbiIpIHJldHVybiByLnB1c2goaSksIHQ7IG4ucHVzaChpKSB9IHJldHVybiB0IH07IHQucmVwbGFjZShlLnNldHRpbmdzLmZvcnN0YXJ0LCB1KS5yZXBsYWNlKGUuc2V0dGluZ3MuaW50ZXJwb2xhdGUsIHUpLnJlcGxhY2UoZS5zZXR0aW5ncy5pZnN0YXJ0LCB1KS5yZXBsYWNlKGUuc2V0dGluZ3MuZWxzZWlmc3RhcnQsIHUpLnJlcGxhY2UoZS5zZXR0aW5ncy5pbmNsdWRlLCB1KS5yZXBsYWNlKC9bXCtcLVwqXC8lIVw/XHxcXiZ+PD49LFwoXCldXHMqKFtBLVphLXpfXSspL2lnbSwgdSk7IGZvciAodmFyIGEgPSAwOyBhIDwgbi5sZW5ndGg7IGErKykgaSArPSAidmFyICIgKyBuW2FdICsgIj1fLiIgKyBuW2FdICsgIjsiOyBmb3IgKHZhciBhID0gMDsgYSA8IHIubGVuZ3RoOyBhKyspIGkgKz0gInZhciAiICsgclthXSArICI9X21ldGhvZC4iICsgclthXSArICI7IjsgcmV0dXJuICI8JSAiICsgaSArICIgJT4iIH0sIHRoaXMuX19jb252ZXJ0ID0gZnVuY3Rpb24gKGUsIHQpIHsgdmFyIG4gPSBbXS5qb2luKCIiKTsgcmV0dXJuIG4gKz0gIid1c2Ugc3RyaWN0JzsiLCBuICs9ICJ2YXIgXz1ffHx7fTsiLCBuICs9ICJ2YXIgX291dD0nJztfb3V0Kz0nIiwgdCAhPT0gITEgPyAobiArPSBlLnJlcGxhY2UoL1xcL2csICJcXFxcIikucmVwbGFjZSgvW1xyXHRcbl0vZywgIiAiKS5yZXBsYWNlKC8nKD89W14lXSolPikvZywgIgkiKS5zcGxpdCgiJyIpLmpvaW4oIlxcJyIpLnNwbGl0KCIJIikuam9pbigiJyIpLnJlcGxhY2UoLzwlPSguKz8pJT4vZywgIic7X291dCs9JDE7X291dCs9JyIpLnNwbGl0KCI8JSIpLmpvaW4oIic7Iikuc3BsaXQoIiU+Iikuam9pbigiX291dCs9JyIpICsgIic7cmV0dXJuIF9vdXQ7IiwgbikgOiAobiArPSBlLnJlcGxhY2UoL1xcL2csICJcXFxcIikucmVwbGFjZSgvW1xyXS9nLCAiXFxyIikucmVwbGFjZSgvW1x0XS9nLCAiXFx0IikucmVwbGFjZSgvW1xuXS9nLCAiXFxuIikucmVwbGFjZSgvJyg/PVteJV0qJT4pL2csICIJIikuc3BsaXQoIiciKS5qb2luKCJcXCciKS5zcGxpdCgiCSIpLmpvaW4oIiciKS5yZXBsYWNlKC88JT0oLis/KSU+L2csICInO19vdXQrPSQxO19vdXQrPSciKS5zcGxpdCgiPCUiKS5qb2luKCInOyIpLnNwbGl0KCIlPiIpLmpvaW4oIl9vdXQrPSciKSArICInO3JldHVybiBfb3V0LnJlcGxhY2UoL1tcXHJcXG5dXFxzK1tcXHJcXG5dL2csICdcXHJcXG4nKTsiLCBuKSB9LCB0aGlzLnBhcnNlID0gZnVuY3Rpb24gKGUsIHQpIHsgdmFyIGkgPSB0aGlzOyBpZiAoIXQgfHwgdC5sb29zZSAhPT0gITEpIGUgPSB0aGlzLl9fbGV4aWNhbEFuYWx5emUoZSkgKyBlOyByZXR1cm4gZSA9IHRoaXMuX19yZW1vdmVTaGVsbChlLCB0KSwgZSA9IHRoaXMuX190b05hdGl2ZShlLCB0KSwgdGhpcy5fcmVuZGVyID0gbmV3IEZ1bmN0aW9uKCJfLCBfbWV0aG9kIiwgZSksIHRoaXMucmVuZGVyID0gZnVuY3Rpb24gKGUsIHQpIHsgaWYgKCF0IHx8IHQgIT09IG4ub3B0aW9ucy5fbWV0aG9kKSB0ID0gcih0LCBuLm9wdGlvbnMuX21ldGhvZCk7IHJldHVybiBpLl9yZW5kZXIuY2FsbCh0aGlzLCBlLCB0KSB9LCB0aGlzIH0gfSwgZS5jb21waWxlID0gZnVuY3Rpb24gKGUsIHQpIHsgaWYgKCF0IHx8IHQgIT09IHRoaXMub3B0aW9ucykgdCA9IHIodCwgdGhpcy5vcHRpb25zKTsgdHJ5IHsgdmFyIGkgPSB0aGlzLl9fY2FjaGVbZV0gPyB0aGlzLl9fY2FjaGVbZV0gOiAobmV3IHRoaXMudGVtcGxhdGUodGhpcy5vcHRpb25zKSkucGFyc2UoZSwgdCk7IGlmICghdCB8fCB0LmNhY2hlICE9PSAhMSkgdGhpcy5fX2NhY2hlW2VdID0gaTsgcmV0dXJuIGkgfSBjYXRjaCAocykgeyByZXR1cm4gbigiSnVpY2VyIENvbXBpbGUgRXhjZXB0aW9uOiAiICsgcy5tZXNzYWdlKSwgeyByZW5kZXI6IGZ1bmN0aW9uICgpIHsgfSB9IH0gfSwgZS50b19odG1sID0gZnVuY3Rpb24gKGUsIHQsIG4pIHsgaWYgKCFuIHx8IG4gIT09IHRoaXMub3B0aW9ucykgbiA9IHIobiwgdGhpcy5vcHRpb25zKTsgcmV0dXJuIHRoaXMuY29tcGlsZShlLCBuKS5yZW5kZXIodCwgbi5fbWV0aG9kKSB9LCB0eXBlb2YgbW9kdWxlICE9ICJ1bmRlZmluZWQiICYmIG1vZHVsZS5leHBvcnRzID8gbW9kdWxlLmV4cG9ydHMgPSBlIDogdGhpcy5qdWljZXIgPSBlIH0pKCk7IA=="));

        public String V8Build(String view, object modal)
        {
            String result = String.Empty;

            using (var engine = new V8ScriptEngine())
            {
                engine.Execute(Juicer);

                engine.Script.view = view;

                var modal_script = "modal = " + JsonConvert.SerializeObject(modal);
                
                engine.Execute(modal_script);

                result = engine.Invoke("juicer", engine.Script.view, engine.Script.modal);
            }

            return result.ToString();
        }
        #endregion
    }
}