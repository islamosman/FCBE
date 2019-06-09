using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Web.config", Watch = true)]
[assembly: OwinStartup(typeof(Fly.Startup))]

namespace Fly
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
