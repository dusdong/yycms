using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yycms.admin.Models
{
    public class yy_Page_Type2
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public int IsMaster { get; set; }
        public long SupportPlatform { get; set; }
        public string Author { get; set; }
        public string Website { get; set; }
        public string QQ { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Summary { get; set; }
        public string Name { get; set; }
        public string Name_En { get; set; }
        public List<yy_Page_Type2Image> Images { get; set; }
        public int Version { get; set; }
        public System.DateTime CreateDate { get; set; }
    }

    public class yy_Page_Type2Image
    {
        public string src { get; set; }
    }
}
