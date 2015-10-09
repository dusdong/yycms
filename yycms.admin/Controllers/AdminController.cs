using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using yycms.entity;

namespace yycms.admin.Controllers
{
    public class AdminController : BasicController
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            return View();
        }

        public ActionResult Setting()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (User != null) 
            {
                return new RedirectResult("/Admin/index");
            }
            return View();
        }
    }
}
