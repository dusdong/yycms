using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yycms.union.wechat
{
  public class Device:SDK
    {
      public Device(String _Accecc_Token)
        {
            base.Access_Token = _Accecc_Token;
        }

      #region 申请设备
      public JToken Add()
      {
          var ReqStr = JsonConvert.SerializeObject(new
              {
                  quantity = 500,
                  apply_reason = "申请设备是为了在实体店使用，希望能给予通过，谢谢啦：）"
              });

          var res = Request(Device_Add, ReqStr);

          if (res != null && res.HasValues && res["errcode"].Value<int>() == 0)
          {
              return res["data"];
          }

          return null;
      }
      #endregion

      #region 查询设备
      public JToken Get(int PageIndex)
      {
          var ReqStr = JsonConvert.SerializeObject(new
          {
              begin = PageIndex * 50,
              count =  50
          });

          var res = Request(Device_Get, ReqStr);

          if (res != null && res.HasValues && res["errcode"].Value<int>() == 0)
          {
              return res["data"];
          }

          return null;
      }
      #endregion
    }
}
