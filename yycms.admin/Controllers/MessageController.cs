using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;

namespace yycms.admin.Controllers
{
    public class MessageController : BasicController
    {
        //消息列表
        public ActionResult Index()
        {
            return View();
        }

        //新增消息
        public ActionResult Add()
        {
            return View();
        }

        // 修改消息
        public ActionResult Edit()
        {
            return View();
        }

        // 消息分类
        public ActionResult Types()
        {
            return View();
        }
    }
}