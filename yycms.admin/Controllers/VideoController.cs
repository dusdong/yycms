using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace yycms.admin.Controllers
{
    public class VideoController : BasicController
    {
        //视频列表
        public ActionResult Index()
        {
            ViewBag.Types = Video_Types();
            return View();
        }

        //新增视频
        public ActionResult Add()
        {
            ViewBag.Types = Video_Types();
            return View();
        }

        // 修改视频
        public ActionResult Edit()
        {
            ViewBag.Types = Video_Types();
            return View();
        }

        // GET: 视频分类
        public ActionResult Types()
        {
            return View();
        }

        private String Video_Types()
        {
            return JsonConvert.SerializeObject(DB.yy_Video_Type
                .OrderBy(x => x.ShowIndex)
                .ToList());
        }
    }
}