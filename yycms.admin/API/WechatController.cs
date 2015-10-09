using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using yycms.admin.Models;
using yycms.entity;
using yycms.union.wechat;

namespace yycms.admin.API
{
    /// <summary>
    /// 微信
    /// </summary>
    [BasicAuthen]
    public class WechatController : BasicAPI
    {
        #region  微信列表
        /// <summary>
        /// 获取微信列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseData<yy_Fans_Wechat>> Get(RequestEntity value)
        {
            //查询的表名称
            Type Table = typeof(yy_Fans_Wechat);

            var FormData = await Request.Content.ReadAsAsync<Dictionary<String, String>>();

            #region where condition

            //筛选条件
            var Where = String.Empty;

            var WhereBuild = new List<string>();

            #region 微信标题
            if (!String.IsNullOrEmpty(value.Title))
            {
                WhereBuild.Add("Title like '%" + value.Title + "%'");
            }
            #endregion

            #region 微信分类
            if (value.TypeID > 0)
            {
                WhereBuild.Add("TypeIDs like '%," + value.TypeID + ",%'");
            }
            #endregion

            #region 根据时间过滤
            #region 大于等于 开始时间 && 小于等于 结束时间
            if (!String.IsNullOrEmpty(value.StartTime) && !String.IsNullOrEmpty(value.EndTime))
            {
                DateTime st, et;

                if (DateTime.TryParse(value.StartTime, out st) && DateTime.TryParse(value.EndTime, out et))
                {
                    WhereBuild.Add(" CreateDate >= '" + st.ToString("yyyy-MM-dd") + " 00:00:00' AND CreateDate <= '" + et.ToString("yyyy-MM-dd") + " 23:59:59'");
                }
            }
            #endregion
            #region 大于等于开始时间
            else if (!String.IsNullOrEmpty(value.StartTime))
            {
                DateTime st;
                if (DateTime.TryParse(value.StartTime, out st))
                {
                    WhereBuild.Add(" CreateDate >= '" + st.ToString("yyyy-MM-dd") + " 00:00:00'");
                }
            }
            #endregion
            #region 小于等于结束时间
            else if (!String.IsNullOrEmpty(value.EndTime))
            {
                DateTime et;
                if (DateTime.TryParse(value.EndTime, out et))
                {
                    WhereBuild.Add(" CreateDate <= '" + et.ToString("yyyy-MM-dd") + " 23:59:59'");
                }
            }
            #endregion
            #endregion

            if (WhereBuild.Count > 0)
            {
                Where = " WHERE " + String.Join(" AND ", WhereBuild);
            }
            #endregion

            #region OrderBy
            //排序规则
            String OrderBy = " ID DESC ";
            if (!String.IsNullOrEmpty(value.OrderBy))
            {
                OrderBy = " " + value.OrderBy + " " + (value.IsDesc ? "DESC" : "ASC");
            }
            #endregion

            #region 拼接sql语句
            String colsStr = " * ";
            #region  查询数据
            String QuertCMD = String.Format(@"SELECT TOP {0} * FROM (
                                SELECT ROW_NUMBER() OVER (ORDER BY {4}) AS RowNumber," + colsStr + @" FROM {2} WITH(NOLOCK) {3} 
                                ) A WHERE RowNumber > {0} * ({1}-1)", value.PageSize, value.PageIndex + 1, "[" + Table.Name + "]", Where, OrderBy);
            #endregion
            #region 查询总数
            String DataCountCMD = @"SELECT COUNT(1) FROM [" + Table.Name + "] WITH(NOLOCK) " + Where;
            #endregion
            #endregion

            #region 执行查询并返回数据
            var DataCount = DB.Database.SqlQuery<int>(DataCountCMD).FirstOrDefault();
            return new ResponseData<yy_Fans_Wechat>(value.PageSize,
                value.PageIndex,
                DataCount,
                (DataCount % value.PageSize == 0 ? DataCount / value.PageSize : DataCount / value.PageSize + 1),
                DB.Database.SqlQuery<yy_Fans_Wechat>(QuertCMD).ToList());
            #endregion
        }
        #endregion

        #region 微信详情
        /// <summary>
        /// 获取微信详情
        /// </summary>
        /// <param name="id">微信ID。</param>
        /// <returns></returns>
        [HttpGet]

        public yy_Banner Get(int id)
        {
            //微信详情
            return DB.yy_Banner.Find(id);
        }
        #endregion

        #region 刷新粉丝列表
        /// <summary>
        /// 重新从微信服务器拉取粉丝
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResponseItem RefreshFans()
        {
            MessageQueue_Send("WechatFans", User.ID.ToString());

            return new ResponseItem(0, String.Empty);
        }
        #endregion

        #region 根据OpenID发送消息
        /// <summary>
        /// 根据OpenID发送消息
        /// </summary>
        /// <param name="value">群发实体。</param>
        [HttpPost]
        public ResponseItem MessageByIDs(WechatModel value)
        {
            var plt = User_Platforms_Wechat();

            var f = new Fans(plt.Access_token);

            String errMsg = String.Empty;

            int errcode = 0;

            foreach (var v in value.IDs)
            {
                JObject code = null;

                if (value.Type.Equals("image"))
                {
                    code = f.SendByImage(v, value.MaterialID);
                }
                if (value.Type.Equals("news"))
                {
                    foreach(var vv in value.News)
                    {
                        if(string.IsNullOrEmpty(vv.url))
                        {
                            //根域名
                            vv.url = request.Url.Authority;
                        }
                    }

                    code = f.SendByNews(v, value.News);
                }
                else
                {
                    code = f.SendMsg(v, value.Message);
                }

              if (code.Value<int>("errcode") != 0)
                {
                    errcode = code.Value<int>("errcode");

                    errMsg += code.Value<String>("errmsg") + "，";
                }
            }

            return new ResponseItem(errcode, errMsg);
        }
        #endregion

        #region  新闻列表
        /// <summary>
        /// 获取新闻列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseData<FansNewsItem>> News(RequestEntity value)
        {
            //查询的表名称
            Type Table = typeof(yy_News);

            var FormData = await Request.Content.ReadAsAsync<Dictionary<String, String>>();

            #region where condition
            //筛选条件
            var Where = String.Empty;

            var WhereBuild = new List<string>();

            #region 新闻标题
            if (!String.IsNullOrEmpty(value.Title))
            {
                WhereBuild.Add("Title like '%" + value.Title + "%'");
            }
            #endregion

            #region 新闻分类
            if (value.TypeID > 0)
            {
                WhereBuild.Add("TypeIDs like '%," + value.TypeID + ",%'");
            }
            #endregion

            if (WhereBuild.Count > 0)
            {
                Where = " WHERE " + String.Join(" AND ", WhereBuild);
            }
            #endregion

            #region OrderBy
            //排序规则
            String OrderBy = " ID DESC ";
            if (!String.IsNullOrEmpty(value.OrderBy))
            {
                OrderBy = " " + value.OrderBy + " " + (value.IsDesc ? "DESC" : "ASC");
            }
            #endregion

            #region 拼接sql语句
            String colsStr = " ID,Title,Summary,LookCount,WechatNewsUrl,DefaultImg,CreateDate ";
            #region  查询数据
            String QuertCMD = String.Format(@"SELECT TOP {0} * FROM (
                                SELECT ROW_NUMBER() OVER (ORDER BY {4}) AS RowNumber," + colsStr + @" FROM {2} WITH(NOLOCK) {3} 
                                ) A WHERE RowNumber > {0} * ({1}-1)", value.PageSize, value.PageIndex + 1, "[" + Table.Name + "]", Where, OrderBy);
            #endregion
            #region 查询总数
            String DataCountCMD = @"SELECT COUNT(1) FROM [" + Table.Name + "] WITH(NOLOCK) " + Where;
            #endregion
            #endregion

            #region 执行查询并返回数据
            var DataCount = DB.Database.SqlQuery<int>(DataCountCMD).FirstOrDefault();
            return new ResponseData<FansNewsItem>(value.PageSize,
                value.PageIndex,
                DataCount,
                (DataCount % value.PageSize == 0 ? DataCount / value.PageSize : DataCount / value.PageSize + 1),
                DB.Database.SqlQuery<FansNewsItem>(QuertCMD).ToList());
            #endregion
        }
        #endregion
    }
}
