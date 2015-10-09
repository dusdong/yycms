using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace yycms.union.wechat
{
    public class POI:SDK
    {
        public POI(String _Accecc_Token)
        {
            base.Access_Token = _Accecc_Token;
        }

        #region 上传门店图片
        public String Logo_Add(String _ImagePath)
        {
            try
            {
                var ImageItem = Upload(Upload_Image, _ImagePath);

                if (ImageItem != null && ImageItem.HasValues)
                {
                    return ImageItem["url"].Value<String>();
                }
                else
                {
                    return String.Empty;
                }
            }
            catch
            {
                return String.Empty;
            }
        }
        #endregion

        #region 创建门店
        /// <summary>
        /// 创建门店
        /// </summary>
        /// <param name="sid">商户自己的 id，用于后续审核通过收到 poi_id 的通知时，做对应关系。请商户自己保证唯一识别性</param>
        /// <param name="business_name">门店名称（仅为商户名，如：国美、麦当劳，不应包含地区、店号等信息，错误示例：北京国美）</param>
        /// <param name="branch_name">分店名称（不应包含地区信息、不应与门店名重复，错误示例：北京王府井店）</param>
        /// <param name="province">门店所在的省份（直辖市填城市名,如：北京市）</param>
        /// <param name="city">门店所在的城市</param>
        /// <param name="district">门店所在地区</param>
        /// <param name="address">门店所在的详细街道地址（不要填写省市信息）</param>
        /// <param name="telephone">门店的电话（纯数字，区号、分机号均由“-”隔开）</param>
        /// <param name="categories">门店的类型（详细分类参见分类附表，不同级分类用“,”隔开，如：美食，川菜，火锅）</param>
        /// <param name="offset_type">坐标类型，1 为火星坐标（目前只能选 1）</param>
        /// <param name="longitude">门店所在地理位置的经度</param>
        /// <param name="latitude">门店所在地理位置的纬度（经纬度均为火星坐标，最好选用腾讯地图标记的坐标）</param>
        /// <param name="photo_list">图片列表，url 形式，可以有多张图片，尺寸为640*340px。必须为上一接口生成的 url</param>
        /// <param name="recommend">推荐品，餐厅可为推荐菜；酒店为推荐套房；景点为推荐游玩景点等，针对自己行业的推荐内容</param>
        /// <param name="special">特色服务，如免费 wifi，免费停车，送货上门等商户能提供的特色功能或服务</param>
        /// <param name="introduction">商户简介，主要介绍商户信息等</param>
        /// <param name="open_time">营业时间，24 小时制表示，用“-”连接，如8:00-20:00</param>
        /// <param name="avg_price">人均价格，大于 0 的整数</param>
        /// <returns></returns>
        public bool Add(
            long sid,
            String business_name,
            String branch_name,
            String province,
            String city,
            String district,
            String address,
            String telephone,
            String[] categories,
            int offset_type,
            String longitude,
            String latitude,
            String[] photo_list,
            String recommend,
            String special,
            String introduction,
            String open_time,
            String avg_price)
        {
            

            var ReqStr = JsonConvert.SerializeObject(
                new
                {
                    business = new
                    {
                        base_info = new
                        {
                            sid = sid,
                            business_name = business_name,
                            branch_name = branch_name,
                            province = province,
                            city = city,
                            district = district,
                            address = address,
                            telephone = telephone,
                            categories = categories,
                            offset_type = offset_type,
                            longitude = longitude,
                            latitude = latitude,
                            photo_list = photo_list,
                            recommend = recommend,
                            special = special,
                            introduction = introduction,
                            open_time = open_time,
                            avg_price = avg_price
                        }
                    }
                });

            var res = Request(POI_Add, ReqStr);

            if (res != null && res.HasValues && res["errcode"].Value<int>() == 0)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region 查询门店
        /// <summary>
        /// 查询门店列表
        /// 商户可以通过该接口，批量查询自己名下的门店 list，并获取已审核通过的 poi_id（审核中和审
        /// 核驳回的不返回 poi_id）、商户自身 sid 用于对应、商户名、分店名、地址字段
        /// available_state 门店是否可用状态。1 表示系统错误、2 表示审核中、3 审核通过、4 审核驳回。当该字段为 1、2、4 状态时，poi_id 为空
        /// </summary>
        /// <param name="begin"></param>
        /// <returns></returns>
        public JObject Get(long PageIndex)
        {
            var ReqStr = JsonConvert.SerializeObject(
               new
               {
                   begin = PageIndex * 50,
                   limit = 50
               });

            var res = Request(POI_Get, ReqStr);

            if (res != null && res.HasValues && res["errcode"].Value<int>() == 0)
            {
                return res;
            }

            return null;
        }
        #endregion

        #region 删除门店
        /// <summary>
        /// 查询门店列表
        /// 商户可以通过该接口，批量查询自己名下的门店 list，并获取已审核通过的 poi_id（审核中和审
        /// 核驳回的不返回 poi_id）、商户自身 sid 用于对应、商户名、分店名、地址字段
        /// available_state 门店是否可用状态。1 表示系统错误、2 表示审核中、3 审核通过、4 审核驳回。当该字段为 1、2、4 状态时，poi_id 为空
        /// </summary>
        /// <param name="poi_id">门店 ID</param>
        /// <returns></returns>
        public Boolean Delete(long poi_id)
        {
            var ReqStr = JsonConvert.SerializeObject(
               new
               {
                   poi_id = poi_id
               });

            var res = Request(POI_Delete, ReqStr);

            if (res != null && res.HasValues && res["errcode"].Value<int>() == 0)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region 修改门店
        /// <summary>
        /// 修改门店
        /// 商户可以通过该接口，修改门店的服务信息，包括：图片列表、营业时间、推荐、特色服务、简
        /// 介、人均价格、电话 7 个字段。目前基础字段包括（名称、坐标、地址等不可修改）
        /// </summary>
        /// <param name="poi_id">门店ID</param>
        /// <param name="telephone"></param>
        /// <param name="photo_list">图片列表</param>
        /// <param name="recommend">推荐</param>
        /// <param name="special">特色服务</param>
        /// <param name="introduction">简介</param>
        /// <param name="open_time">营业时间</param>
        /// <param name="avg_price">人均价格</param>
        /// <returns></returns>
        public Boolean Update(long poi_id, String telephone, String[] photo_list, String recommend,
            String special, String introduction, String open_time,String avg_price)
        {
            var ReqStr = JsonConvert.SerializeObject(
               new
               {
                   business = new
                   {
                       base_info = new
                       {
                           poi_id = poi_id,
                           telephone = telephone,
                           photo_list = photo_list,
                           recommend = recommend,
                           special = special,
                           introduction = introduction,
                           open_time = open_time,
                           avg_price = avg_price
                       }
                   }
               });

            var res = Request(POI_Update, ReqStr);

            if (res != null && res.HasValues && res["errcode"].Value<int>() == 0)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region 查询门店信息
        /// <summary>
        /// 查询门店信息
        /// 在审核通过并获取 poi_id 后，商户可以利用 poi_id，查询具体某条门店的信息。若在查询时，
        /// update_status 字段为 1，表明在 5 个工作日内曾用 update 接口修改过门店扩展字段，该扩展字段
        /// 为最新的修改字段，尚未经过审核采纳，因此不是最终结果。最终结果会在 5 个工作日内，最终
        /// 确认是否采纳，并前端生效（但该扩展字段的采纳过程不影响门店的可用性，即 available_state仍为审核通过状态）
        /// </summary>
        /// <param name="poi_id">门店 ID</param>
        /// <returns></returns>
        public Boolean Detail(String poi_id)
        {
            var ReqStr = JsonConvert.SerializeObject(
               new
               {
                   poi_id = poi_id
               });

            var res = Request(POI_Detail, ReqStr);

            if (res != null && res.HasValues && res["errcode"].Value<int>() == 0)
            {
                return true;
            }

            return false;
        }
        #endregion
    }
}
