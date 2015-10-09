using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;

namespace yycms.admin.Controllers
{
    public class BannerController : BasicController
    {
        //广告列表
        public ActionResult Index()
        {
            ViewBag.Types = Banner_Types();
            return View();
        }

        //新增广告
        public ActionResult Add()
        {
            ViewBag.Types = Banner_Types();
            return View();
        }

        // 修改广告
        public ActionResult Edit()
        {
            ViewBag.Types = Banner_Types();
            return View();
        }

        // 广告分类
        public ActionResult Types()
        {
            return View();
        }

        private String Banner_Types()
        {
            return JsonConvert.SerializeObject(DB.yy_Banner_Type
                .OrderBy(x => x.ShowIndex)
                .ToList());
        }
    }
}