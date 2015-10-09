using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yycms.entity;

namespace yycms.admin.Models
{
    /// <summary>
    /// 用户详情
    /// </summary>
   public class UserModel
    {
        /// <summary>
        /// 基本信息
        /// </summary>
        public yy_User User { get; set; }

        /// <summary>
        /// 微信信息
        /// </summary>
        public yy_Platforms wechat { get; set; }
    }
}
