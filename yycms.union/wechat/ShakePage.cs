using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace yycms.union.wechat
{
   public class ShakePage:SDK
    {
       public ShakePage(String _Accecc_Token) { base.Access_Token = _Accecc_Token; }

       #region 上传图片
       private String UploadImage(String _ImagePath)
       {
           try
           {
               if (String.IsNullOrEmpty(_ImagePath) || _ImagePath.Contains("http://")) { return _ImagePath; }

               var ImageItem = Upload(shakepage_upload, _ImagePath);

               if (ImageItem != null && ImageItem.HasValues && ImageItem["errcode"].Value<int>() == 0)
               {
                   return ImageItem["data"]["pic_url"].Value<String>();
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

       /// <summary>
       /// 新增页面
       /// 新增摇一摇出来的页面信息，包括在摇一摇页面出现的主标题、副标题、图片和点击进去的超链接。
       /// 其中，图片必须为用素材管理接口上传至微信侧服务器后返回的链接。
       /// </summary>
       /// <param name="title">在摇一摇页面展示的主标题，不超过6个字</param>
       /// <param name="description">在摇一摇页面展示的副标题，不超过7个字</param>
       /// <param name="icon_url">在摇一摇页面展示的图片。图片需先上传至微信侧服务器，用“素材管理-上传图片素材”接口上传图片，返回的图片URL再配置在此处</param>
       /// <param name="comment">页面的备注信息，不超过15个字</param>
       /// <param name="page_url">跳转链接</param>
       /// <returns></returns>
       public String Add(String title, String description, String icon_url, String comment,String page_url) 
       {
           icon_url = UploadImage(icon_url);

           var ReqStr = JsonConvert.SerializeObject(
               new
               {
                   title = title,
                   description = description,
                   icon_url = icon_url,
                   page_url = page_url,
                   comment = comment
               });

           var res = Request(shakepage_add, ReqStr);

           if (res != null && res.HasValues && res["errcode"].Value<int>() == 0)
           {
               return res["data"]["page_id"].Value<String>();
           }

           return JsonConvert.SerializeObject(res);
       }

       /// <summary>
       /// 编辑页面信息
       /// 编辑摇一摇出来的页面信息，包括在摇一摇页面出现的主标题、副标题、图片和点击进去的超链接。
       /// </summary>
       /// <param name="page_id">摇周边页面唯一ID</param>
       /// <param name="title">在摇一摇页面展示的主标题，不超过6个字</param>
       /// <param name="description">在摇一摇页面展示的副标题，不超过7个字</param>
       /// <param name="icon_url">在摇一摇页面展示的图片。图片需先上传至微信侧服务器，用“素材管理-上传图片素材”接口上传图片，返回的图片URL再配置在此处</param>
       /// <param name="comment">页面的备注信息，不超过15个字</param>
       /// <param name="page_url">跳转链接</param>
       /// <returns></returns>
       public Boolean Update(long page_id, String title, String description, String icon_url, String comment, String page_url) 
       {
           icon_url = UploadImage(icon_url);

           var ReqStr = JsonConvert.SerializeObject(
               new
               {
                   page_id=page_id,
                   title = title,
                   description = description,
                   icon_url = icon_url,
                   page_url = page_url,
                   comment = comment
               });

           var res = Request(shakepage_update, ReqStr);

           if (res != null && res.HasValues && res["errcode"].Value<int>() == 0)
           {
               return true;
           }

           return false;
       }

       /// <summary>
       /// 删除页面
       /// 删除已有的页面，包括在摇一摇页面出现的主标题、
       /// 副标题、图片和点击进去的超链接。只有页面与设备没有关联关系时，才可被删除。
       /// </summary>
       /// <param name="page_ids">摇周边页面唯一ID集合</param>
       /// <returns></returns>
       public Boolean Delete(long[] page_ids) 
       {
           var ReqStr = JsonConvert.SerializeObject(
               new
               {
                   page_ids = page_ids
               });

           var res = Request(shakepage_delete, ReqStr);

           if (res != null && res.HasValues && res["errcode"].Value<int>() == 0)
           {
               return true;
           }

           return false;
       }

       /// <summary>
       /// 查询页面列表
       /// 查询已有的页面，包括在摇一摇页面出现的主标题、副标题、图片和点击进去的超链接。
       /// 提供两种查询方式，可指定页面ID查询，也可批量拉取页面列表。
       /// </summary>
       /// <param name="PageIndex"></param>
       /// <returns></returns>
       public JToken Get(int PageIndex) 
       {
           var ReqStr = JsonConvert.SerializeObject(new
           {
               begin = PageIndex * 50,
               count = 50
           });

           var res = Request(shakepage_search, ReqStr);

           if (res != null && res.HasValues && res["errcode"].Value<int>() == 0)
           {
               return res["data"];
           }

           return null;
       }


       /// <summary>
       /// 配置设备与页面的关联关系
       /// 配置设备与页面的关联关系。支持建立或解除关联关系，也支持新增页面或覆盖页面等操作。
       /// 配置完成后，在此设备的信号范围内，即可摇出关联的页面信息。
       /// 若设备配置多个页面，则随机出现页面信息。一个设备最多可配置30个关联页面。
       /// </summary>
       /// <param name="device_id">设备编号，若填了UUID、major、minor，则可不填设备编号，若二者都填，则以设备编号为优先</param>
       /// <param name="uuid">UUID、major、minor，三个信息需填写完整，若填了设备编号，则可不填此信息</param>
       /// <param name="major"></param>
       /// <param name="minor"></param>
       /// <param name="bind">关联操作标志位， 0为解除关联关系，1为建立关联关系</param>
       /// <param name="append">新增操作标志位， 0为覆盖，1为新增</param>
       /// <param name="page_ids">待关联的页面列表</param>
       /// <returns></returns>
       public Boolean Relate(long device_id,String uuid,long major,long minor,int bind,int append, long[] page_ids)
       {
           var ReqStr = JsonConvert.SerializeObject(
               new
               {
                   device_identifier = new
                   {
                       device_id = device_id,
                       uuid = uuid,
                       major = major,
                       minor = minor
                   },
                   page_ids = page_ids,
                   bind = bind,
                   append = append,
               });

           var res = Request(shakepage_relate_page, ReqStr);

           if (res != null && res.HasValues && res["errcode"].Value<int>() == 0)
           {
               return true;
           }

           return false;
       }



       /// <summary>
       /// 以页面为维度的数据统计接口
       /// 查询单个页面通过摇周边摇出来的人数、次数，点击摇周边页面的人数、次数；
       /// 查询的最长时间跨度为30天。此接口无法获取当天的数据，最早只能获取前一天的数据。
       /// 由于系统在凌晨处理前一天的数据，太早调用此接口可能获取不到数据，建议在早上8：00之后调用此接口。
       /// </summary>
       /// <param name="page_id">指定页面的设备ID</param>
       /// <param name="begin_date">起始日期时间戳，最长时间跨度为30天</param>
       /// <param name="end_date">结束日期时间戳，最长时间跨度为30天</param>
       /// <returns></returns>
       public JToken statistics(long page_id, long begin_date, long end_date)
       {
           var ReqStr = JsonConvert.SerializeObject(
               new
               {
                   page_id = page_id,
                   begin_date = begin_date,
                   end_date = end_date
               });

           var res = Request(shakepage_statistics, ReqStr);

           if (res != null && res.HasValues && res["errcode"].Value<int>() == 0)
           {
               return res;
           }

           return null;
       }
    }
}
