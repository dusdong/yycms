using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace yycms.admin.Controllers
{
    /// <summary>
    /// 用户列表
    /// </summary>
    public class UserController : BasicController
    {
        // GET: User
        public ActionResult Index()
        {
            //用户分类
            ViewBag.Roles = Types_List();

            return View();
        }

        // GET: User
        public ActionResult Add()
        {
            //用户分类
            ViewBag.Roles = Types_List();

            //用户权限
            ViewBag.Permissions =Permission_Tree();

            return View();
        }

        // GET: User
        public ActionResult Edit()
        {
            //用户分类
            ViewBag.Roles = Types_List();

            //用户权限
            ViewBag.Permissions = Permission_Tree();

            return View();
        }

        // GET: User
        public ActionResult Types()
        {
            //用户权限
            ViewBag.Permissions = Permission_Tree();

            return View();
        }

        string Types_List() 
        {
            return JsonConvert.SerializeObject(DB.yy_User_Type
                .Where(x => x.IsShow == 1)
                .OrderBy(x => x.ID)
                .Select(x => new { ID = x.ID, Name = x.Name, Desc = x.Summary, PID = x.PID }).ToList());
        }

        string Permission_Tree()
        {
            var result = new List<object>();

            var Types = DB.yy_Permission_Type.OrderBy(x => x.ShowIndex).ToList();

            foreach (var v in Types)
            {
                result.Add(new
                {
                    Name = v.Name,
                    ID = -v.ID,
                    PID = 0
                });

                var pmsList = DB.yy_Permission.Where(x => x.TypeID == v.ID).ToList();

                foreach (var vv in pmsList)
                {
                    result.Add(new
                    {
                        Name = vv.MenuName,
                        ID = vv.ID,
                        PID = -v.ID
                    });
                }
            }

            return JsonConvert.SerializeObject(result);
        }
    }
}