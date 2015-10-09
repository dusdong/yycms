using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using yycms.admin.Models;
using yycms.entity;

namespace yycms.admin.API
{
    /// <summary>
    /// 新闻
    /// </summary>
    [BasicAuthen]
    public class NewsController : BasicAPI
    {
        #region  新闻列表
        /// <summary>
        /// 获取新闻列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ResponseData<NewsListItem>> Get(RequestEntity value)
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
            String colsStr = " ID,Title,KeyWords,LookCount,IsShow,CreateDate ";
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
            return new ResponseData<NewsListItem>(value.PageSize,
                value.PageIndex,
                DataCount,
                (DataCount % value.PageSize == 0 ? DataCount / value.PageSize : DataCount / value.PageSize + 1),
                DB.Database.SqlQuery<NewsListItem>(QuertCMD).ToList());
            #endregion
        }
        #endregion

        #region 新闻详情
        /// <summary>
        /// 获取新闻详情
        /// </summary>
        /// <param name="id">新闻ID。</param>
        /// <returns></returns>
        [HttpGet]

        public yy_News Get(int id)
        {
            //新闻详情
            return DB.yy_News.Find(id);
        }
        #endregion

        #region 添加新闻
        /// <summary>
        /// 添加新闻
        /// </summary>
        /// <param name="value">新闻实体。</param>
        [HttpPost]
        public ResponseItem Post(yy_News value)
        {
            try
            {
                value.Info = KeywordReplace.Excute(DB, value.Info, 1, value.TypeIDs);

                DB.yy_News.Add(value);

                DB.SaveChanges();

                //同步微信
                if (value.TargetPlatforms.Contains(",1,"))
                {
                    MessageQueue_Send("WechatSync", value.ID.ToString());
                }

                return new ResponseItem (0, "添加新闻成功。");
            }
            catch (Exception ex)
            {
                return new ResponseItem(2, ex.Message);
            }
        }
        #endregion

        #region 修改新闻
        /// <summary>
        /// 修改新闻
        /// </summary>
        /// <param name="value">新闻实体。</param>
        [HttpPut]
        public ResponseItem Put(yy_News value)
        {
            var _Entity = DB.yy_News.Find(value.ID);

            if (_Entity != null)
            {
                _Entity.IsShow = value.IsShow;
                _Entity.ShowIndex = value.ShowIndex;
                _Entity.Title = value.Title;
                _Entity.KeyWords = value.KeyWords;
                _Entity.Summary = value.Summary;
                _Entity.TypeIDs = value.TypeIDs;

                if (!_Entity.Info.Equals(value.Info))
                {
                    value.Info = KeywordReplace.Excute(DB, value.Info, 1, value.TypeIDs);
                }

                _Entity.Info = value.Info;
                _Entity.TargetPlatforms = value.TargetPlatforms;
                _Entity.CanReply = value.CanReply;
                _Entity.CreateDate = value.CreateDate;
                DB.SaveChanges();

                return new ResponseItem(0, "");
            }

            return new ResponseItem(2, "不存在的新闻。");
        }
        #endregion

        #region 删除新闻
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">新闻ID。</param>
        [HttpDelete]
        public ResponseItem Delete(int id)
        {
            DB.Database.ExecuteSqlCommand("DELETE yy_News WHERE ID = " + id);

            return new ResponseItem(0, "");
        }
        #endregion

        #region 显示或隐藏新闻
        /// <summary>
        /// 显示或隐藏新闻
        /// </summary>
        /// <param name="value">新闻对象。</param>
        /// <returns></returns>
        [HttpPut]
        public ResponseItem ShowHide(yy_News value)
        {
            var _News = DB.yy_News.Find(value.ID);
            if (_News != null)
            {
                _News.IsShow = value.IsShow;
                DB.SaveChanges();

                return new ResponseItem(0, "");
            }

            return new ResponseItem(2, "不存在的新闻。");
        }
        #endregion

        #region 根据ID批量删除新闻
        /// <summary>
        /// 批量删除新闻
        /// </summary>
        /// <param name="ids">新闻ID集合，用英文逗号链接。</param>
        [HttpDelete]
        public ResponseItem DeleteByIDs(String ids)
        {
            var IDs = ids.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var v in IDs)
            {
                long id = 0;

                if (long.TryParse(v, out id))
                {
                    DB.Database.ExecuteSqlCommand("DELETE yy_News WHERE ID = " + id);
                }
            }
            return new ResponseItem(0, "");
        }
        #endregion

        #region 根据ID批量显示新闻
        /// <summary>
        /// 批量显示新闻
        /// </summary>
        /// <param name="ids">新闻ID集合，用英文逗号链接。</param>
        [HttpPut]
        public ResponseItem ShowByIDs(String ids)
        {
            var IDs = ids.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var v in IDs)
            {
                long id = 0;

                if (long.TryParse(v, out id))
                {
                    DB.Database.ExecuteSqlCommand("UPDATE yy_News SET IsShow = 1 WHERE ID = " + id);
                }
            }
            return new ResponseItem(0, "");
        }
        #endregion

        #region 根据ID批量隐藏新闻
        /// <summary>
        /// 批量隐藏新闻
        /// </summary>
        /// <param name="ids">新闻ID集合，用英文逗号链接。</param>
        [HttpPut]
        public ResponseItem HideByIDs(String ids)
        {
            var IDs = ids.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var v in IDs)
            {
                long id = 0;

                if (long.TryParse(v, out id))
                {
                    DB.Database.ExecuteSqlCommand("UPDATE yy_News SET IsShow = 0 WHERE ID = " + id);
                }
            }
            return new ResponseItem(0, "");
        }
        #endregion
    }
}
