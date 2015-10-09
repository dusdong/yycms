using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace yycms.admin.Controllers
{
    public class PageController : BasicController
    {
        //模板包列表
        public ActionResult Index()
        {
            return View();
        }

        //生成模板
        public ActionResult Build()
        {
            ViewBag.Type_News = JsonConvert.SerializeObject(
                DB.yy_News_Type.OrderByDescending(x => x.ID).ToList());

            ViewBag.Type_Product = JsonConvert.SerializeObject(
                DB.yy_Product_Type.OrderByDescending(x => x.ID).ToList());

            ViewBag.Type_Photo = JsonConvert.SerializeObject(
                DB.yy_Photo_Type.OrderByDescending(x => x.ID).ToList());

            ViewBag.Type_Video = JsonConvert.SerializeObject(
                DB.yy_Video_Type.OrderByDescending(x => x.ID).ToList());

            return View();
        }

        //查看生成进度
        public ActionResult Build_Progress()
        {
            return View();
        }

        //模板包页面列表
        public ActionResult Types()
        {
            return View();
        }

        //新增模板包页面
        public ActionResult Add()
        {
            ViewBag.Types = Page_Types();
            return View();
        }

        //新增模板包页面
        public ActionResult Progress()
        {
            ViewBag.Types = Page_Types();
            return View();
        }

        //修改模板包页面
        public ActionResult Edit()
        {
            ViewBag.Types = Page_Types();
            return View();
        }

        private String Page_Types()
        {
            return JsonConvert.SerializeObject(DB.yy_Page_Type
                .OrderByDescending(x => x.ID)
                .ToList());
        }
    }
}