using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using yycms.admin.Models;
using yycms.entity;

namespace yycms.admin.Controllers
{
    public class BasicController : Controller
    {
        #region 数据库
        private DBConnection _DB;
        protected DBConnection DB
        {
            get { if (_DB == null) { _DB = new DBConnection(); } return _DB; }
        }
        #endregion

        #region 当前用户
        private yy_User _User;
        protected new yy_User User
        {
            get { return _User; }
            set { _User = value; }
        }
        #endregion

        #region 当前用户的权限集合
        List<yy_Permission> _Permission;
        protected List<yy_Permission> Permission
        {
            get
            {
                if (_Permission == null)
                {
                    var PermissionStr = HttpRuntime.Cache.Get(Const.PermissionCacheKey+User.ID.ToString());

                    if (PermissionStr == null)
                    {
                            #region 用户权限
                            _Permission = DB.Database.SqlQuery<yy_Permission>(
                                String.Format(Const.PermissionSql,User.Permission)).ToList();
                            HttpRuntime.Cache.Insert(Const.PermissionCacheKey + User.ID.ToString(), _Permission);
                            #endregion

                            #region 权限分组
                            var _PermissionType = DB.yy_Permission_Type.ToList();
                            HttpRuntime.Cache.Insert(Const.PermissionTypeCacheKey, _PermissionType);
                            #endregion
                    }
                    else
                    {
                        _Permission = PermissionStr as List<yy_Permission>;
                    }
                }

                return _Permission;
            }
        }
        #endregion

        #region 系统配置
        yy_SiteSetting _SiteSetting;
        protected yy_SiteSetting SiteSetting
        {
            get
            {
                if (_SiteSetting == null)
                {
                    var SiteSettingStr = HttpRuntime.Cache.Get(Const.SiteSettingKey);

                    if (SiteSettingStr == null)
                    {
                        _SiteSetting = DB.yy_SiteSetting.FirstOrDefault();

                        HttpRuntime.Cache.Insert(Const.SiteSettingKey, _SiteSetting);
                    }
                    else
                    {
                        _SiteSetting = SiteSettingStr as yy_SiteSetting;
                    }
                }

                return _SiteSetting;
            }
        }
        #endregion

        /// <summary>
        /// 权限验证，无需权限请在action或controller标记AllowAnonymousAttribute
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            #region 站点配置
            ViewBag.SiteSetting = SiteSetting;
            #endregion

            #region 获取用户信息
            var UserCK = Request.Cookies.Get(Const.SessionId);
            if (UserCK != null && !String.IsNullOrEmpty(UserCK.Value))
            {
                try
                {
                    User = JsonConvert.DeserializeObject<yy_User>(
                        HttpUtility.UrlDecode(UserCK.Value)
                        );
                }
                catch { }
            }
            #endregion

            #region 如果无需权限验证直接跳过
            if (filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) ||
              filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true))
            {
                base.OnActionExecuting(filterContext);

                return;
            }
            #endregion

            #region 登陆失败，或没有登陆
            if (User == null)
            {
                filterContext.Result = new RedirectResult("/Admin/Login");
                return;
            }
            #endregion

            #region 没有权限访问当前页面
            String ActionPath = "/" +
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower() +
                "/" +
                filterContext.ActionDescriptor.ActionName.ToLower();

            var CurrentViewPage = Permission.Where(x => x.PageName == ActionPath).FirstOrDefault();

            if (CurrentViewPage == null)
            {
                filterContext.Result = new RedirectResult("/Admin/NoPermission");
                return;
            }
            //当前访问的页面，用于在客户端定位页面所属的菜单类目，做选中效果
            ViewBag.CurrentPage = CurrentViewPage;

            base.OnActionExecuting(filterContext);
            #endregion

            ViewBag.User = User;
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            DB.Dispose();
            base.Dispose(disposing);
        }
    }
}