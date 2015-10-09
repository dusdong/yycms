using Newtonsoft.Json.Linq;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace yycms.service
{
    public class TaskListener : IJobListener
    {
        String JobKey(IJobExecutionContext context)
        {
            return MD5(context.JobDetail.Key.Name + context.JobDetail.Key.Group);
        }

        /// <summary>
        /// Job执行被拒接
        /// </summary>
        public void JobExecutionVetoed(IJobExecutionContext context)
        {

        }

        /// <summary>
        /// job执行之前
        /// </summary>
        public void JobToBeExecuted(IJobExecutionContext context)
        {
            Console.WriteLine(context.JobDetail.Key.Name + " to be executed");
        }

        /// <summary>
        /// 执行之后
        /// </summary>
        /// <param name="context"></param>
        /// <param name="jobException"></param>
        public void JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            Console.WriteLine(context.JobDetail.Key.Name + " was excuted" + (jobException != null ? ", has error!" : ""));
            context.Scheduler.ResumeJob(context.JobDetail.Key);
        }

        public string Name
        {
            get
            {
                return this.GetType().Name;
            }
        }

        String MD5(String _text)
        {
            using (var md5 = new MD5CryptoServiceProvider())
            {
                byte[] result = md5.ComputeHash(Encoding.UTF8.GetBytes(_text));

                return Encoding.UTF8.GetString(result);
            }
        }
    }
}
