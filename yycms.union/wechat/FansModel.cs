using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yycms.union.wechat
{
    public class FansModel
    {
        public int total
        {
            get; set;
        }
        public int count
        {
            get; set;
        }

        public FansItem data { get; set; }
        public String next_openid { get; set; }
    }

    public class FansItem
    {
        public String[] openid { get; set; }
    }

    public class FansInfo
    {
        public int subscribe { get; set;}
        public String openid { get; set; }
        public String nickname { get; set; }
        public int sex { get; set; }
        public String language { get; set; }
        public String city { get; set; }
        public String province { get; set; }
        public String country { get; set; }
        public String headimgurl { get; set; }
        public int subscribe_time { get; set; }
        public String remark { get; set; }
        public int groupid { get; set; }
    }
}
