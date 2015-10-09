using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace yycms.union.wechat
{
   public class SDK
    {
       protected String Access_Token { get; set; }

       #region 门店接口
        /// <summary>
        /// 上传图片
        /// 用 POI 接口新建门店时所使用的图片 url 必须为微信自己域名的 url，因此需要先用上传图片接
        ///口上传图片并获取 url，再创建门店。上传的图片限制文件大小限制 1MB，支持 JPG 格式
        /// </summary>
       protected String Upload_Image
       {
           get
           {
               return "https://file.api.weixin.qq.com/cgi-bin/media/uploadimg?access_token=" + Access_Token;
           }
       }

        /// <summary>
        /// 创建门店
        /// 创建门店接口是为商户提供创建自己门店数据的接口，门店数据字段越完整，商户页面展示越丰
        /// 富，能够更多的吸引用户，并提高曝光度。
        /// 创建门店接口调用成功后会返回 errcode、errmsg，但不会实时返回 poi_id。成功创建后，门店信
        /// 息会经过审核，审核通过后方可使用，并获取 poi_id，该 id 为门店的唯一 id，强烈建议自行存
        /// 储该 id，并为后续调用使用。
        /// </summary>
       protected String POI_Add 
       {
           get 
           {
               return "http://api.weixin.qq.com/cgi-bin/poi/addpoi?access_token=" + Access_Token;
           }
       }

        /// <summary>
        /// 查询门店列表
        /// 商户可以通过该接口，批量查询自己名下的门店 list，并获取已审核通过的 poi_id（审核中和审
        /// 核驳回的不返回 poi_id）、商户自身 sid 用于对应、商户名、分店名、地址字段。
        /// </summary>
       protected String POI_Get
       {
           get 
           {
               return "http://api.weixin.qq.com/cgi-bin/poi/getpoilist?access_token=" + Access_Token;
           }
       }

       /// <summary>
       /// 查询门店信息
       /// 在审核通过并获取 poi_id 后，商户可以利用 poi_id，查询具体某条门店的信息。若在查询时，
       /// update_status 字段为 1，表明在 5 个工作日内曾用 update 接口修改过门店扩展字段，该扩展字段
       /// 为最新的修改字段，尚未经过审核采纳，因此不是最终结果。最终结果会在 5 个工作日内，最终
       /// 确认是否采纳，并前端生效（但该扩展字段的采纳过程不影响门店的可用性，即 available_state仍为审核通过状态）
       /// </summary>
       protected String POI_Detail
       {
           get
           {
               return "http://api.weixin.qq.com/cgi-bin/poi/getpoi?access_token=" + Access_Token;
           }
       }


       /// <summary>
       /// 删除门店
       /// 商户可以通过该接口，删除已经成功创建的门店。请商户慎重调用该接口，门店信息被删除后，
       ///可能会影响其他与门店相关的业务使用，如卡券等。同样，该门店信息也不会在微信的商户详情
       ///页显示，不会再推荐入附近功能。
       /// </summary>
       protected String POI_Delete
       {
           get
           {
               return "http://api.weixin.qq.com/cgi-bin/poi/delpoi?access_token=" + Access_Token;
           }
       }


       /// <summary>
       /// 修改门店服务信息
       ///商户可以通过该接口，修改门店的服务信息，包括：图片列表、营业时间、推荐、特色服务、简
       ///介、人均价格、电话 7 个字段。目前基础字段包括（名称、坐标、地址等不可修改）
       /// </summary>
       protected String POI_Update
       {
           get
           {
               return "http://api.weixin.qq.com/cgi-bin/poi/updatepoi?access_token=" + Access_Token;
           }
       }
       #endregion

       #region 卡券
       /// <summary>
       ///  消耗 code
       ///  消耗 code 接口是核销卡券的唯一接口，仅支持核销有效期内的卡券，否则会返回错误码
       ///  invalid time。
       ///  自定义 code（use_custom_code 为 true）的优惠券，在 code 被核销时，必须调用此
       ///  接口。用于将用户客户端的 code 状态变更。自定义 code 的卡券调用接口时， post 数据中
       ///  需包含 card_id，非自定义 code 不需上报。
       ///   </summary>
       protected String ticket_consume
       {
           get
           {
               return "https://api.weixin.qq.com/card/code/unavailable?access_token=" + Access_Token;
           }
       }

       /// <summary>
       /// 创建卡券
       /// 创建卡券接口是微信卡券的基础接口，用于创建一类新的卡券，获取card_id，创建成功并通过审核后，
       /// 商家可以通过文档提供的其他接口将卡券下发给用户，每次成功领取，库存数量相应扣除。
       /// </summary>
       protected String ticket_create
       {
           get
           {
               return "https://api.weixin.qq.com/card/create?access_token=" + Access_Token;
           }
       }

       /// <summary>
       /// 删除卡券接口
       /// 删除卡券接口允许商户删除任意一类卡券。
       /// 删除卡券后，该卡券对应已生成的领取用二维码、添加到卡包JS API均会失效。
       /// 注意：如用户在商家删除卡券前已领取一张或多张该卡券依旧有效。
       /// 即删除卡券不能删除已被用户领取，保存在微信客户端中的卡券。
       /// </summary>
       protected String ticket_delete
       {
           get
           {
               return "https://api.weixin.qq.com/card/delete?access_token=" + Access_Token;
           }
       }

       /// <summary>
       /// 批量查询卡列表
       /// </summary>
       protected String ticket_batchget
       {
           get 
           {
               return "https://api.weixin.qq.com/card/batchget?access_token=" + Access_Token;
           }
       }

       /// <summary>
       /// 查看卡券详情
       /// 调用该接口可查询卡券字段详情及卡券所处状态。
       /// 建议开发者调用卡券更新信息接口后调用该接口验证是否更新成功。
       /// </summary>
       protected String ticket_detail
       {
           get 
           {
               return "https://api.weixin.qq.com/card/get?access_token=" + Access_Token; 
           }
       }

       /// <summary>
       /// 更改卡券信息接口
       /// 支持更新所有卡券类型的部分通用字段及特殊卡券（会员卡、飞机票、电影票、会议门票）中特定字段的信息。
       /// 开发者注意事项注
       /// 1. 更改卡券的部分字段后会重新提交审核，详情见字段说明，更新成功后可通过调用查看卡券详情接口核查更新结果；
       /// 2. 仅填入需要更新的字段，许多开发者在调用该接口时会填入brandname等不支持修改的字段，导致更新不成功。
       /// </summary>
       protected String ticket_update
       {
           get
           {
               return "https://api.weixin.qq.com/card/update?access_token=" + Access_Token;
           }
       }

       /// <summary>
       /// 修改库存接口
       /// 调用修改库存接口增减某张卡券的库存。
       /// </summary>
       protected String ticket_modifystock
       {
           get
           {
               return "https://api.weixin.qq.com/card/modifystock?access_token=" + Access_Token;
           }
       }
       
       #endregion

       #region 摇一摇
       /// <summary>
       /// 申请设备ID
       /// 请配置设备所需的UUID、Major、Minor。申请成功后返回批次ID，
       /// 可用返回的批次ID用“查询设备列表”接口拉取本次申请的设备ID。
       /// 单次新增设备超过500个，需走人工审核流程，大概需要三个工作日；
       /// 单次新增设备不超过500个的，当日可返回申请的设备ID。
       /// 一个公众账号最多可申请99999个设备ID，如需申请的设备ID数超过最大限额，
       /// 请邮件至zhoubian@tencent.com，邮件格式如下： 标题：申请提升设备ID额度 
       /// 内容：1、公众账号名称及appid（wx开头的字符串，在mp平台可查看） 
       /// 2、用途 3、预估需要多少设备ID
       /// </summary>
       protected String Device_Add
       {
           get
           {
               return "https://api.weixin.qq.com/shakearound/device/applyid?access_token=" + Access_Token;
           }
       }

       /// <summary>
       /// 查询设备列表
       /// 查询已有的设备ID、UUID、Major、Minor、激活状态、备注信息、关联门店、关联页面等信息。
       /// 可指定设备ID或完整的UUID、Major、Minor查询，也可批量拉取设备信息列表。
       /// </summary>
       protected String Device_Get
       {
           get
           {
               return "https://api.weixin.qq.com/shakearound/device/search?access_token=" + Access_Token;
           }
       }
       #endregion

       #region 摇一摇页面
       /// <summary>
       /// 以页面为维度的数据统计接口
       /// 查询单个页面通过摇周边摇出来的人数、次数，点击摇周边页面的人数、次数；
       /// 查询的最长时间跨度为30天。此接口无法获取当天的数据，最早只能获取前一天的数据。
       /// 由于系统在凌晨处理前一天的数据，太早调用此接口可能获取不到数据，建议在早上8：00之后调用此接口。
       /// </summary>
       protected String shakepage_statistics { get { return "https://api.weixin.qq.com/shakearound/statistics/page?access_token=" + Access_Token; } }

       /// <summary>
       /// 上传图片素材
       /// 上传在摇一摇页面展示的图片素材，素材保存在微信侧服务器上。 格式限定为：jpg,jpeg,png,gif，
       /// 图片大小建议120px*120 px，限制不超过200 px *200 px，图片需为正方形。
       /// </summary>
       protected String shakepage_upload { get { return "https://api.weixin.qq.com/shakearound/material/add?access_token=" + Access_Token; } }
       
       /// <summary>
       /// 新增页面
       /// 新增摇一摇出来的页面信息，包括在摇一摇页面出现的主标题、副标题、图片和点击进去的超链接。
       /// 其中，图片必须为用素材管理接口上传至微信侧服务器后返回的链接。
       /// </summary>
       protected String shakepage_add { get { return "https://api.weixin.qq.com/shakearound/page/add?access_token=" + Access_Token; } }
      
       /// <summary>
       /// 编辑页面信息
       /// 编辑摇一摇出来的页面信息，包括在摇一摇页面出现的主标题、副标题、图片和点击进去的超链接。
       /// </summary>
       protected String shakepage_update { get { return "https://api.weixin.qq.com/shakearound/page/update?access_token=" + Access_Token; } }
       
       /// <summary>
       /// 删除页面
       /// 删除已有的页面，包括在摇一摇页面出现的主标题、副标题、图片和点击进去的超链接。
       /// 只有页面与设备没有关联关系时，才可被删除。
       /// </summary>
       protected String shakepage_delete { get { return "https://api.weixin.qq.com/shakearound/page/delete?access_token=" + Access_Token; } }
      
       /// <summary>
       /// 查询页面列表
       /// 查询已有的页面，包括在摇一摇页面出现的主标题、副标题、图片和点击进去的超链接。
       /// 提供两种查询方式，可指定页面ID查询，也可批量拉取页面列表。
       /// </summary>
       protected String shakepage_search { get { return "https://api.weixin.qq.com/shakearound/page/search?access_token=" + Access_Token; } }

       /// <summary>
       /// 配置设备与页面的关联关系
       /// 配置设备与页面的关联关系。支持建立或解除关联关系，也支持新增页面或覆盖页面等操作。
       /// 配置完成后，在此设备的信号范围内，即可摇出关联的页面信息。若设备配置多个页面，
       /// 则随机出现页面信息。一个设备最多可配置30个关联页面。
       /// </summary>
       protected String shakepage_relate_page { get { return "https://api.weixin.qq.com/shakearound/device/bindpage?access_token=" + Access_Token; } }
       #endregion

       #region 红包
       /// <summary>
       /// 发红包
       /// 用于企业向微信用户个人发现金红包，目前支持向指定微信用户的openid发放指定金额红包。
       /// </summary>
       protected String redpack_send 
       {
           get
           {
               return "https://api.mch.weixin.qq.com/mmpaymkttransfers/sendredpack"; 
           }
       }
       
       #endregion

       #region 菜单
       /// <summary>
       /// 添加菜单
       /// 自定义菜单能够帮助公众号丰富界面，让用户更好更快地理解公众号的功能。开启自定义菜单后，公众号
       /// </summary>
       protected String menu_add
       {
           get
           {
               return " https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + Access_Token;
           }
       }

       /// <summary>
       /// 同步菜单
       /// 使用接口创建自定义菜单后，开发者还可使用接口查询自定义菜单的结构。
       /// </summary>
       protected String menu_Get
       {
           get
           {
               return "https://api.weixin.qq.com/cgi-bin/menu/get?access_token=" + Access_Token;
           }
       }
       #endregion

       #region 群发
       /// <summary>
       /// 根据分组进行群发【订阅号与服务号认证后均可用】
       /// </summary>
       protected String Group_Get
       {
           get
           {
               return "https://api.weixin.qq.com/cgi-bin/message/mass/sendall?access_token=" + Access_Token;
           }
       }

       /// <summary>
       /// 根据OpenID列表群发【订阅号不可用，服务号认证后可用】
       /// </summary>
       protected String GroupID_Get
       {
           get
           {
               return "https://api.weixin.qq.com/cgi-bin/message/mass/send?access_token=" + Access_Token;
           }
       }
       /// <summary>
       /// 视频的media_id需通过POST请求
       /// </summary>
       protected String Group_Video 
       {
           get 
           {
               return "https://file.api.weixin.qq.com/cgi-bin/media/uploadvideo?access_token=" + Access_Token;
           }
       }

       #endregion

       #region 二维码
       /// <summary>
       /// 创建二维码ticket
       /// 每次创建二维码ticket需要提供一个开发者自行设定的参数（scene_id），
       /// 分别介绍临时二维码和永久二维码的创建二维码ticket过程。
       /// </summary>
       protected String QRCode_Create
       {
           get
           {
               return "https://api.weixin.qq.com/cgi-bin/qrcode/create?access_token=" + Access_Token;
           }
       }

       /// <summary>
       /// 通过ticket换取二维码
       /// 获取二维码ticket后，开发者可用ticket换取二维码图片。
       /// 请注意，本接口无须登录态即可调用。
       /// </summary>
       protected String QRCode_Show
       {
           get
           {
               return "https://mp.weixin.qq.com/cgi-bin/showqrcode?ticket=";
           }
       }
       #endregion

       #region 模板消息
       /// <summary>
       /// 模板消息接口文档
       /// 为了保证用户不受到骚扰，在开发者出现需要主动提醒、通知用户时，
       /// 才允许开发者在公众平台网站中模板消息库中选择模板，选择后获得模板ID，再根据模板ID向用户主动推送提醒、通知消息。
       /// 模板消息调用时主要需要模板ID和模板中各参数的赋值内容。请注意：
       /// 1.模板中参数内容必须以".DATA"结尾，否则视为保留字;
       /// 2.模板保留符号"{{ }}"
       /// </summary>
       protected String TemplateMsg_Send
       {
           get
           {
               return "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token="+Access_Token;
           }
        }
        #endregion

       #region 素材
        /// <summary>
        /// 在新增了永久素材后，开发者可以分类型获取永久素材的列表。
        /// 1、获取永久素材的列表，也会包含公众号在公众平台官网素材管理模块中新建的图文消息、语音、视频等素材（但需要先通过获取素材列表来获知素材的media_id）
        /// 2、临时素材无法通过本接口获取
        /// 3、调用该接口需https协议
        /// </summary>
        protected String material_batchget
        {
            get
            {
                return "https://api.weixin.qq.com/cgi-bin/material/batchget_material?access_token=" + Access_Token;
            }
        }

        /// <summary>
        /// 获取素材总数
        /// 1.永久素材的总数，也会计算公众平台官网素材管理中的素材
        /// 2.图片和图文消息素材（包括单图文和多图文）的总数上限为5000，其他素材的总数上限为1000
        /// 3.调用该接口需https协议
        /// </summary>
        protected String material_count
        {
            get
            {
                return "https://api.weixin.qq.com/cgi-bin/material/get_materialcount?access_token=" + Access_Token;
            }
        }

        /// <summary>
        /// 修改永久图文素材
        /// 1、也可以在公众平台官网素材管理模块中保存的图文消息（永久图文素材）
        /// 2、调用该接口需https协议
        /// </summary>
        protected String material_update
        {
            get
            {
                return "https://api.weixin.qq.com/cgi-bin/material/update_news?access_token=" + Access_Token;
            }
        }

        /// <summary>
        /// 删除永久素材
        /// 1、请谨慎操作本接口，因为它可以删除公众号在公众平台官网素材管理模块中新建的图文消息、语音、视频等素材（但需要先通过获取素材列表来获知素材的media_id）
        /// 2、临时素材无法通过本接口删除
        /// 3、调用该接口需https协议
        /// </summary>
        protected String material_del
        {
            get
            {
                return "https://api.weixin.qq.com/cgi-bin/material/del_material?access_token=" + Access_Token;
            }
        }

        /// <summary>
        /// 获取永久素材
        /// 1、获取永久素材也可以获取公众号在公众平台官网素材管理模块中新建的图文消息、图片、语音、视频等素材（但需要先通过获取素材列表来获知素材的media_id）
        /// 2、临时素材无法通过本接口获取
        /// 3、调用该接口需https协议
        /// </summary>
        protected String material_get
        {
            get
            {
                return "https://api.weixin.qq.com/cgi-bin/material/get_material?access_token=" + Access_Token;
            }
        }

        /// <summary>
        /// 新增永久素材
        /// 1、新增的永久素材也可以在公众平台官网素材管理模块中看到
        /// 2、永久素材的数量是有上限的，请谨慎新增。图文消息素材和图片素材的上限为5000，其他类型为1000
        /// 3、素材的格式大小等要求与公众平台官网一致。具体是，图片大小不超过2M，
        ///    支持bmp/png/jpeg/jpg/gif格式，语音大小不超过5M，长度不超过60秒，支持mp3/wma/wav/amr格式
        /// 4、调用该接口需https协议
        /// </summary>
        protected String material_add
        {
            get
            {
                return "https://api.weixin.qq.com/cgi-bin/material/add_news?access_token=" + Access_Token;
            }
        }

        /// <summary>
        /// 上传图文消息内的图片获取URL
        /// 本接口所上传的图片不占用公众号的素材库中图片数量的5000个的限制。图片仅支持jpg/png格式，大小必须在1MB以下。
        /// </summary>
        protected String material_uploadimg
        {
            get
            {
                return "https://api.weixin.qq.com/cgi-bin/media/uploadimg?access_token=" + Access_Token;
            }
        }

        /// <summary>
        /// 新增其他类型永久素材
        /// 通过POST表单来调用接口，表单id为media，包含需要上传的素材内容，有filename、filelength、content-type等信息。
        /// 请注意：图片素材将进入公众平台官网素材管理模块中的默认分组。
        /// </summary>
        protected String material_add_material
        {
            get
            {
                return "https://api.weixin.qq.com/cgi-bin/material/add_material?access_token=" + Access_Token;
            }
        }
        #endregion

        protected JObject Upload(string url, string filename)
        {
            ServicePointManager.ServerCertificateValidationCallback += (a, b, c, d) => true;

            WebClient wc = new WebClient();

            byte[] result = wc.UploadFile(url, filename);

            string resultjson = Encoding.Default.GetString(result);

            try
            {
                return JsonConvert.DeserializeObject<JObject>(resultjson);
            }
            catch
            {
                return null;
            }
        }

        protected JObject Request(string url, String jsonStr)
        {
            ServicePointManager.ServerCertificateValidationCallback += (a, b, c, d) => true;

            WebClient wc = new WebClient();

            byte[] result = wc.UploadData(url, Encoding.UTF8.GetBytes(jsonStr));

            string resultjson = Encoding.UTF8.GetString(result);

            try
            {
                return JsonConvert.DeserializeObject<JObject>(resultjson);
            }
            catch
            {
                return null;
            }
        }

        protected String Get(string url)
        {
            ServicePointManager.ServerCertificateValidationCallback += (a, b, c, d) => true;

            WebClient wc = new WebClient() { Encoding = Encoding.UTF8 };

            string resultjson = wc.DownloadString(url);

            try
            {
                return resultjson;
            }
            catch
            {
                return null;
            }
        }

        protected String Request_XML(string url, String XmlStr,String mechantPayID,String certPath)
        {
            ServicePointManager.ServerCertificateValidationCallback = (sender, _certificate, chain, errors) =>
            {
                if (errors == SslPolicyErrors.None)
                    return true;
                return false;
            };
            var cert = certPath;
            //本地测试无需修改
            //如果是IIS下需要修改应用程序时读取用户配置文件为true
            X509Certificate cer = new X509Certificate(cert, mechantPayID);
            #region 该部分是关键，若没有该部分则在IIS下会报 CA证书出错
            X509Certificate2 certificate = new X509Certificate2(cert, mechantPayID);
            X509Store store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            store.Remove(certificate);//可省略
            store.Add(certificate);
            store.Close();
            #endregion

            HttpWebRequest webrequest = (HttpWebRequest)HttpWebRequest.Create(url);
            webrequest.ClientCertificates.Add(cer);
            webrequest.Method = "post";

            byte[] data = Encoding.UTF8.GetBytes(XmlStr);
            using (Stream reqStream = webrequest.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }

            HttpWebResponse webreponse = (HttpWebResponse)webrequest.GetResponse();
            Stream stream = webreponse.GetResponseStream();
            string resp = string.Empty;
            using (StreamReader reader = new StreamReader(stream))
            {
                resp = reader.ReadToEnd();
            }
            return resp;
        }
    }
}
