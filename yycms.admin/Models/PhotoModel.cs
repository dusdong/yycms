using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yycms.entity;

namespace yycms.admin.Models
{
    /// <summary>
    /// 相册详情
    /// </summary>
   public class PhotoModel
    {
        /// <summary>
        /// 相册实体。
        /// </summary>
        public yy_Photo Photo { get; set; }

        /// <summary>
        /// 相册图片集合。
        /// </summary>
        public List<yy_Photo_Item> Items { get; set; }
    }
}
