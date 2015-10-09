using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace yycms.service
{
    /// <summary>
    /// 任务接口
    /// </summary>
   public interface ITask 
    {
        /// <summary>
        /// 任务执行的间隔时间，以秒为单位
        /// </summary>
        int WithIntervalInSeconds { get; }

        /// <summary>
        /// 任务将要执行的内容
        /// </summary>
        /// <param name="context"></param>
        void Run(IJobExecutionContext context);
    }

   /// <summary>
   /// 任务父类
   /// </summary>
   public class Task<T> : IJob where T : ITask
   {
       /// <summary>
       /// 任务名
       /// </summary>
       public string Name
       {
           get
           {
               return typeof(T).Name;
           }
       }

       /// <summary>
       /// 分组
       /// </summary>
       public string Group
       {
           get
           {
               return typeof(T).Namespace;
           }
       }

       /// <summary>
       /// 任务说明
       /// </summary>
       public string Description
       {
           get { return String.Empty; }
       }


       private T _Instance;

       /// <summary>
       /// 当前任务实例
       /// </summary>
       public T Instance
       {
           get 
           {
               if (_Instance == null)
               {
                 _Instance = (T)Activator.CreateInstance(GetType().GetGenericArguments()[0]);
               }
               return _Instance; 
           }
       }


       public Task() { }

       /// <summary>
       /// 执行任务
       /// </summary>
       /// <param name="context">任务上下文</param>
       public void Execute(IJobExecutionContext context)
       {
           try
           {
               Instance.Run(context);
           }
           catch (Exception ex)
           {
               new Log().Add(Name, JsonConvert.SerializeObject(ex), EventLogEntryType.Error);
           }
       }
   }
}
