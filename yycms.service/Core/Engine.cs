using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Messaging;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Topshelf;

namespace yycms.service
{
    /// <summary>
    /// 任务引擎
    /// </summary>
    public class Engine : ServiceControl
    {
        /// <summary>
        /// 任务集合
        /// </summary>
       public static Dictionary<JobKey, TriggerKey> Jobs = null;

        /// <summary>
       /// 获取当前运行程序集中所有继承了ITask接口的任务
        /// </summary>
       List<Type> Tasks
       {
           get
           {             
               return Assembly.GetExecutingAssembly()
                   .GetTypes()
                   .Where(x => x.GetInterfaces().Contains(typeof(ITask)))
                   .Select(x => typeof(Task<>).MakeGenericType(x))
                   .ToList();
           }
       }


        /// <summary>
        /// Quartz执行计划
        /// </summary>
       IScheduler Scheduler = null;



        /// <summary>
        /// 构造函数
        /// </summary>
       public Engine()
       {
           Jobs = new Dictionary<JobKey, TriggerKey>();
           Scheduler = StdSchedulerFactory.GetDefaultScheduler();
       }

        /// <summary>
        /// 启动
        /// </summary>
       public void Start()
       {
           #region 启动任务计划
           Scheduler.Start();
           Console.WriteLine("Task");
           foreach (var t in Tasks)
           {
               try
               {
                   //获取当前泛型类的类型
                   var TaskType = t.GetGenericArguments()[0];

                   //创建实例
                   var TaskInstance = Activator.CreateInstance(TaskType) as ITask;

                   var Job = JobBuilder.Create(t)
                         .WithIdentity(TaskType.Name, TaskType.Namespace)
                         .Build();

                   if (!Scheduler.CheckExists(Job.Key))
                   {
                       var Trigger = TriggerBuilder.Create()
                         .WithIdentity(TaskType.Name, TaskType.Namespace)
                         .WithSimpleSchedule(
                           a => a.WithIntervalInSeconds(TaskInstance.WithIntervalInSeconds).RepeatForever()
                           ).StartNow().Build();

                      Scheduler.ScheduleJob(Job, Trigger);

                      Jobs.Add(Job.Key, Trigger.Key);
                   }
               }
             
               catch (Exception ex)
               {
                   new Log().Add(t.Name, JsonConvert.SerializeObject(ex), EventLogEntryType.Error);

                   Console.WriteLine(ex.Message);
               }
               Thread.Sleep(1);
           }
           Scheduler.ListenerManager.AddJobListener(new TaskListener(), new TaskMatcher());
           #endregion
       }

       #region 
       /// <summary>
        /// 停止
        /// </summary>
       public void Stop() 
       {
           Scheduler.Shutdown();
       }

       public bool Start(HostControl hostControl)
       {
           this.Start();

           return true;
       }

       public bool Stop(HostControl hostControl)
       {
           this.Stop();
           return true;
       }
       #endregion
    }
}
