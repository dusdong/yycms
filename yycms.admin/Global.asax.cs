using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace yycms.admin
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            // 添加一个转换器 IsoDateTimeConverter，其用于日期数据的序列化和反序列化
            var dateTimeConverter = new IsoDateTimeConverter();
            dateTimeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            serializerSettings.Converters.Add(dateTimeConverter);

            // 清除全部 Formatter（默认有 4 个，分别是：JsonMediaTypeFormatter, XmlMediaTypeFormatter, FormUrlEncodedMediaTypeFormatter, JQueryMvcFormUrlEncodedFormatter）
            // GlobalConfiguration.Configuration.Formatters.Clear();

            // 如果请求 header 中有 accept: text/html 则返回这个新建的 JsonMediaTypeFormatter 数据
            var jsonFormatter = new JsonMediaTypeFormatter();
            jsonFormatter.SerializerSettings = serializerSettings;
            // jsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html")); 
            jsonFormatter.MediaTypeMappings.Add(new RequestHeaderMapping("accept", "text/html", StringComparison.InvariantCultureIgnoreCase, 
                true, new MediaTypeHeaderValue("text/html")));
            GlobalConfiguration.Configuration.Formatters.Insert(0, jsonFormatter);


            // 请求 url 中如果带有参数 xml=true，则返回 xml 数据
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.MediaTypeMappings.Add(new QueryStringMapping("xml", "true", "application/xml"));

            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new Engine());
        }
    }

    public class Engine : RazorViewEngine
    {
        public Engine()
        {

            base.AreaViewLocationFormats = new string[]

            {

                "~/Areas/{2}/Views/{1}/{0}.cshtml",

                "~/Areas/{2}/Views/Shared/{0}.cshtml",

            };

            base.AreaMasterLocationFormats = new string[]

            {

                "~/Areas/{2}/Views/{1}/{0}.cshtml",

                "~/Areas/{2}/Views/Shared/{0}.cshtml",



            };

            base.AreaPartialViewLocationFormats = new string[]

            {

                "~/Areas/{2}/Views/{1}/{0}.cshtml",

                "~/Areas/{2}/Views/Shared/{0}.cshtml",

            };

            base.ViewLocationFormats = new string[]

            {

                "~/Views/{1}/{0}.cshtml",

                "~/Views/Shared/{0}.cshtml",

            };

            base.MasterLocationFormats = new string[]

            {

                "~/Views/{1}/{0}.cshtml",

                "~/Views/Shared/{0}.cshtml",

            };

            base.PartialViewLocationFormats = new string[]

            {

                "~/Views/{1}/{0}.cshtml",

                "~/Views/Shared/{0}.cshtml",

            };

            base.FileExtensions = new string[]

            {

                "cshtml",

            };

        }
    }
}
