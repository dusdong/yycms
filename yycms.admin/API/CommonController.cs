using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using yycms.admin.Models;
using yycms.entity;

namespace yycms.admin.API
{
    /// <summary>
    /// 通用
    /// </summary>
     [BasicAuthen]
    public class CommonController : BasicAPI
    {
        #region 上传文件
        /// <summary>
        /// 上传文件
        /// </summary>
        [HttpPost]
        public String Upload()
        {
            if (HttpContext.Current.Request.Files.Count < 1) { return ""; }

            var result = new List<String>();
            String today = DateTime.Now.ToString("yyyy-MM-dd");
            String dic = HostingEnvironment.MapPath("~/images/upload/" + today + "/");
            if (!Directory.Exists(dic))
            {
                Directory.CreateDirectory(dic);
            }

            for (var i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                HttpPostedFile file = HttpContext.Current.Request.Files[i];

                if (file.ContentLength < 1)
                {
                    continue;
                }

                String fileName = Guid.NewGuid().ToString();
                String ext = Path.GetExtension(file.FileName);
                String path = dic + fileName + ext;
                file.SaveAs(path);

                var config = DB.yy_SiteSetting.FirstOrDefault();

                #region 加水印
                if (config!=null && 
                    config.EnabelWatermark==1 && 
                    !String.IsNullOrEmpty(config.Watermark) && 
                    File.Exists(HttpContext.Current.Server.MapPath(config.Watermark)))
                {
                    var ImgPath = HttpContext.Current.Server.MapPath(config.Watermark);

                    var NewImagePath = HttpContext.Current.Server.MapPath("/images/upload/" + today);

                    fileName = Guid.NewGuid().ToString();

                    var NewImageSavePath = NewImagePath + "\\" + fileName + ext;

                    var ImgItem = System.Drawing.Image.FromFile(path);

                    Watermark.AddImageSignPic(ImgItem,  NewImageSavePath , ImgPath, 5, 100, 6);
                    try
                    {
                        File.Delete(path);
                    }
                    catch
                    {
                        
                    }

                    result.Add("/images/upload/" + today + "/" + fileName + ext);
                }
                #endregion

                #region 不加水印
                else
                {
                    result.Add("/images/upload/" + today + "/" + fileName + ext);
                }
                #endregion
            }

            if (result.Count < 1) { return ""; }

            return String.Join(",", result);
        }
        #endregion

        #region 上传证书文件
        /// <summary>
        /// 上传证书文件
        /// </summary>
        [HttpPost]
        public String UploadCertificate()
        {
            if (HttpContext.Current.Request.Files.Count < 1) { return ""; }

            var dic = HostingEnvironment.MapPath("~/images/wxpaycertificate/" + User.ID.ToString() + "/");

            if (!Directory.Exists(dic))
            {
                Directory.CreateDirectory(dic);
            }

            var file = HttpContext.Current.Request.Files[0];
            if (file.ContentLength < 1)
            {
                return null;
            }

            if (!Path.GetExtension(file.FileName).ToLower().Equals(".p12"))
            {
                return null;
            }

            String savePath = dic + file.FileName;

            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }
            file.SaveAs(savePath);

            return savePath;
        }
        #endregion

        #region 上传模板包
        /// <summary>
        /// 上传证书文件
        /// </summary>
        [HttpPost]
        public Object UploadSkinPackage()
        {
            if (HttpContext.Current.Request.Files.Count < 1) 
            {
                return new
                {
                    code = 1,
                    msg = "不存在任何文件。"
                };
            }

            var file = HttpContext.Current.Request.Files[0];
            if (file.ContentLength < 1)
            {
                return new 
                {
                    code = 2, 
                    msg = "上传风格包没有任何内容。" 
                };
            }

            if (!Path.GetExtension(file.FileName).ToLower().Equals(".zip"))
            {
                return new 
                {
                    code = 3, 
                    msg = "风格包必须是.zip格式。" 
                };
            }

            byte[] PackageData = null;

            using (var inputStream = file.InputStream)
            {
                var memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                PackageData = memoryStream.ToArray();
            }

            try {
                String Result = new ImportSkin().Import(PackageData, User.ID);

                if (!String.IsNullOrEmpty(Result))
                {
                    return new { code = 4, msg = Result };
                }
                else
                {
                    return new { code = 0, msg = String.Empty };
                }
            }
            catch(Exception ex)
            {
                return new { code = 5, msg = ex.Message + ex.Source + ex.StackTrace };
            }

        }
        #endregion

        #region 上传蜘蛛
        /// <summary>
        /// 上传蜘蛛
        /// </summary>
        [HttpPost]
        public Object UploadCrawler()
        {
            if (HttpContext.Current.Request.Files.Count < 1) { return ""; }

            String TypeIDs = HttpContext.Current.Request.Params["typeids"];

            if(String.IsNullOrEmpty(TypeIDs))
            {
                return new { code = 1, msg = "所属分类不能为空。" };
            }

            for (var i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                HttpPostedFile file = HttpContext.Current.Request.Files[i];

                if (file.ContentLength < 1) { return new { code = 2, msg = "文件内容为空。" }; }

                String jsonScript = String.Empty;

                try
                {
                    using (var sr = new StreamReader(file.InputStream))
                    {
                        jsonScript = sr.ReadToEnd();
                    }
                }
                catch
                {
                }

                if (string.IsNullOrEmpty(jsonScript)) { return new { code = 3, msg = "文件读取错误。" };  }

                yy_Spider obj = null;

                try
                {
                    obj = JsonConvert.DeserializeObject<yy_Spider>(jsonScript);
                    obj.Code = (obj.SourceUrls + obj.RuleConfig).GetHashCode().ToString();

                    var spideItem = DB.yy_Spider.Where(x => x.Code == obj.Code).FirstOrDefault();

                    if (spideItem!=null)
                    {
                        return new { code = 4, msg = "已存在的蜘蛛。" };
                    }

                    obj.CreateDate = DateTime.Now;
                    obj.LastStartTime = DateTime.Now;
                    obj.UserID = User.ID;
                    obj.LookCount = 0;
                    obj.Status = 0;
                    obj.TypeIDs = TypeIDs;
                }
                catch
                {
                    return new { code = 5, msg = "文件格式为空。" };
                }

                DB.yy_Spider.Add(obj);
                DB.SaveChanges();
            }

            return new { code = 0, msg = "导入成功。" };
        }
        #endregion

        #region 站点配置
        /// <summary>
        /// 站点配置
        /// </summary>
        [HttpPost]
        public ResponseItem SiteConfig(yy_SiteSetting value)
        {
            DB.Database.ExecuteSqlCommand("DELETE yy_SiteSetting");
            DB.yy_SiteSetting.Add(value);
            DB.SaveChanges();

            var SiteSettingStr = HttpRuntime.Cache.Get(Const.SiteSettingKey);

            if (HttpRuntime.Cache[Const.SiteSettingKey] == null)
            {
                HttpRuntime.Cache.Remove(Const.SiteSettingKey);
            }

            HttpRuntime.Cache.Insert(Const.SiteSettingKey, value);

            return new ResponseItem(0, String.Empty);
        }
        #endregion
    }
}