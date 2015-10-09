using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
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
    /// 页面分类
    /// </summary>
    [BasicAuthen]
    public class PageTypeController : BasicAPI
    {
        #region  页面分类列表
        /// <summary>
        /// 获取页面分类列表
        /// </summary>
        /// <returns>页面分类列表</returns>
        [HttpPost]
        public async Task<ResponseData<yy_Page_Type2>> Get(RequestEntity value)
        {
            //查询的表名称
            Type Table = typeof(yy_Page_Type);

            var FormData = await Request.Content.ReadAsAsync<Dictionary<String, String>>();

            #region where condition

            //筛选条件
            var Where = String.Empty;

            var WhereBuild = new List<string>();

            #region 页面标题
            if (!String.IsNullOrEmpty(value.Title))
            {
                WhereBuild.Add("Title like '%" + value.Title + "%'");
            }
            #endregion

            #region 页面分类
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
            return new ResponseData<yy_Page_Type2>(value.PageSize,
                value.PageIndex,
                DataCount,
                (DataCount % value.PageSize == 0 ? DataCount / value.PageSize : DataCount / value.PageSize + 1),
                DB.Database.SqlQuery<yy_Page_Type>(QuertCMD).ToList().Select(x => new yy_Page_Type2()
                {
                    ID = x.ID,
                    Author = x.Author,
                    CreateDate = x.CreateDate,
                    Email = x.Email,
                    //配合客户端angular输出，这里必须做处理
                    Images = x.Images.Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                     .Select(m => new yy_Page_Type2Image (){ src = m }).ToList(),
                    IsMaster = x.IsMaster,
                    Mobile = x.Mobile,
                    Name = x.Name,
                    Name_En = x.Name_En,
                    QQ = x.QQ,
                    Summary = x.Summary,
                    SupportPlatform = x.SupportPlatform,
                    UserID = x.UserID,
                    Version = x.Version,
                    Website = x.Website
                }).ToList());
            #endregion
        }
        #endregion

        #region 页面分类详情
        /// <summary>
        /// 获取页面分类详情
        /// </summary>
        /// <param name="id">页面分类ID。</param>
        /// <returns></returns>
        [HttpGet]
        public yy_Page_Type Get(int id)
        {
            return DB.yy_Page_Type.Find(id);
        }
        #endregion

        #region 添加页面分类
        /// <summary>
        /// 添加页面分类
        /// </summary>
        /// <param name="value">页面分类实体。</param>
        [HttpPost]
        public ResponseItem Post(yy_Page_Type value)
        {
            try
            {
                DB.yy_Page_Type.Add(value);
                DB.SaveChanges();
                return new ResponseItem(0, "添加页面分类成功。");
            }
            catch (Exception ex)
            {
                return new ResponseItem(2, ex.Message);
            }
        }
        #endregion

        #region 修改页面分类
        /// <summary>
        /// 修改页面分类
        /// </summary>
        /// <param name="value">页面分类实体。</param>
        [HttpPut]
        public ResponseItem Put(yy_Page_Type value)
        {
            var _Entity = DB.yy_Page_Type.Find(value.ID);

            if (_Entity != null)
            {
                _Entity.Images = value.Images;
                _Entity.IsMaster = value.IsMaster;
                _Entity.Mobile = value.Mobile;
                _Entity.Name = value.Name;
                _Entity.Name_En = value.Name_En;
                _Entity.QQ = value.QQ;
                _Entity.Summary = value.Summary;
                _Entity.SupportPlatform = value.SupportPlatform;
                _Entity.UserID = value.UserID;
                _Entity.Version = value.Version;
                _Entity.Website = value.Website;
                _Entity.Author = value.Author;
                _Entity.Email = value.Email;
                _Entity.CreateDate = value.CreateDate;
                DB.SaveChanges();
                return new ResponseItem(0, "");
            }

            return new ResponseItem(2, "不存在的页面分类。");
        }
        #endregion

        #region 删除页面分类
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">页面分类ID。</param>
        [HttpDelete]
        public ResponseItem Delete(int id)
        {
            var BasePath = ConfigurationManager.AppSettings["SkinBaseDirectory"];
            
            var Entity = DB.yy_Page_Type.Where(x => x.ID == id).FirstOrDefault();

            if (Entity != null)
            {
                var SkinPath = BasePath + Entity.Name_En + Entity.Version.ToString();

                if (Directory.Exists(SkinPath))
                {
                    Directory.Delete(SkinPath,true);
                } 
            }
            
            DB.Database.ExecuteSqlCommand("DELETE yy_Page_Type WHERE ID = " + id);
            DB.Database.ExecuteSqlCommand("DELETE FROM yy_Page WHERE TypeID = " + id);
            DB.Database.ExecuteSqlCommand("DELETE FROM yy_Page_Build_Task WHERE TypeID = " + id);
            DB.Database.ExecuteSqlCommand("DELETE FROM yy_Page_Build_Config WHERE PageTypeID = " + id);

            return new ResponseItem(0, "");
        }
        #endregion

        #region 根据ID批量删除页面分类
        /// <summary>
        /// 批量删除页面分类
        /// </summary>
        /// <param name="ids">页面分类ID集合，用英文逗号链接。</param>
        [HttpDelete]
        public ResponseItem DeleteByIDs(String ids)
        {
            var IDs = ids.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var v in IDs)
            {
                long id = 0;

                if (long.TryParse(v, out id))
                {
                    var Entity = DB.yy_Page_Type.Where(x => x.ID == id).FirstOrDefault();

                    if (Entity != null)
                    {
                        var SkinPath = Entity.Name_En + Entity.Version.ToString();

                        if (Directory.Exists(SkinPath))
                        {
                            Directory.Delete(SkinPath);
                        }
                    }

                    DB.Database.ExecuteSqlCommand("DELETE yy_Page_Type WHERE ID = " + id);
                    DB.Database.ExecuteSqlCommand("DELETE FROM yy_Page WHERE TypeID = " + id);
                    DB.Database.ExecuteSqlCommand("DELETE FROM yy_Page_Build_Task WHERE TypeID = " + id);
                    DB.Database.ExecuteSqlCommand("DELETE FROM yy_Page_Build_Config WHERE PageTypeID = " + id);
                }
            }
            return new ResponseItem(0, "");
        }
        #endregion
    }
}
