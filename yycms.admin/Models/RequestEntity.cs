using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yycms.admin.Models
{
    /// <summary>
    /// 请求实体
    /// </summary>
  public class RequestEntity
    {
        /// <summary>
        /// 是否倒序。
        /// </summary>
        public Boolean IsDesc { get; set; }

        /// <summary>
        /// 排序字段名称。
        /// </summary>
        public String OrderBy { get; set; }

        /// <summary>
        /// 当前页码，默认从0开始。
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页条数，最少20条。
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 标题关键字。
        /// </summary>
        public String Title { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public String KeyWords { get; set; }

        /// <summary>
        /// 所属分类ID。
        /// </summary>
        public long TypeID { get; set; }

        /// <summary>
        /// 新增数据的起止时间：开始。
        /// </summary>
        public String StartTime { get; set; }

        /// <summary>
        /// 新增数据的起止时间：结束。
        /// </summary>
        public String EndTime { get; set; }
    }
}
