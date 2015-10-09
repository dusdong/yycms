
namespace yycms.admin.Models
{
    /// <summary>
    /// 响应实体
    /// </summary>
   public class ResponseItem
    {
        public ResponseItem() { }

        public ResponseItem(int _code,string _msg)
        {
            this.code = _code;
            this.msg = _msg;
        }

        /// <summary>
        /// 响应代码
        /// </summary>
        public int code { get; set; }

        /// <summary>
        /// 相应内容
        /// </summary>
        public string msg { get; set; }
    }
}
