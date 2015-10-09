using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace yycms.admin.Models
{
    /// <summary>
    /// 第三方平台
    /// </summary>
    public enum Unions
    {
        /// <summary>
        /// 默认（即不存在的平台，用于外部方法使用）
        /// </summary>
        Default = -1,

        //[Union(typeof(BaiDuSDK))]
        BaiDu = 0,

        //[Union(SDKType = typeof(DouBanSDK))]
        DouBan = 1,

        //[Union(SDKType = typeof(KaiXinSDK))]
        KaiXin = 2,

        //[Union(SDKType = typeof(QQExmailSDK))]
        QQExmail = 3,

        //[Union(SDKType = typeof(RenRenSDK))]
        RenRen = 4,

        //[Union(SDKType = typeof(SinaBlogSDK))]
        SinaBlog = 5,

        //[Union(SDKType = typeof(SohuSDK))]
        Sohu = 6,

        //[Union(SDKType = typeof(T163SDK))]
        T163 = 7,

        //[Union(SDKType = typeof(TaoBaoSDK))]
        TaoBao = 8,

        //[Union(SDKType = typeof(TQQSDK))]
        TQQ = 9,

        //[Union(SDKType = typeof(WeiboSDK))]
        Weibo = 10,

        //[Union(SDKType = typeof(WeiXinGZSDK))]
        WeiXinGZ = 11,

        //[Union(SDKType = typeof(YhdSDK))]
        Yhd = 12,

        //[Union(SDKType = typeof(JDSDK))]
        JD = 13,

       //[Union(SDKType = typeof(YiXinGZSDK))]
        YiXinGZ = 14,

        //[Union(SDKType = typeof(YiXinSDK))]
        YiXin = 15,

        //[Union(SDKType = typeof(WeiXinSDK))]
        WeiXin = 16,

        //[Union(SDKType = typeof(yueyaCMSSDK))]
        yueyaCMS = 17
    }

    public class UnionCollection 
    {
        private static List<Object> _Items;

        public static List<Object> Items
        {
            get 
            {
                if (_Items == null)
                {
                    _Items = new List<object>();
                    _Items.Add(new { ID = -1, Name = "无", Desc = "" });
                    _Items.Add(new { ID = 0, Name = "百度", Desc = "" });
                    _Items.Add(new { ID = 1, Name = "豆瓣", Desc = "" });
                    _Items.Add(new { ID = 2, Name = "开心", Desc = "" });
                    _Items.Add(new { ID = 3, Name = "QQ企邮", Desc = "" });
                    _Items.Add(new { ID = 4, Name = "人人", Desc = "" });
                    _Items.Add(new { ID = 5, Name = "新浪博客", Desc = "" });
                    _Items.Add(new { ID = 6, Name = "搜狐", Desc = "" });
                    _Items.Add(new { ID = 7, Name = "T163", Desc = "" });
                    _Items.Add(new { ID = 8, Name = "淘宝", Desc = "" });
                    _Items.Add(new { ID = 9, Name = "腾讯微博", Desc = "" });
                    _Items.Add(new { ID = 10, Name = "新浪微博", Desc = "" });
                    _Items.Add(new { ID = 11, Name = "微信", Desc = "" });
                    _Items.Add(new { ID = 12, Name = "一号店", Desc = "" });
                    _Items.Add(new { ID = 13, Name = "京东", Desc = "" });
                    _Items.Add(new { ID = 14, Name = "易信", Desc = "" });
                    //_Items.Add(new { ID = 15, Name = "", Desc = "" });
                    //_Items.Add(new { ID = 16, Name = "", Desc = "" });
                    _Items.Add(new { ID = 17, Name = "玥雅CMS", Desc = "" });
                }
                return UnionCollection._Items; 
            }
        }

        
       
    }
}