using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace yycms.admin.Models
{
    public class NewsListItem
    {
        public long ID { get; set; }

        public String Title { get; set; }

        public String KeyWords { get; set; }

        public int IsShow { get; set; }

        public long LookCount { get; set; }

        public DateTime CreateDate { get; set; }
    }

    /// <summary>
    /// 粉丝列表用到的新闻实体
    /// </summary>
    public class FansNewsItem
    {
        public long ID { get; set; }

        public String Title { get; set; }

        public String DefaultImg { get; set; }

        public String Summary { get; set; }

        public long LookCount { get; set; }

        public String WechatNewsUrl { get; set; }

        public DateTime CreateDate { get; set; }
    }
}