using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using yycms.admin.Models;
using yycms.entity;

namespace yycms.admin.API
{
    public class BasicAuthenAttribute : ActionFilterAttribute
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
        protected yy_User User
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
                    var PermissionStr = HttpRuntime.Cache.Get(Const.PermissionCacheKey);

                    if (PermissionStr == null)
                    {
                        using (var DB = new DBConnection())
                        {
                            _Permission = DB.Database.SqlQuery<yy_Permission>(
                                String.Format(Const.PermissionSql, User.Permission)).ToList();
                        }

                        HttpRuntime.Cache.Insert(Const.PermissionCacheKey, _Permission);
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

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            #region 如果无需权限验证直接跳过
            if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Count > 0)
            {
                base.OnActionExecuting(actionContext);

                return;
            }
            #endregion

            String UserStr = String.Empty;

            try
            {
                UserStr = actionContext.Request.Headers.GetCookies().FirstOrDefault()
                    .Cookies.FirstOrDefault().Value;

                UserStr = HttpUtility.UrlDecode(UserStr);
            }
            catch
            {

            }

            if (!String.IsNullOrEmpty(UserStr))
            {
                try
                {
                    User = JsonConvert.DeserializeObject<yy_User>(UserStr);
                }
                catch { User = null; }
            }

            if (User == null)
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("无效的用户",
                        Encoding.UTF8,
                        "application/json")
                };

                return;
            }

            //String ActionPath = "/" + actionContext.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower() + "/" + actionContext.ActionDescriptor.ActionName.ToLower();
            //如果没有权限访问当前API方法
            //如果需要验证每一个API的权限可继续验证，这里暂时不需要了
        }
    }
}