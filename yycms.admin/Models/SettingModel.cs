using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yycms.admin.Models
{
    /// <summary>
    /// 站点配置实体
    /// </summary>
    public class SettingModel
    {
        /// <summary>
        /// 网站域名
        /// </summary>
        public String Biz_SiteAddress { get; set; }

        /// <summary>
        /// 水印图片
        /// </summary>
        public String watermark { get; set; }

        /// <summary>
        /// 自动加水印
        /// </summary>
        public bool EnabelWatermark { get; set; }

        /// <summary>
        /// 站长邮箱
        /// </summary>
        public String Biz_AdminEmail { get; set; }

        /// <summary>
        /// 邮件服务器IP
        /// </summary>
        public String MailUrl { get; set; }

        /// <summary>
        /// 邮件服务器端口
        /// </summary>
        public int MailPort { get; set; }

        /// <summary>
        /// 邮件服务器账号
        /// </summary>
        public String MailAccount { get; set; }

        /// <summary>
        /// 邮件服务器密码
        /// </summary>
        public String MailPassword { get; set; }

        /// <summary>
        /// 系统顶部自定义导航
        /// </summary>
        public String TopLinks { get; set; }
    }
}
