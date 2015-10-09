using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;

namespace yycms.admin.Controllers
{
    public class NewsController : BasicController
    {
        //新闻列表
        public ActionResult Index()
        {
            ViewBag.Types = News_Types();
            return View();
        }

        //新增新闻
        public ActionResult Add()
        {
            ViewBag.Types = News_Types();
            return View();
        }

        // 修改新闻
        public ActionResult Edit()
        {
            ViewBag.Types = News_Types();
            return View();
        }

        // GET: 新闻分类
        public ActionResult Types()
        {
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