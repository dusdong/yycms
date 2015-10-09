using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace yycms.admin.Models
{
    public class Const
    {
        /// <summary>
        /// 当前用户在客户端cookie的缓存Key
        /// </summary>
        public const String SessionId = "SessionId-Id";

        /// <summary>
        /// 当前用户菜单的缓存key，使用时还需要搭配UserID做唯一标识
        /// </summary>
        public const String PermissionCacheKey = "PermissionCacheKey_";

        /// <summary>
        /// 当前用户菜单分组的缓存key
        /// </summary>
        public const String PermissionTypeCacheKey = "PermissionTypeCacheKey";

        /// <summary>
        /// 用户权限查询语句 
        /// </summary>
        public const String PermissionSql = "SELECT * FROM [dbo].[yy_Permission] WITH(NOLOCK) WHERE ID IN ({0})";

        /// <summary>
        /// 站点配置缓存Key
        /// </summary>
        public const String SiteSettingKey = "SiteSetting";
    }
}