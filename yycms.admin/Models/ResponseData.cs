using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yycms.admin.Models
{
    /// <summary>
    /// 输出实体
    /// </summary>
    /// <typeparam name="T"></typeparam>
   public class ResponseData<T>
    {
        /// <summary>
        /// 每页数量。
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 当前页码。
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 数据总数。
        /// </summary>
        public int DataCount { get; set; }

        /// <summary>
        /// 分页数。
        /// </summary>
        public int PageCount { get; set; }

        /// <summary>
        /// 数据集合
        /// </summary>
        public List<T> Data { get; set; }

        public ResponseData() { }
        public ResponseData(int _PageSize, int _PageIndex, int _DataCount, int _PageCount, List<T> _Data)
        {
            this.PageSize = _PageSize;
            this.PageIndex = _PageIndex;
            this.DataCount = _DataCount;
            this.PageCount = _PageCount;
            this.Data = _Data;
        }
    }
}
