using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace yycms.admin.Models
{
    public class SpiderListItem
    {
        public long ID { get; set; }

        public String Title { get; set; }

        public String KeyWords { get; set; }

        public Int32 IsShow { get; set; }

        public Decimal Quality { get; set; }

        public Int32 SpiderMode { get; set; }

        public String TargetPlatforms { get; set; }

        public long ExecutionInterval { get; set; }

        public DateTime LastStartTime { get; set; }

        public long LookCount { get; set; }

        public int Status { get; set; }
    }
}