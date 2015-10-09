using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace yycms.admin.Controllers
{
    public class PhotoController : BasicController
    {
        //相册列表
        public ActionResult Index()
        {
            ViewBag.Types = Photo_Types();
            return View();
        }

        //新增相册
        public ActionResult Add()
        {
            ViewBag.Types = Photo_Types();
            return View();
        }

        // 修改相册
        public ActionResult Edit()
        {
            ViewBag.Types = Photo_Types();
            return View();
        }

        // GET: 相册分类
        public ActionResult Types()
        {
            return View();
        }

        private String Photo_Types()
        {
            return JsonConvert.SerializeObject(DB.yy_Photo_Type
                .OrderBy(x => x.ShowIndex)
                .ToList());
        }
    }
}