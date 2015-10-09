using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace yycms.service
{
    /// <summary>
    /// 单个服务的对象
    /// </summary>
  public class yyTask
    {
      /// <summary>
      /// 执行控制器
      /// </summary>
      public Timer Timer { get; set; }

      /// <summary>
      /// 对应的服务
      /// </summary>
      public ITask Service { get; set; }
    }
}
