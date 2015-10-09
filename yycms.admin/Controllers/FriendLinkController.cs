using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;

namespace yycms.admin.Controllers
{
    public class FriendLinkController : BasicController
    {
        //友链列表
        public ActionResult Index()
        {
            return View();
        }

        //新增友链
        public ActionResult Add()
        {
            return View();
        }

        // 修改友链
        public ActionResult Edit()
        {
            return View();
        }

        // 友链分类
        public ActionResult Types()
        {
            return View();
        }
    }
}