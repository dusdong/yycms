using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yycms.admin.Models
{
   public class WechatModel
    {
        public String[] IDs { get; set; }

        public String Message { get; set; }

        public String Type { get; set; }

        public String MaterialID { get; set; }

        public article[] News { get; set; }
    }

    public class article
    {
        public String title { get; set; }
        public String description { get; set; }
        public String url { get; set; }
        public String picurl { get; set; }
    }
}
