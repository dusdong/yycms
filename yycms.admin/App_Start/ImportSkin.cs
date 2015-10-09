using HtmlAgilityPack;
using Ionic.Zip;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using yycms.entity;

namespace yycms.admin
{
    /// <summary>
    /// 导入风格包
    /// </summary>
    public class ImportSkin
    {
        /// <summary>
        /// 模板的保存根目录，绝对路径，结尾加上\
        /// </summary>
        public String SkinBaseDirectory = ConfigurationManager.AppSettings["SkinBaseDirectory"];

        #region 数据库
        private DBConnection _DB;
        protected DBConnection DB
        {
            get { if (_DB == null) { _DB = new DBConnection(); } return _DB; }
        }
        #endregion

        public ImportSkin()
        {
        }

        /// <summary>
        /// 导入模板
        /// </summary>
        /// <param name="FileData">模板包</param>
        /// <param name="UserID">当前上传者的ID</param>
        /// <returns></returns>
        public String Import(Byte[] FileData,long UserID)
        {
            SkinConfig SConfig = null;

            var SkinDirectoryName = String.Empty;

            using (var SkinPackage = ZipFile.Read(new MemoryStream(FileData)))
            {
                #region 获取Config对象
                var ConfigEntry = SkinPackage.Entries.Where(a => a.FileName.ToLower().Equals("config.js")).FirstOrDefault();
                if (ConfigEntry == null)
                {
                    return "未找到config.js文件。";
                }
                #endregion

                MemoryStream ms = new MemoryStream();
                ConfigEntry.Extract(ms);
                String ConfigContent = Encoding.Default.GetString(ms.ToArray());
                SConfig = JsonConvert.DeserializeObject<SkinConfig>(ConfigContent);

                #region 序列化是否成功
                if (SConfig == null)
                {
                    return "配置文件反序列化错误。";
                }
                #endregion

                #region 模板中文名称不得为空
                if (String.IsNullOrEmpty(SConfig.Name))
                {
                    return "当前风格的名称不得为空。";
                }
                #endregion

                #region 验证模板英文名称

                if (Regex.IsMatch(SConfig.Name_En, "^[A-Za-z0-9]{1,50}$") == false)
                {
                    return "模板Name_EN应为字母或数字，长度在1~50位内。";
                }
                #endregion

                #region 检测版本号
                if (SConfig.Version < 1)
                {
                    return "版本号不得小于1。";
                }
                #endregion

                SkinDirectoryName = SkinBaseDirectory + SConfig.Name_En + SConfig.Version.ToString();

                #region 是否已经存在同名目录（模板+版本）
                if (Directory.Exists(SkinDirectoryName))
                {
                    return "模板已存在。";
                }
                #endregion

                #region 检测图片/模板数量,不得小于1
                if (SConfig.Images == null || SConfig.Images.Count < 1)
                {
                    return "至少需要上传一张截图。";
                }
                if (SConfig.Pages == null || SConfig.Pages.Count < 1)
                {
                    return "至少需要上传1个模板。";
                }
                #endregion

                SkinPackage.ExtractAll(SkinDirectoryName, ExtractExistingFileAction.OverwriteSilently);

                #region 检测图片路径
                if (SConfig.Images != null && SConfig.Images.Count > 0)
                {
                    String[] AllowTypes = new String[] { ".jpg", ".png", "jpeg" };

                    foreach (var v in SConfig.Images)
                    {
                        if (!File.Exists(SkinDirectoryName + "/" + v))
                        {
                            Directory.Delete(SkinDirectoryName, true);
                            return v + "图片不存在目录中。";
                        }

                        if (!AllowTypes.Contains(Path.GetExtension(v).ToLower()))
                        {
                            Directory.Delete(SkinDirectoryName, true);
                            return "模板截图只能是jpg/png/jpeg格式文件。";
                        }

                        Thread.Sleep(1);
                    }
                }
                #endregion

                #region 检测模板源文件路径,如果没问题就读取
                if (SConfig.Pages != null && SConfig.Pages.Count > 0)
                {
                    foreach (var v in SConfig.Pages)
                    {
                        /*
                         * 源码路径读取的方式:
                         * 优先级1：根据Source节点读取源码
                         * 如果不存在
                         * 优先级2：根据SaveName姐弟读取源码
                         */

                        String SourcePath = SkinDirectoryName + "/" + v.Source;

                        //如果为空，则尝试取保存文件名作为源代码文件的存放路径
                        if (String.IsNullOrEmpty(v.Source)) { SourcePath = SkinDirectoryName + "/" + v.SaveName; }

                        if (!File.Exists(SourcePath))
                        {
                            Directory.Delete(SkinDirectoryName, true);
                            return "模板[" + v.Title + "]路径配置不正确。";
                        }

                        using (StreamReader sr = new StreamReader(SourcePath, Encoding.UTF8))
                        {
                            v.Source = sr.ReadToEnd();
                        }

                        File.Delete(SourcePath);

                        Thread.Sleep(1);
                    }
                }
                #endregion
            }

            #region 当前新的模板
            var SkinType = new yy_Page_Type()
            {
                Version = SConfig.Version,
                Name = SConfig.Name.Trim(),
                Name_En = SConfig.Name_En.Trim(),
                Images = String.Join(",", SConfig.Images),
                CreateDate = DateTime.Now,
                IsMaster = 0,
                Email = String.IsNullOrEmpty(SConfig.Email) ? "" : SConfig.Email,
                Mobile = String.IsNullOrEmpty(SConfig.Mobile) ? "" : SConfig.Mobile,
                QQ = String.IsNullOrEmpty(SConfig.QQ) ? "" : SConfig.QQ,
                Summary = String.IsNullOrEmpty(SConfig.Summary) ? "" : SConfig.Summary,
                SupportPlatform = SConfig.SupportPlatform,
                UserID = UserID,
                Website = String.IsNullOrEmpty(SConfig.Website) ? "" : SConfig.Website,
                Author = String.IsNullOrEmpty(SConfig.Author) ? "" : SConfig.Author,
            };
            #endregion

            #region 如果不存在同英文名，版本号的记录，就添加模板
            Boolean HasSkin = false;
                if (DB.yy_Page_Type.Where(a => a.Name_En == SkinType.Name_En && a.Version == SkinType.Version).FirstOrDefault() == null)
                {
                    DB.yy_Page_Type.Add(SkinType);
                    DB.SaveChanges();
                }
                else { HasSkin = true; }

            if (HasSkin)
            {
                return "网站已经有这个风格了。";
            }
            #endregion

            String SkinPath = SkinBaseDirectory + SConfig.Name_En + SConfig.Version.ToString("f0");
            String relativeSkinPath = "/" + new DirectoryInfo(SkinBaseDirectory).Name + "/" + SConfig.Name_En + SConfig.Version.ToString("f0");

            #region 添加组件
            var Compoments = new List<yy_Page>();
            var CompomentsMasterPages = SConfig.Pages.Where(a => a.PageType == PageType.Component).ToList();
            foreach (var v in CompomentsMasterPages)
            {
                var CompomentItem = new yy_Page()
                {
                    CanBuild = v.CanBuild ? 1 : 0,
                    ExtensionName = v.SaveName,
                    LastUpdateTime = DateTime.Now,
                    PageCode = v.Source,
                    PageKind = (int)v.PageType,
                    Summary = v.Remark,
                    PageName = v.Title,
                    SavePath = v.SaveDirectory,
                    TypeID = SkinType.ID,
                    CreateDate = DateTime.Now
                };

                CompomentItem.PageCode = SourceReference_Replace(CompomentItem.PageCode,
                    relativeSkinPath, v.CompressScript, v.CompressStyle, v.CompressDocument);

                Compoments.Add(CompomentItem);

                Thread.Sleep(1);
            }

            if (Compoments.Count > 0)
            {
                    foreach (var v in Compoments)
                    {
                        DB.yy_Page.Add(v);
                    }
                    DB.SaveChanges();
            }
            #endregion

            #region 添加页面（并替换页面中引用的组件ID）
            var Pages = new List<yy_Page>();
            var MasterPages = SConfig.Pages.Where(a => a.PageType != PageType.Component).ToList();
            foreach (var v in MasterPages)
            {
                var PageItem = new yy_Page()
                {
                    CanBuild = v.CanBuild ? 1 : 0,
                    CreateDate = DateTime.Now,
                    ExtensionName = v.SaveName,
                    LastUpdateTime = DateTime.Now,
                    PageCode = v.Source,
                    PageKind = (int)v.PageType,
                    Summary = v.Remark,
                    PageName = v.Title,
                    SavePath = v.SaveDirectory,
                    TypeID = SkinType.ID,
                    BuildType = (int)v.BuildType
                };
                PageItem.PageCode = PageReference_Replace(Compoments, PageItem.PageCode);
                PageItem.PageCode = SourceReference_Replace(PageItem.PageCode,
                    relativeSkinPath, v.CompressScript, v.CompressStyle, v.CompressDocument);
                Pages.Add(PageItem);
                Thread.Sleep(1);
            }

                foreach (var v in Pages)
                {
                    DB.yy_Page.Add(v);
                }
                DB.SaveChanges();
            #endregion

            #region 压缩样式文件夹文件
            if (!String.IsNullOrEmpty(SConfig.CompressStyle) && Directory.Exists(SkinPath + "/" + SConfig.CompressStyle))
            {
                String[] Files = Directory.GetFiles(SkinPath + "/" + SConfig.CompressStyle);

                foreach (var v in Files)
                {
                    String CompressStyleSource = String.Empty;

                    using (var sr = new StreamReader(v))
                    {
                        CompressStyleSource = sr.ReadToEnd();
                    }

                    CompressStyleSource = new Minifier().MinifyStyleSheet(CompressStyleSource);

                    using (var sr = new StreamWriter(v, false))
                    {
                        sr.Write(CompressStyleSource);
                    }

                    Thread.Sleep(1);
                }
            }
            #endregion

            #region 压缩脚本文件夹文件
            if (!String.IsNullOrEmpty(SConfig.CompressScript) && Directory.Exists(SkinPath + "/" + SConfig.CompressScript))
            {
                String[] Files = Directory.GetFiles(SkinPath + "/" + SConfig.CompressScript);

                foreach (var v in Files)
                {
                    String CompressScriptSource = String.Empty;

                    using (var sr = new StreamReader(v))
                    {
                        CompressScriptSource = sr.ReadToEnd();
                    }

                    CompressScriptSource = new Minifier().MinifyJavaScript(CompressScriptSource);

                    using (var sr = new StreamWriter(v, false))
                    {
                        sr.Write(CompressScriptSource);
                    }

                    Thread.Sleep(1);
                }
            }
            #endregion

            File.Delete(SkinPath + "/config.js");

            return String.Empty;
        }

        /// <summary>
        /// 页面模板引用组建替换
        /// 上传之前应为id="组件名称"，替换后即为对应的ID号
        /// </summary>
        /// <param name="pages">当前模板的组件集合</param>
        /// <param name="htmlCode">当前模板页的源代码</param>
        /// <returns>替换后的源代码</returns>
        protected String PageReference_Replace(List<yy_Page> pages, String htmlCode)
        {
            if (pages == null || pages.Count < 1) { return htmlCode; }

            HtmlDocument doc = new HtmlDocument();

            doc.LoadHtml(htmlCode);

            var Tags = doc.DocumentNode.SelectNodes("//yytag");

            if (Tags == null) { return htmlCode; }

            for (Int32 i = 0; i < Tags.Count; i++)
            {
                if (!Tags[i].HasAttributes) { continue; }

                if (Tags[i].Attributes["id"] != null)
                {
                    //源代码在未替换前，ID属性是组件的名称
                    String ComponentName = Tags[i].Attributes["id"].Value;

                    try
                    {
                        //在组件集合中找到名称一样的组件
                        var _Component = pages.Where(a => a.PageName == ComponentName).FirstOrDefault();

                        if (_Component != null)
                        {
                            //替换成当前组件的ID
                            Tags[i].Attributes["id"].Value = _Component.ID.ToString();
                        }
                    }
                    catch { }
                }
            }

            return doc.DocumentNode.OuterHtml;
        }

        /// <summary>
        /// 替换当前源代码中引用的.js/.css/img/video/audio路径
        /// 例如开发时可能使用的是script/script1.js，那么就替换为/SkinName/script/script1.js
        /// </summary>
        /// <param name="htmlCode">源代码</param>
        /// <param name="SkinName">模板名称（模板根路径+模板名称+模板版本号）</param>
        /// <param name="CompressScript">压缩脚本</param>
        /// <param name="CompressStyle">压缩样式</param>
        /// <param name="CompressDocument">压缩源码</param>
        /// <returns>替换后的源代码</returns>
        protected String SourceReference_Replace(String htmlCode, String SkinName, Boolean CompressScript, Boolean CompressStyle, Boolean CompressDocument)
        {
            HtmlDocument doc = new HtmlDocument();

            doc.LoadHtml(htmlCode);

            #region 脚本
            var Tag_Script = doc.DocumentNode.SelectNodes("//script");
            if (Tag_Script != null)
            {
                foreach (var v in Tag_Script)
                {
                    if (v.Attributes["noskin"] == null &&
                        v.HasAttributes && v.Attributes["src"] != null &&
                        SourceReference_CanReplace(v.Attributes["src"].Value))
                    {
                        if (v.Attributes["src"].Value.Substring(0, 1) != "/")
                        {
                            v.Attributes["src"].Value = SkinName + "/" + v.Attributes["src"].Value;
                        }
                        else
                        {
                            v.Attributes["src"].Value = SkinName + v.Attributes["src"].Value;
                        }
                    }

                    else if (CompressScript &&
                        !String.IsNullOrEmpty(v.InnerHtml) &&
                        v.HasAttributes &&
                        v.Attributes["type"] != null &&
                        v.Attributes["type"].ToString().ToLower().Equals("text/javascript"))
                    {
                        v.InnerHtml = new Minifier().MinifyJavaScript(v.InnerHtml);
                    }
                    Thread.Sleep(1);
                }
            }
            #endregion

            #region 样式
            var Tag_link = doc.DocumentNode.SelectNodes("//link");
            if (Tag_link != null)
            {
                foreach (var v in Tag_link)
                {
                    if (v.Attributes["noskin"] == null && v.HasAttributes && v.Attributes["href"] != null && SourceReference_CanReplace(v.Attributes["href"].Value))
                    {
                        if (v.Attributes["href"].Value.Substring(0, 1) != "/")
                        {
                            v.Attributes["href"].Value = SkinName + "/" + v.Attributes["href"].Value;
                        }
                        else
                        {
                            v.Attributes["href"].Value = SkinName + v.Attributes["href"].Value;
                        }
                    }

                    else if (CompressStyle && !String.IsNullOrEmpty(v.InnerHtml))
                    {
                        v.InnerHtml = new Minifier().MinifyStyleSheet(v.InnerHtml);
                    }
                    Thread.Sleep(1);
                }
            }
            #endregion

            #region 图片
            var Tag_img = doc.DocumentNode.SelectNodes("//img");
            if (Tag_img != null)
            {
                foreach (var v in Tag_img)
                {
                    if (v.Attributes["noskin"] == null && v.HasAttributes && v.Attributes["src"] != null && SourceReference_CanReplace(v.Attributes["src"].Value))
                    {
                        if (v.Attributes["src"].Value.Substring(0, 1) != "/")
                        {
                            v.Attributes["src"].Value = SkinName + "/" + v.Attributes["src"].Value;
                        }
                        else
                        {
                            v.Attributes["src"].Value = SkinName + v.Attributes["src"].Value;
                        }
                    }

                    Thread.Sleep(1);
                }
            }
            #endregion

            #region 视频
            var Tag_video = doc.DocumentNode.SelectNodes("//video");
            if (Tag_video != null)
            {
                foreach (var v in Tag_video)
                {
                    if (v.Attributes["noskin"] == null && v.HasAttributes && v.Attributes["src"] != null && SourceReference_CanReplace(v.Attributes["src"].Value))
                    {
                        if (v.Attributes["src"].Value.Substring(0, 1) != "/")
                        {
                            v.Attributes["src"].Value = SkinName + "/" + v.Attributes["src"].Value;
                        }
                        else
                        {
                            v.Attributes["src"].Value = SkinName + v.Attributes["src"].Value;
                        }
                    }
                    Thread.Sleep(1);
                }
            }
            #endregion

            #region 音频
            var Tag_audio = doc.DocumentNode.SelectNodes("//audio");
            if (Tag_audio != null)
            {
                foreach (var v in Tag_audio)
                {
                    if (v.Attributes["noskin"] == null && v.HasAttributes && v.Attributes["src"] != null && SourceReference_CanReplace(v.Attributes["src"].Value))
                    {
                        if (v.Attributes["src"].Value.Substring(0, 1) != "/")
                        {
                            v.Attributes["src"].Value = SkinName + "/" + v.Attributes["src"].Value;
                        }
                        else
                        {
                            v.Attributes["src"].Value = SkinName + v.Attributes["src"].Value;
                        }
                    }
                    Thread.Sleep(1);
                }
            }
            #endregion

            String ResultHtml = doc.DocumentNode.OuterHtml;

            if (CompressDocument)
            {
                ResultHtml = Regex.Replace(ResultHtml, @"\s{2,}", "");
                ResultHtml = Regex.Replace(ResultHtml, "\n", "");
            }

            return ResultHtml;
        }

        /// <summary>
        /// 根据资源连接地址，判断是否需要替换
        /// </summary>
        /// <param name="_SrcAddress"></param>
        /// <returns></returns>
        private Boolean SourceReference_CanReplace(String _SrcAddress)
        {
            if (String.IsNullOrEmpty(_SrcAddress)) { return false; }

            //if (_SrcAddress.Trim().Substring(0, 1) == "/") { return false; }

            if (_SrcAddress.Length > 7 && _SrcAddress.Substring(0, 7).ToLower().Equals("http://")) { return false; }

            return true;
        }
    }

    #region 风格包实体
    #region 风格包实体
    /// <summary>
    /// 模板风格配置
    /// </summary>
    public class SkinConfig
    {
        /// <summary>
        /// 风格名称 - 中文
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// 风格名称 - 英文
        /// </summary>
        public String Name_En { get; set; }

        /// <summary>
        /// 当前版本
        /// </summary>
        public Int32 Version { get; set; }
        /// <summary>
        /// 风格截图
        /// </summary>
        public List<String> Images { get; set; }
        /// <summary>
        /// 模板列表
        /// </summary>
        public List<PageMaster> Pages { get; set; }

        /// <summary>
        /// 特殊选项 要压缩的样式文件夹
        /// </summary>
        public String CompressStyle { get; set; }
        /// <summary>
        /// 特殊选项 要压缩的脚本文件夹
        /// </summary>
        public String CompressScript { get; set; }
        /// <summary>
        /// 特殊选项 要压缩的图片文件夹
        /// </summary>
        public String CompressImage { get; set; }
        public String Email { get; set; }
        public String Mobile { get; set; }
        public String QQ { get; set; }
        public String Summary { get; set; }
        public int SupportPlatform { get; set; }
        public String Website { get; set; }
        public String Author { get; set; }
    }
    #endregion

    #region 页面实体
    /// <summary>
    /// 模板详情
    /// </summary>
    public class PageMaster
    {
        /// <summary>
        /// 模板名称
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// 0：单页模板
        /// 1：组件
        /// 2：列表页模板
        /// 3，其他
        /// </summary>
        public PageType PageType { get; set; }

        /// <summary>
        /// 页面类型
        /// </summary>
        public BuildType BuildType { get; set; }

        /// <summary>
        /// 保存路径
        /// </summary>
        public String SaveDirectory { get; set; }

        /// <summary>
        /// 保存文件名称
        /// </summary>
        public String SaveName { get; set; }

        /// <summary>
        /// 模板备注
        /// </summary>
        public String Remark { get; set; }

        /// <summary>
        /// 模板代码
        /// </summary>
        public String Source { get; set; }

        /// <summary>
        /// 是否支持一键集成
        /// </summary>
        public Boolean CanBuild { get; set; }

        /// <summary>
        /// 特殊选项 在存储模板数据时，是否进行脚本压缩
        /// </summary>
        public Boolean CompressScript { get; set; }

        /// <summary>
        /// 特殊选项 在存储模板数据时，是否进行样式压缩
        /// </summary>
        public Boolean CompressStyle { get; set; }

        /// <summary>
        /// 特殊选项 在存储模板数据时，是否进行模板代码压缩
        /// </summary>
        public Boolean CompressDocument { get; set; }
    }
    #endregion

    #region 功能类型
    /// <summary>
    /// 页面类型
    /// </summary>
    public enum PageType
    {
        /// <summary>
        /// 单页模板
        /// </summary>
        SinglePage = 0,
        /// <summary>
        /// 公用组件
        /// </summary>
        Component = 1,
        /// <summary>
        /// 列表页
        /// </summary>
        ListPage = 2,
        /// <summary>
        /// 其他
        /// </summary>
        Other = 3
    }
    #endregion

    #region 页面类型
    /// <summary>
    /// 页面生成的类型
    /// </summary>
    public enum BuildType
    {
        /// <summary>
        /// 普通页面
        /// </summary>
        Page = 0,

        /// <summary>
        /// 新闻详情页模板
        /// 将在发布新闻时自动生成该页面
        /// </summary>
        NewsDetail = 1,

        /// <summary>
        /// 产品详情页模板
        /// 将在发布产品时自动生成该页面
        /// </summary>
        ProductDetail = 2,

        /// <summary>
        /// 视频详情页模板
        /// </summary>
        VideoDetail = 3,

        /// <summary>
        /// 相册详情页模板
        /// </summary>
        PhotoDetail = 4
    }
    #endregion
    #endregion
}