//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace yycms.entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class yy_Spider_News
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public long Identifer { get; set; }
        public long SpiderID { get; set; }
        public string Title { get; set; }
        public string SourceUrl { get; set; }
        public string DefaultImage { get; set; }
        public string KeyWords { get; set; }
        public string Summary { get; set; }
        public string Info { get; set; }
        public int IsSync { get; set; }
        public System.DateTime CreateDate { get; set; }
    }
}