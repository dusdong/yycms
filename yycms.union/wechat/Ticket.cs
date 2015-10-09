using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yycms.union.wechat
{
    public class Ticket : SDK
    {
        public Ticket(String _Accecc_Token)
        {
            base.Access_Token = _Accecc_Token;
        }

        #region 核销卡券
        /// <summary>
        /// 消耗卡券 code 接口是核销卡券的唯一接口，仅支持核销有效期内的卡券，否则会返回错误码
        ///invalid time。
        ///自定义 code（use_custom_code 为 true）的优惠券，在 code 被核销时，必须调用此
        ///接口。用于将用户客户端的 code 状态变更。自定义 code 的卡券调用接口时， post 数据中
        ///需包含 card_id，非自定义 code 不需上报。
        /// </summary>
        /// <param name="_card_id">card_id</param>
        /// <returns></returns>
        public String Consume(String _code)
        {
            var ReqStr = JsonConvert.SerializeObject(
               new
               {
                   code = _code
               });

            var res = Request(ticket_consume, ReqStr);

            if (res != null && res.HasValues && (
                res["errcode"].Value<int>() == 0
                ||
                res["errcode"].Value<int>() == 40099
                ))
            {
                return String.Empty;
            }

            return JsonConvert.SerializeObject(res);
        }
        #endregion

        #region 新增卡券
        #region 添加团购券
        /// <summary>
        /// 添加团购券
        /// </summary>
        /// <param name="logo_url">卡券的商户logo，建议像素为300*300。</param>
        /// <param name="brand_name">商户名字,字数上限为12个汉字。</param>
        /// <param name="code_type">Code展示类型，"CODE_TYPE_TEXT"，文本；"CODE_TYPE_BARCODE"，一维码 ；"CODE_TYPE_QRCODE"，二维码；"CODE_TYPE_ONLY_QRCODE",二维码无code显示；"CODE_TYPE_ONLY_BARCODE",一维码无code显示；</param>
        /// <param name="title">卡券名，字数上限为9个汉字。(建议涵盖卡券属性、服务及金额)。</param>
        /// <param name="sub_title">券名，字数上限为18个汉字。</param>
        /// <param name="color">券颜色。按色彩规范标注填写Color010-Color100。</param>
        /// <param name="notice">卡券使用提醒，字数上限为16个汉字。</param>
        /// <param name="service_phone">客服电话。</param>
        /// <param name="description">卡券使用说明，字数上限为1024个汉字。</param>
        /// <param name="type">使用时间的类型，仅支持填写1或2。1为固定日期区间，2为固定时长（自领取后按天算）。</param>
        /// <param name="begin_timestamp">type为1时专用，表示起用时间。从1970年1月1日00:00:00至起用时间的秒数，最终需转换为字符串形态传入。（东八区时间，单位为秒）</param>
        /// <param name="end_timestamp">type为1时专用，表示结束时间，建议设置为截止日期的23:59:59过期。（东八区时间，单位为秒）</param>
        /// <param name="fixed_term">type为2时专用，表示自领取后多少天内有效，领取后当天有效填写0。（单位为天）</param>
        /// <param name="fixed_begin_term">type为2时专用，表示自领取后多少天开始生效。（单位为天）</param>
        /// <param name="quantity">卡券库存的数量，不支持填写0，上限为100000000。</param>
        /// <param name="get_limit">每人可领券的数量限制。</param>
        /// <param name="use_custom_code">是否自定义Code码。填写true或false，默认为false。通常自有优惠码系统的开发者选择自定义Code码，在卡券投放时带入。</param>
        /// <param name="bind_openid">是否指定用户领取，填写true或false。默认为false。</param>
        /// <param name="can_share">卡券领取页面是否可分享。</param>
        /// <param name="can_give_friend">卡券是否可转赠。</param>
        /// <param name="location_id_list">门店位置ID。调用POI门店管理接口获取门店位置ID。</param>
        /// <param name="custom_url_name">自定义跳转外链的入口名字。</param>
        /// <param name="custom_url">自定义跳转的URL。</param>
        /// <param name="custom_url_sub_title">显示在入口右侧的提示语。</param>
        /// <param name="promotion_url_name">营销场景的自定义入口名称。</param>
        /// <param name="promotion_url">入口跳转外链的地址链接</param>
        /// <param name="source">第三方来源名，例如同程旅游、大众点评。</param>
        /// <param name="deal_detail">团购券专用，团购详情。</param>
        /// <returns></returns>
        public String Add_Groupon(String logo_url,
            String brand_name,
            String code_type,
            String title,
            String sub_title,
            String color,
            String notice,
            String service_phone,
            String description,
            int type,
            long begin_timestamp,
            long end_timestamp,
            int fixed_term,
            int fixed_begin_term,
            int quantity,
            int get_limit,
            bool use_custom_code,
            bool bind_openid,
            bool can_share,
            bool can_give_friend,
            int[] location_id_list,
            String custom_url_name,
            String custom_url,
            String custom_url_sub_title,
            String promotion_url_name,
            String promotion_url,
            String source,
            String deal_detail)
        #endregion
        {
            var _logoResult = Upload(Upload_Image, logo_url);

            if (_logoResult != null && _logoResult.HasValues)
            {
                logo_url = _logoResult["url"].Value<String>();
            }

            #region 请求
            var ReqObject = JsonConvert.DeserializeObject<JObject>(
                         JsonConvert.SerializeObject(
                         new

                         {
                             card = new
                             {
                                 card_type = "GROUPON",
                                 groupon = new
                                 {
                                     base_info = new
                                     {
                                         logo_url = logo_url,
                                         brand_name = brand_name,
                                         code_type = code_type,
                                         title = title,
                                         sub_title = sub_title,
                                         color = color,
                                         notice = notice,
                                         service_phone = service_phone,
                                         description = description,
                                         date_info = new
                                         {
                                             type = type,
                                         },
                                         sku = new
                                         {
                                             quantity = quantity
                                         },
                                         get_limit = get_limit,
                                         use_custom_code = use_custom_code,
                                         bind_openid = bind_openid,
                                         can_share = can_share,
                                         can_give_friend = can_give_friend,
                                         location_id_list = location_id_list,
                                         custom_url_name = custom_url_name,
                                         custom_url = custom_url,
                                         custom_url_sub_title = custom_url_sub_title,
                                         promotion_url_name = promotion_url_name,
                                         promotion_url = promotion_url,
                                         source = source,
                                     },
                                     deal_detail = deal_detail
                                 }
                             }
                         }
                         ));

            if (type == 1)
            {
                ReqObject["card"]["groupon"]["base_info"]["date_info"]["begin_timestamp"] = begin_timestamp;
                ReqObject["card"]["groupon"]["base_info"]["date_info"]["end_timestamp"] = end_timestamp;
            }
            else if (type == 2)
            {
                ReqObject["card"]["groupon"]["base_info"]["date_info"]["fixed_term"] = fixed_term;
                //领取后多少天开始生效
                //fixed_begin_term = fixed_begin_term
            }

            var ReqStr = JsonConvert.SerializeObject(ReqObject);
            #endregion

            var addResult = Request(ticket_create, ReqStr);

            if (addResult != null && addResult.HasValues)
            {
                if (addResult["errcode"].Value<int>() == 0)
                {
                    return addResult["card_id"].Value<String>();
                }
            }

            return String.Empty;
        }

        #region 添加代金券
        /// <summary>
        /// 添加代金券
        /// </summary>
        /// <param name="logo_url">卡券的商户logo，建议像素为300*300。</param>
        /// <param name="brand_name">商户名字,字数上限为12个汉字。</param>
        /// <param name="code_type">Code展示类型，"CODE_TYPE_TEXT"，文本；"CODE_TYPE_BARCODE"，一维码 ；"CODE_TYPE_QRCODE"，二维码；"CODE_TYPE_ONLY_QRCODE",二维码无code显示；"CODE_TYPE_ONLY_BARCODE",一维码无code显示；</param>
        /// <param name="title">卡券名，字数上限为9个汉字。(建议涵盖卡券属性、服务及金额)。</param>
        /// <param name="sub_title">券名，字数上限为18个汉字。</param>
        /// <param name="color">券颜色。按色彩规范标注填写Color010-Color100。</param>
        /// <param name="notice">卡券使用提醒，字数上限为16个汉字。</param>
        /// <param name="service_phone">客服电话。</param>
        /// <param name="description">卡券使用说明，字数上限为1024个汉字。</param>
        /// <param name="type">使用时间的类型，仅支持填写1或2。1为固定日期区间，2为固定时长（自领取后按天算）。</param>
        /// <param name="begin_timestamp">type为1时专用，表示起用时间。从1970年1月1日00:00:00至起用时间的秒数，最终需转换为字符串形态传入。（东八区时间，单位为秒）</param>
        /// <param name="end_timestamp">type为1时专用，表示结束时间，建议设置为截止日期的23:59:59过期。（东八区时间，单位为秒）</param>
        /// <param name="fixed_term">type为2时专用，表示自领取后多少天内有效，领取后当天有效填写0。（单位为天）</param>
        /// <param name="fixed_begin_term">type为2时专用，表示自领取后多少天开始生效。（单位为天）</param>
        /// <param name="quantity">卡券库存的数量，不支持填写0，上限为100000000。</param>
        /// <param name="get_limit">每人可领券的数量限制。</param>
        /// <param name="use_custom_code">是否自定义Code码。填写true或false，默认为false。通常自有优惠码系统的开发者选择自定义Code码，在卡券投放时带入。</param>
        /// <param name="bind_openid">是否指定用户领取，填写true或false。默认为false。</param>
        /// <param name="can_share">卡券领取页面是否可分享。</param>
        /// <param name="can_give_friend">卡券是否可转赠。</param>
        /// <param name="location_id_list">门店位置ID。调用POI门店管理接口获取门店位置ID。</param>
        /// <param name="custom_url_name">自定义跳转外链的入口名字。</param>
        /// <param name="custom_url">自定义跳转的URL。</param>
        /// <param name="custom_url_sub_title">显示在入口右侧的提示语。</param>
        /// <param name="promotion_url_name">营销场景的自定义入口名称。</param>
        /// <param name="promotion_url">入口跳转外链的地址链接</param>
        /// <param name="source">第三方来源名，例如同程旅游、大众点评。</param>
        /// <param name="least_cost">表示起用金额。（单位为分）</param>
        /// <param name="reduce_cost">表示减免金额。（单位为分）</param>
        /// <returns></returns>
        public String Add_Cash(String logo_url,
            String brand_name,
            String code_type,
            String title,
            String sub_title,
            String color,
            String notice,
            String service_phone,
            String description,
            int type,
            long begin_timestamp,
            long end_timestamp,
            int fixed_term,
            int fixed_begin_term,
            int quantity,
            int get_limit,
            bool use_custom_code,
            bool bind_openid,
            bool can_share,
            bool can_give_friend,
            int[] location_id_list,
            String custom_url_name,
            String custom_url,
            String custom_url_sub_title,
            String promotion_url_name,
            String promotion_url,
            String source,
            long least_cost,
            long reduce_cost)
        #endregion
        {
            var _logoResult = Upload(Upload_Image, logo_url);

            if (_logoResult != null && _logoResult.HasValues)
            {
                logo_url = _logoResult["url"].Value<String>();
            }

            #region 请求
            var ReqObject = JsonConvert.DeserializeObject<JObject>(
                         JsonConvert.SerializeObject(
                         new

                         {
                             card = new
                             {
                                 card_type = "CASH",
                                 cash = new
                                 {
                                     base_info = new
                                     {
                                         logo_url = logo_url,
                                         brand_name = brand_name,
                                         code_type = code_type,
                                         title = title,
                                         sub_title = sub_title,
                                         color = color,
                                         notice = notice,
                                         service_phone = service_phone,
                                         description = description,
                                         date_info = new
                                         {
                                             type = type,
                                         },
                                         sku = new
                                         {
                                             quantity = quantity
                                         },
                                         get_limit = get_limit,
                                         use_custom_code = use_custom_code,
                                         bind_openid = bind_openid,
                                         can_share = can_share,
                                         can_give_friend = can_give_friend,
                                         location_id_list = location_id_list,
                                         custom_url_name = custom_url_name,
                                         custom_url = custom_url,
                                         custom_url_sub_title = custom_url_sub_title,
                                         promotion_url_name = promotion_url_name,
                                         promotion_url = promotion_url,
                                         source = source,
                                     },
                                     least_cost = least_cost,
                                     reduce_cost = reduce_cost
                                 }
                             }
                         }
                         ));

            if (type == 1)
            {
                ReqObject["card"]["cash"]["base_info"]["date_info"]["begin_timestamp"] = begin_timestamp;
                ReqObject["card"]["cash"]["base_info"]["date_info"]["end_timestamp"] = end_timestamp;
            }
            else if (type == 2)
            {
                ReqObject["card"]["cash"]["base_info"]["date_info"]["fixed_term"] = fixed_term;
                //领取后多少天开始生效
                //fixed_begin_term = fixed_begin_term
            }

            var ReqStr = JsonConvert.SerializeObject(ReqObject);
            #endregion

            var addResult = Request(ticket_create, ReqStr);

            if (addResult != null && addResult.HasValues)
            {
                if (addResult["errcode"].Value<int>() == 0)
                {
                    return addResult["card_id"].Value<String>();
                }
            }

            return String.Empty;
        }

        #region 添加折扣券
        /// <summary>
        /// 添加折扣券
        /// </summary>
        /// <param name="logo_url">卡券的商户logo，建议像素为300*300。</param>
        /// <param name="brand_name">商户名字,字数上限为12个汉字。</param>
        /// <param name="code_type">Code展示类型，"CODE_TYPE_TEXT"，文本；"CODE_TYPE_BARCODE"，一维码 ；"CODE_TYPE_QRCODE"，二维码；"CODE_TYPE_ONLY_QRCODE",二维码无code显示；"CODE_TYPE_ONLY_BARCODE",一维码无code显示；</param>
        /// <param name="title">卡券名，字数上限为9个汉字。(建议涵盖卡券属性、服务及金额)。</param>
        /// <param name="sub_title">券名，字数上限为18个汉字。</param>
        /// <param name="color">券颜色。按色彩规范标注填写Color010-Color100。</param>
        /// <param name="notice">卡券使用提醒，字数上限为16个汉字。</param>
        /// <param name="service_phone">客服电话。</param>
        /// <param name="description">卡券使用说明，字数上限为1024个汉字。</param>
        /// <param name="type">使用时间的类型，仅支持填写1或2。1为固定日期区间，2为固定时长（自领取后按天算）。</param>
        /// <param name="begin_timestamp">type为1时专用，表示起用时间。从1970年1月1日00:00:00至起用时间的秒数，最终需转换为字符串形态传入。（东八区时间，单位为秒）</param>
        /// <param name="end_timestamp">type为1时专用，表示结束时间，建议设置为截止日期的23:59:59过期。（东八区时间，单位为秒）</param>
        /// <param name="fixed_term">type为2时专用，表示自领取后多少天内有效，领取后当天有效填写0。（单位为天）</param>
        /// <param name="fixed_begin_term">type为2时专用，表示自领取后多少天开始生效。（单位为天）</param>
        /// <param name="quantity">卡券库存的数量，不支持填写0，上限为100000000。</param>
        /// <param name="get_limit">每人可领券的数量限制。</param>
        /// <param name="use_custom_code">是否自定义Code码。填写true或false，默认为false。通常自有优惠码系统的开发者选择自定义Code码，在卡券投放时带入。</param>
        /// <param name="bind_openid">是否指定用户领取，填写true或false。默认为false。</param>
        /// <param name="can_share">卡券领取页面是否可分享。</param>
        /// <param name="can_give_friend">卡券是否可转赠。</param>
        /// <param name="location_id_list">门店位置ID。调用POI门店管理接口获取门店位置ID。</param>
        /// <param name="custom_url_name">自定义跳转外链的入口名字。</param>
        /// <param name="custom_url">自定义跳转的URL。</param>
        /// <param name="custom_url_sub_title">显示在入口右侧的提示语。</param>
        /// <param name="promotion_url_name">营销场景的自定义入口名称。</param>
        /// <param name="promotion_url">入口跳转外链的地址链接</param>
        /// <param name="source">第三方来源名，例如同程旅游、大众点评。</param>
        /// <param name="discount">表示打折额度（百分比）。填30就是七折。</param>
        /// <returns></returns>
        public String Add_Discount(String logo_url,
            String brand_name,
            String code_type,
            String title,
            String sub_title,
            String color,
            String notice,
            String service_phone,
            String description,
            int type,
            long begin_timestamp,
            long end_timestamp,
            int fixed_term,
            int fixed_begin_term,
            int quantity,
            int get_limit,
            bool use_custom_code,
            bool bind_openid,
            bool can_share,
            bool can_give_friend,
            int[] location_id_list,
            String custom_url_name,
            String custom_url,
            String custom_url_sub_title,
            String promotion_url_name,
            String promotion_url,
            String source,
            int discount)
        #endregion
        {
            var _logoResult = Upload(Upload_Image, logo_url);

            if (_logoResult != null && _logoResult.HasValues)
            {
                logo_url = _logoResult["url"].Value<String>();
            }

            #region 请求
            var ReqObject = JsonConvert.DeserializeObject<JObject>(
                         JsonConvert.SerializeObject(
                         new

                         {
                             card = new
                             {
                                 card_type = "DISCOUNT",
                                 discount = new
                                 {
                                     base_info = new
                                     {
                                         logo_url = logo_url,
                                         brand_name = brand_name,
                                         code_type = code_type,
                                         title = title,
                                         sub_title = sub_title,
                                         color = color,
                                         notice = notice,
                                         service_phone = service_phone,
                                         description = description,
                                         date_info = new
                                         {
                                             type = type,
                                         },
                                         sku = new
                                         {
                                             quantity = quantity
                                         },
                                         get_limit = get_limit,
                                         use_custom_code = use_custom_code,
                                         bind_openid = bind_openid,
                                         can_share = can_share,
                                         can_give_friend = can_give_friend,
                                         location_id_list = location_id_list,
                                         custom_url_name = custom_url_name,
                                         custom_url = custom_url,
                                         custom_url_sub_title = custom_url_sub_title,
                                         promotion_url_name = promotion_url_name,
                                         promotion_url = promotion_url,
                                         source = source,
                                     },
                                     discount = discount
                                 }
                             }
                         }
                         ));

            if (type == 1)
            {
                ReqObject["card"]["discount"]["base_info"]["date_info"]["begin_timestamp"] = begin_timestamp;
                ReqObject["card"]["discount"]["base_info"]["date_info"]["end_timestamp"] = end_timestamp;
            }
            else if (type == 2)
            {
                ReqObject["card"]["discount"]["base_info"]["date_info"]["fixed_term"] = fixed_term;
                //领取后多少天开始生效
                //fixed_begin_term = fixed_begin_term
            }

            var ReqStr = JsonConvert.SerializeObject(ReqObject);
            #endregion

            var addResult = Request(ticket_create, ReqStr);

            if (addResult != null && addResult.HasValues)
            {
                if (addResult["errcode"].Value<int>() == 0)
                {
                    return addResult["card_id"].Value<String>();
                }
            }

            return String.Empty;
        }

        #region 添加礼品券
        /// <summary>
        /// 添加礼品券
        /// </summary>
        /// <param name="logo_url">卡券的商户logo，建议像素为300*300。</param>
        /// <param name="brand_name">商户名字,字数上限为12个汉字。</param>
        /// <param name="code_type">Code展示类型，"CODE_TYPE_TEXT"，文本；"CODE_TYPE_BARCODE"，一维码 ；"CODE_TYPE_QRCODE"，二维码；"CODE_TYPE_ONLY_QRCODE",二维码无code显示；"CODE_TYPE_ONLY_BARCODE",一维码无code显示；</param>
        /// <param name="title">卡券名，字数上限为9个汉字。(建议涵盖卡券属性、服务及金额)。</param>
        /// <param name="sub_title">券名，字数上限为18个汉字。</param>
        /// <param name="color">券颜色。按色彩规范标注填写Color010-Color100。</param>
        /// <param name="notice">卡券使用提醒，字数上限为16个汉字。</param>
        /// <param name="service_phone">客服电话。</param>
        /// <param name="description">卡券使用说明，字数上限为1024个汉字。</param>
        /// <param name="type">使用时间的类型，仅支持填写1或2。1为固定日期区间，2为固定时长（自领取后按天算）。</param>
        /// <param name="begin_timestamp">type为1时专用，表示起用时间。从1970年1月1日00:00:00至起用时间的秒数，最终需转换为字符串形态传入。（东八区时间，单位为秒）</param>
        /// <param name="end_timestamp">type为1时专用，表示结束时间，建议设置为截止日期的23:59:59过期。（东八区时间，单位为秒）</param>
        /// <param name="fixed_term">type为2时专用，表示自领取后多少天内有效，领取后当天有效填写0。（单位为天）</param>
        /// <param name="fixed_begin_term">type为2时专用，表示自领取后多少天开始生效。（单位为天）</param>
        /// <param name="quantity">卡券库存的数量，不支持填写0，上限为100000000。</param>
        /// <param name="get_limit">每人可领券的数量限制。</param>
        /// <param name="use_custom_code">是否自定义Code码。填写true或false，默认为false。通常自有优惠码系统的开发者选择自定义Code码，在卡券投放时带入。</param>
        /// <param name="bind_openid">是否指定用户领取，填写true或false。默认为false。</param>
        /// <param name="can_share">卡券领取页面是否可分享。</param>
        /// <param name="can_give_friend">卡券是否可转赠。</param>
        /// <param name="location_id_list">门店位置ID。调用POI门店管理接口获取门店位置ID。</param>
        /// <param name="custom_url_name">自定义跳转外链的入口名字。</param>
        /// <param name="custom_url">自定义跳转的URL。</param>
        /// <param name="custom_url_sub_title">显示在入口右侧的提示语。</param>
        /// <param name="promotion_url_name">营销场景的自定义入口名称。</param>
        /// <param name="promotion_url">入口跳转外链的地址链接</param>
        /// <param name="source">第三方来源名，例如同程旅游、大众点评。</param>
        /// <param name="gift">填写礼品的名称。</param>
        /// <returns></returns>
        public String Add_Gift(String logo_url,
            String brand_name,
            String code_type,
            String title,
            String sub_title,
            String color,
            String notice,
            String service_phone,
            String description,
            int type,
            long begin_timestamp,
            long end_timestamp,
            int fixed_term,
            int fixed_begin_term,
            int quantity,
            int get_limit,
            bool use_custom_code,
            bool bind_openid,
            bool can_share,
            bool can_give_friend,
            int[] location_id_list,
            String custom_url_name,
            String custom_url,
            String custom_url_sub_title,
            String promotion_url_name,
            String promotion_url,
            String source,
            String gift)
        #endregion
        {
            var _logoResult = Upload(Upload_Image, logo_url);

            if (_logoResult != null && _logoResult.HasValues)
            {
                logo_url = _logoResult["url"].Value<String>();
            }

            #region 请求
            var ReqObject = JsonConvert.DeserializeObject<JObject>(
                         JsonConvert.SerializeObject(
                         new

                         {
                             card = new
                             {
                                 card_type = "GIFT",
                                 gift = new
                                 {
                                     base_info = new
                                     {
                                         logo_url = logo_url,
                                         brand_name = brand_name,
                                         code_type = code_type,
                                         title = title,
                                         sub_title = sub_title,
                                         color = color,
                                         notice = notice,
                                         service_phone = service_phone,
                                         description = description,
                                         date_info = new
                                         {
                                             type = type,
                                         },
                                         sku = new
                                         {
                                             quantity = quantity
                                         },
                                         get_limit = get_limit,
                                         use_custom_code = use_custom_code,
                                         bind_openid = bind_openid,
                                         can_share = can_share,
                                         can_give_friend = can_give_friend,
                                         location_id_list = location_id_list,
                                         custom_url_name = custom_url_name,
                                         custom_url = custom_url,
                                         custom_url_sub_title = custom_url_sub_title,
                                         promotion_url_name = promotion_url_name,
                                         promotion_url = promotion_url,
                                         source = source,
                                     },
                                     gift = gift
                                 }
                             }
                         }
                         ));

            if (type == 1)
            {
                ReqObject["card"]["gift"]["base_info"]["date_info"]["begin_timestamp"] = begin_timestamp;
                ReqObject["card"]["gift"]["base_info"]["date_info"]["end_timestamp"] = end_timestamp;
            }
            else if (type == 2)
            {
                ReqObject["card"]["gift"]["base_info"]["date_info"]["fixed_term"] = fixed_term;
                //领取后多少天开始生效
                //fixed_begin_term = fixed_begin_term
            }

            var ReqStr = JsonConvert.SerializeObject(ReqObject);
            #endregion

            var addResult = Request(ticket_create, ReqStr);

            if (addResult != null && addResult.HasValues)
            {
                if (addResult["errcode"].Value<int>() == 0)
                {
                    return addResult["card_id"].Value<String>();
                }
            }

            return String.Empty;
        }

        #region 添加优惠券
        /// <summary>
        /// 添加优惠券
        /// </summary>
        /// <param name="logo_url">卡券的商户logo，建议像素为300*300。</param>
        /// <param name="brand_name">商户名字,字数上限为12个汉字。</param>
        /// <param name="code_type">Code展示类型，"CODE_TYPE_TEXT"，文本；"CODE_TYPE_BARCODE"，一维码 ；"CODE_TYPE_QRCODE"，二维码；"CODE_TYPE_ONLY_QRCODE",二维码无code显示；"CODE_TYPE_ONLY_BARCODE",一维码无code显示；</param>
        /// <param name="title">卡券名，字数上限为9个汉字。(建议涵盖卡券属性、服务及金额)。</param>
        /// <param name="sub_title">券名，字数上限为18个汉字。</param>
        /// <param name="color">券颜色。按色彩规范标注填写Color010-Color100。</param>
        /// <param name="notice">卡券使用提醒，字数上限为16个汉字。</param>
        /// <param name="service_phone">客服电话。</param>
        /// <param name="description">卡券使用说明，字数上限为1024个汉字。</param>
        /// <param name="type">使用时间的类型，仅支持填写1或2。1为固定日期区间，2为固定时长（自领取后按天算）。</param>
        /// <param name="begin_timestamp">type为1时专用，表示起用时间。从1970年1月1日00:00:00至起用时间的秒数，最终需转换为字符串形态传入。（东八区时间，单位为秒）</param>
        /// <param name="end_timestamp">type为1时专用，表示结束时间，建议设置为截止日期的23:59:59过期。（东八区时间，单位为秒）</param>
        /// <param name="fixed_term">type为2时专用，表示自领取后多少天内有效，领取后当天有效填写0。（单位为天）</param>
        /// <param name="fixed_begin_term">type为2时专用，表示自领取后多少天开始生效。（单位为天）</param>
        /// <param name="quantity">卡券库存的数量，不支持填写0，上限为100000000。</param>
        /// <param name="get_limit">每人可领券的数量限制。</param>
        /// <param name="use_custom_code">是否自定义Code码。填写true或false，默认为false。通常自有优惠码系统的开发者选择自定义Code码，在卡券投放时带入。</param>
        /// <param name="bind_openid">是否指定用户领取，填写true或false。默认为false。</param>
        /// <param name="can_share">卡券领取页面是否可分享。</param>
        /// <param name="can_give_friend">卡券是否可转赠。</param>
        /// <param name="location_id_list">门店位置ID。调用POI门店管理接口获取门店位置ID。</param>
        /// <param name="custom_url_name">自定义跳转外链的入口名字。</param>
        /// <param name="custom_url">自定义跳转的URL。</param>
        /// <param name="custom_url_sub_title">显示在入口右侧的提示语。</param>
        /// <param name="promotion_url_name">营销场景的自定义入口名称。</param>
        /// <param name="promotion_url">入口跳转外链的地址链接</param>
        /// <param name="source">第三方来源名，例如同程旅游、大众点评。</param>
        /// <param name="default_detail">填写优惠详情。</param>
        /// <returns></returns>
        public String Add_General_Coupon(String logo_url,
            String brand_name,
            String code_type,
            String title,
            String sub_title,
            String color,
            String notice,
            String service_phone,
            String description,
            int type,
            long begin_timestamp,
            long end_timestamp,
            int fixed_term,
            int fixed_begin_term,
            int quantity,
            int get_limit,
            bool use_custom_code,
            bool bind_openid,
            bool can_share,
            bool can_give_friend,
            int[] location_id_list,
            String custom_url_name,
            String custom_url,
            String custom_url_sub_title,
            String promotion_url_name,
            String promotion_url,
            String source,
            String default_detail)
        #endregion
        {
            var _logoResult = Upload(Upload_Image, logo_url);

            if (_logoResult != null && _logoResult.HasValues)
            {
                logo_url = _logoResult["url"].Value<String>();
            }

            #region 请求
            var ReqObject = JsonConvert.DeserializeObject<JObject>(
                         JsonConvert.SerializeObject(
                         new

                         {
                             card = new
                             {
                                 card_type = "GENERAL_COUPON",
                                 general_coupon = new
                                 {
                                     base_info = new
                                     {
                                         logo_url = logo_url,
                                         brand_name = brand_name,
                                         code_type = code_type,
                                         title = title,
                                         sub_title = sub_title,
                                         color = color,
                                         notice = notice,
                                         service_phone = service_phone,
                                         description = description,
                                         date_info = new
                                         {
                                             type = type,
                                         },
                                         sku = new
                                         {
                                             quantity = quantity
                                         },
                                         get_limit = get_limit,
                                         use_custom_code = use_custom_code,
                                         bind_openid = bind_openid,
                                         can_share = can_share,
                                         can_give_friend = can_give_friend,
                                         location_id_list = location_id_list,
                                         custom_url_name = custom_url_name,
                                         custom_url = custom_url,
                                         custom_url_sub_title = custom_url_sub_title,
                                         promotion_url_name = promotion_url_name,
                                         promotion_url = promotion_url,
                                         source = source,
                                     },
                                     default_detail = default_detail
                                 }
                             }
                         }
                         ));

            if (type == 1)
            {
                ReqObject["card"]["general_coupon"]["base_info"]["date_info"]["begin_timestamp"] = begin_timestamp;
                ReqObject["card"]["general_coupon"]["base_info"]["date_info"]["end_timestamp"] = end_timestamp;
            }
            else if (type == 2)
            {
                ReqObject["card"]["general_coupon"]["base_info"]["date_info"]["fixed_term"] = fixed_term;
                //领取后多少天开始生效
                //fixed_begin_term = fixed_begin_term
            }

            var ReqStr = JsonConvert.SerializeObject(ReqObject);
            #endregion

            var addResult = Request(ticket_create, ReqStr);

            if (addResult != null && addResult.HasValues)
            {
                if (addResult["errcode"].Value<int>() == 0)
                {
                    return addResult["card_id"].Value<String>();
                }
            }

            return String.Empty;
        }
        #endregion

        #region 删除卡券
        /// <summary>
        /// 删除卡券
        /// 删除卡券接口允许商户删除任意一类卡券。
        /// 删除卡券后，该卡券对应已生成的领取用二维码、添加到卡包JS API均会失效。 
        /// 注意：如用户在商家删除卡券前已领取一张或多张该卡券依旧有效。
        /// 即删除卡券不能删除已被用户领取，保存在微信客户端中的卡券。
        /// </summary>
        /// <param name="cardId">卡券ID</param>
        /// <returns></returns>
        public Boolean Delete(String cardId)
        {
            var ReqStr = JsonConvert.SerializeObject(
               new
               {
                   card_id = cardId
               });

            var res = Request(ticket_delete, ReqStr);

            if (res != null && res.HasValues && res["errcode"].Value<int>() == 0)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region 更新卡券
        /// <summary>
        /// 更改卡券信息
        /// 支持更新所有卡券类型的部分通用字段及
        /// 特殊卡券（会员卡、飞机票、电影票、会议门票）中特定字段的信息。
        /// 开发者注意事项注
        /// 1. 更改卡券的部分字段后会重新提交审核，详情见字段说明，更新成功后可通过调用查看卡券详情接口核查更新结果；
        /// 2. 仅填入需要更新的字段，许多开发者在调用该接口时会填入brandname等不支持修改的字段，导致更新不成功。
        /// </summary>
        /// <param name="_cardType">卡券类型</param>
        /// <param name="_card_id">卡券ID</param>
        /// <param name="logo_url">卡券的商户logo，建议像素为300*300。</param>
        /// <param name="color">卡券颜色。</param>
        /// <param name="_notice">使用提醒，字数上限为16个汉字。</param>
        /// <param name="_service_phone">客服电话。</param>
        /// <param name="_description">使用说明。</param>
        /// <param name="_location_id_list">支持更新适用门店列表。</param>
        /// <param name="_custom_url_name">自定义跳转入口的名字。</param>
        /// <param name="_custom_url">自定义跳转的URL。</param>
        /// <param name="_custom_url_sub_title">显示在入口右侧的提示语。</param>
        /// <param name="_promotion_url_name">营销场景的自定义入口名称。</param>
        /// <param name="_promotion_url">入口跳转外链的地址链接。</param>
        /// <param name="_promotion_url_sub_title">显示在营销入口右侧的提示语。</param>
        /// <param name="_code_type">Code码展示类型，"CODE_TYPE_TEXT"，文本；"CODE_TYPE_BARCODE"，一维码 ；"CODE_TYPE_QRCODE"，二维码；"CODE_TYPE_ONLY_QRCODE",二维码无code显示；"CODE_TYPE_ONLY_BARCODE",一维码无code显示；</param>
        /// <param name="_get_limit">每人可领券的数量限制。</param>
        /// <param name="_can_share">卡券原生领取页面是否可分享。</param>
        /// <param name="_can_give_friend">卡券是否可转赠。</param>
        /// <param name="_type">有效期类型，仅支持更改type为1的时间戳，不支持填入2。</param>
        /// <param name="_begin_timestamp">固定日期区间专用，表示起用时间。（单位为秒）</param>
        /// <param name="_end_timestamp">固定日期区间专用，表示结束时间。结束时间仅支持往后延长。</param>
        /// <returns></returns>
        public Boolean Update(String _cardType, String _card_id,
            String _logo_url,
            String _color,
            String _notice,
            String _service_phone,
            String _description,
            int[] _location_id_list,
            String _custom_url_name,
            String _custom_url,
            String _custom_url_sub_title,
            String _promotion_url_name,
            String _promotion_url,
            String _promotion_url_sub_title,
            String _code_type,
            long? _get_limit,
            Boolean? _can_share,
            Boolean? _can_give_friend,
            int? _type,
            long? _begin_timestamp,
            long? _end_timestamp)
        {
            var BaseInfo = new JObject();

            if (!String.IsNullOrEmpty(_logo_url)) { BaseInfo["logo_url"] = _logo_url; }
            //if (!String.IsNullOrEmpty(_color)) { BaseInfo["color"] = _color; }
            if (!String.IsNullOrEmpty(_notice)) { BaseInfo["notice"] = _notice; }
            if (!String.IsNullOrEmpty(_service_phone)) { BaseInfo["service_phone"] = _service_phone; }
            if (!String.IsNullOrEmpty(_description)) { BaseInfo["description"] = _description; }
            if (_location_id_list != null) { BaseInfo["location_id_list"] = JsonConvert.SerializeObject(_location_id_list); }
            if (!String.IsNullOrEmpty(_custom_url_name)) { BaseInfo["custom_url_name"] = _custom_url_name; }
            if (!String.IsNullOrEmpty(_custom_url)) { BaseInfo["custom_url"] = _custom_url; }
            if (!String.IsNullOrEmpty(_custom_url_sub_title)) { BaseInfo["custom_url_sub_title"] = _custom_url_sub_title; }
            if (!String.IsNullOrEmpty(_promotion_url_name)) { BaseInfo["promotion_url_name"] = _promotion_url_name; }
            if (!String.IsNullOrEmpty(_promotion_url)) { BaseInfo["promotion_url"] = _promotion_url; }
            if (!String.IsNullOrEmpty(_promotion_url_sub_title)) { BaseInfo["promotion_url_sub_title"] = _promotion_url_sub_title; }
            if (!String.IsNullOrEmpty(_code_type)) { BaseInfo["code_type"] = _code_type; }
            if (_get_limit.HasValue) { BaseInfo["get_limit"] = _get_limit.Value; }
            if (_can_share.HasValue) { BaseInfo["can_share"] = _can_share.Value; }
            if (_can_give_friend.HasValue) { BaseInfo["can_give_friend"] = _can_give_friend.Value; }
            if (_type.HasValue) { BaseInfo["type"] = _type.Value; }
            if (_begin_timestamp.HasValue) { BaseInfo["begin_timestam"] = _begin_timestamp.Value; }
            if (_end_timestamp.HasValue) { BaseInfo["end_timestamp"] = _end_timestamp.Value; }

            var ReqStr = String.Empty;

            if (_cardType.Equals("GROUPON"))
            {
                ReqStr = JsonConvert.SerializeObject(new
                {
                    card_id = _card_id,
                    groupon = new { base_info = BaseInfo }
                });
            }
            else if (_cardType.Equals("CASH"))
            {
                ReqStr = JsonConvert.SerializeObject(new
                {
                    card_id = _card_id,
                    cash = new { base_info = BaseInfo }
                });
            }
            else if (_cardType.Equals("DISCOUNT"))
            {
                ReqStr = JsonConvert.SerializeObject(new
                {
                    card_id = _card_id,
                    discount = new { base_info = BaseInfo }
                });
            }
            else if (_cardType.Equals("GIFT"))
            {
                ReqStr = JsonConvert.SerializeObject(new
                {
                    card_id = _card_id,
                    gift = new { base_info = BaseInfo }
                });
            }
            else if (_cardType.Equals("GENERAL_COUPON"))
            {
                ReqStr = JsonConvert.SerializeObject(new
                {
                    card_id = _card_id,
                    general_coupon = new { base_info = BaseInfo }
                });
            }

            var res = Request(ticket_update, ReqStr);

            if (res != null && res.HasValues && res["errcode"].Value<int>() == 0)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region 批量查询卡券
        /// <summary>
        /// 批量查询卡券
        /// </summary>
        /// <param name="cardId">卡券ID</param>
        /// <returns></returns>
        public JObject BatchGet(int PageIndex)
        {
            var ReqStr = JsonConvert.SerializeObject(
               new
               {
                   offset = PageIndex * 50,
                   count = 50
               });

            var res = Request(ticket_batchget, ReqStr);

            if (res != null && res.HasValues && res["errcode"].Value<int>() == 0)
            {
                return res;
            }

            return null;
        }
        #endregion

        #region 查询单个卡券详情
        /// <summary>
        /// 批量查询卡券
        /// </summary>
        /// <param name="cardId">卡券ID</param>
        /// <returns></returns>
        public JObject Detail(String cardid)
        {
            var ReqStr = JsonConvert.SerializeObject(
               new
               {
                   card_id = cardid
               });

            var res = Request(ticket_detail, ReqStr);

            if (res != null && res.HasValues && res["errcode"].Value<int>() == 0)
            {
                return res;
            }

            return null;
        }
        #endregion

        #region 
        #region 修改库存接口

        /// <summary>
        /// 修改库存
        /// 调用修改库存接口增减某张卡券的库存。
        /// </summary>
        /// <param name="cardId">卡券ID。</param>
        /// <param name="increase_stock_value">增加多少库存，支持不填或填0。</param>
        /// <param name="reduce_stock_value">减少多少库存，可以不填或填0。</param>
        /// <returns></returns>
        public Boolean ModifyStock(String cardId, int increase_stock_value, int reduce_stock_value)
        {
            var ReqStr = JsonConvert.SerializeObject(
               new
               {
                   card_id = cardId,
                   increase_stock_value = increase_stock_value,
                   reduce_stock_value = reduce_stock_value
               });

            var res = Request(ticket_modifystock, ReqStr);

            if (res != null && res.HasValues && res["errcode"].Value<int>() == 0)
            {
                return true;
            }
            return false;
        }
        #endregion
        #endregion

    }
}
