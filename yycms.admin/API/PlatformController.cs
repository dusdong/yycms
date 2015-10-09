using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using yycms.entity;
using System.Web;
using yycms.admin.Models;

namespace yycms.admin.API
{
    /// <summary>
    /// 平台
    /// </summary>
    [BasicAuthen]
    public class PlatformController : BasicAPI
    {
        #region 修改平台
        /// <summary>
        /// 修改平台
        /// </summary>
        /// <param name="value">平台详情</param>
        [HttpPut]
        public ResponseItem Put(yy_Platforms value)
        {
            if (value == null)
            {
                return new ResponseItem(1, "传入了空数据，无法继续。");
            }
            var _Entity = DB.yy_Platforms.Find(value.ID);
            if (_Entity != null)
            {
                _Entity.Access_token = value.Access_token;
                _Entity.Image = value.Image;
                _Entity.jsapi_ticket = value.jsapi_ticket;
                _Entity.jsapi_ticket_Expires_in = value.jsapi_ticket_Expires_in;
                _Entity.JSSDK = value.JSSDK;
                _Entity.Code = value.Code;
                _Entity.Access_token_Expires_in = value.Access_token_Expires_in;
                _Entity.api_ticket = value.api_ticket;
                _Entity.api_ticket_Expires_in = value.api_ticket_Expires_in;
                _Entity.APPAdminID = value.APPAdminID;
                _Entity.APPID = value.APPID;
                _Entity.APPKey = value.APPKey;
                _Entity.APPName = value.APPName;
                _Entity.APPNumber = value.APPNumber;
                _Entity.APPPayCert = value.APPPayCert;
                _Entity.APPPayID = value.APPPayID;
                _Entity.APPPayKey = value.APPPayKey;
                _Entity.APPSecret = value.APPSecret;
                _Entity.Link = value.Link;
                _Entity.Name = value.Name;
                _Entity.PlatformConfig = value.PlatformConfig;
                _Entity.QRCode = value.QRCode;
                _Entity.RecallUrl = value.RecallUrl;
                _Entity.Refresh_token = value.Refresh_token;
                _Entity.Remark = value.Remark;
                DB.SaveChanges();
                return new ResponseItem(0, "");
            }
            else 
            {
                
                DB.yy_Platforms.Add(value);
                DB.SaveChanges();
                return new ResponseItem(0, "");
            }
        }
        #endregion
    }
}
