using Newtonsoft.Json;
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

namespace yycms.admin.API
{
    /// <summary>
    /// 单页
    /// </summary>
    [BasicAuthen]
    public class SinglePageController : BasicAPI
    {
        #region  单页列表
        /// <summary>
        /// 获取单页列表
        /// </summary>
        /// <returns>单页列表</returns>
        [HttpPost]
        public async Task<ResponseData<NewsListItem>> Get(RequestEntity value)
        {
            //查询的表名称
            Type Table = typeof(yy_SinglePage);

            var FormData = await Request.Content.ReadAsAsync<Dictionary<String, String>>();

            #region where condition

            //筛选条件
            var Where = String.Empty;

            var WhereBuild = new List<string>();

            #region 单页标题
            if (!String.IsNullOrEmpty(value.Title))
            {
                WhereBuild.Add("Title like '%" + value.Title + "%'");
            }
            #endregion

            #region 单页分类
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

        #region 单页详情
        /// <summary>
        /// 获取单页详情
        /// </summary>
        /// <param name="id">单页ID。</param>
        /// <returns></returns>
        [HttpGet]

        public yy_SinglePage Get(int id)
        {
            //单页详情
            return DB.yy_SinglePage.Find(id);
        }
        #endregion

        #region 添加单页
        /// <summary>
        /// 添加单页
        /// </summary>
        /// <param name="value">单页对象。</param>
        [HttpPost]
        public ResponseItem Post(yy_SinglePage value)
        {
            try
            {
                DB.yy_SinglePage.Add(value);
                DB.SaveChanges();
                return new ResponseItem(0, "添加单页成功。");
            }
            catch (Exception ex)
            {
                return new ResponseItem(2, ex.Message);
            }
        }
        #endregion

        #region 修改单页
        /// <summary>
        /// 修改单页
        /// </summary>
        /// <param name="value">单页对象。</param>
        [HttpPut]
        public ResponseItem Put(yy_SinglePage value)
        {
            var _News = DB.yy_SinglePage.Find(value.ID);
            if (_News != null)
            {
                _News.IsShow = value.IsShow;
                _News.Title = value.Title;
                _News.KeyWords = value.KeyWords;
                _News.Summary = value.Summary;
                _News.Info = value.Info;
                _News.CreateDate = value.CreateDate;
                DB.SaveChanges();

                return new ResponseItem(0, "");
            }

            return new ResponseItem(2, "不存在的单页。");
        }
        #endregion

        #region 删除单页
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">单页ID。</param>
        [HttpDelete]
        public ResponseItem Delete(int id)
        {
            DB.Database.ExecuteSqlCommand("DELETE yy_SinglePage WHERE ID = " + id);
            return new ResponseItem(0, "");
        }
        #endregion

        #region 显示或隐藏单页
        /// <summary>
        /// 显示或隐藏单页
        /// </summary>
        /// <param name="value">单页对象。</param>
        /// <returns></returns>
        [HttpPut]
        public ResponseItem ShowHide(yy_SinglePage value)
        {
            var _News = DB.yy_SinglePage.Find(value.ID);
            if (_News != null)
            {
                _News.IsShow = value.IsShow;
                DB.SaveChanges();

                return new ResponseItem(0, "");
            }

            return new ResponseItem(2, "不存在的单页。");
        }
        #endregion

        #region 根据ID批量删除单页
        /// <summary>
        /// 批量删除单页
        /// </summary>
        /// <param name="ids">单页ID集合，用英文逗号链接。</param>
        [HttpDelete]
        public ResponseItem DeleteByIDs(String ids)
        {
            var IDs = ids.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var v in IDs)
            {
                long id = 0;

                if (long.TryParse(v, out id))
                {
                    DB.Database.ExecuteSqlCommand("DELETE yy_SinglePage WHERE ID = " + id);
                }
            }
            return new ResponseItem(0, "");
        }
        #endregion

        #region 根据ID批量显示单页
        /// <summary>
        /// 批量显示单页
        /// </summary>
        /// <param name="ids">单页ID集合，用英文逗号链接。</param>
        [HttpPut]
        public ResponseItem ShowByIDs(String ids)
        {
            var IDs = ids.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var v in IDs)
            {
                long id = 0;

                if (long.TryParse(v, out id))
                {
                    DB.Database.ExecuteSqlCommand("UPDATE yy_SinglePage SET IsShow = 1 WHERE ID = " + id);
                }
            }
            return new ResponseItem(0, "");
        }
        #endregion

        #region 根据ID批量隐藏单页
        /// <summary>
        /// 批量隐藏单页
        /// </summary>
        /// <param name="ids">单页ID集合，用英文逗号链接。</param>
        [HttpPut]
        public ResponseItem HideByIDs(String ids)
        {
            var IDs = ids.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var v in IDs)
            {
                long id = 0;

                if (long.TryParse(v, out id))
                {
                    DB.Database.ExecuteSqlCommand("UPDATE yy_SinglePage SET IsShow = 0 WHERE ID = " + id);
                }
            }
            return new ResponseItem(0, "");
        }
        #endregion
    }
}
