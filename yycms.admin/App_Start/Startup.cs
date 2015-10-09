using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

[assembly: OwinStartup(typeof(yycms.admin.App_Start.Startup))]
namespace yycms.admin.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app) 
        {
            app.MapSignalR();
        }
    }
}