using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace yycms.admin.Controllers
{
    public class FansController : BasicController
    {
        // GET: Fans
        public ActionResult Index()
        {
            ViewBag.Types = News_Types();
            return View();
        }

        private String News_Types()
        {
            return JsonConvert.SerializeObject(DB.yy_News_Type
                .OrderBy(x => x.ShowIndex)
                .ToList());
        }
    }
}