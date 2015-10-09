using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace yycms.admin.Controllers
{
    public class ProductController : BasicController
    {
        //产品列表
        public ActionResult Index()
        {
            ViewBag.Types = Product_Types();
            return View();
        }

        //新增产品
        public ActionResult Add()
        {
            ViewBag.Types = Product_Types();
            return View();
        }

        // 修改产品
        public ActionResult Edit()
        {
            ViewBag.Types = Product_Types();
            return View();
        }

        // GET: 产品分类
        public ActionResult Types()
        {
            return View();
        }

        private String Product_Types()
        {
            return JsonConvert.SerializeObject(DB.yy_Product_Type
                .OrderBy(x => x.ShowIndex)
                .ToList());
        }
    }
}