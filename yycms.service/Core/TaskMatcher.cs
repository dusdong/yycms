using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yycms.service
{
    public class TaskMatcher : IMatcher<JobKey>
    {
        public bool IsMatch(JobKey key)
        {
            return true;
        }
    }
}
