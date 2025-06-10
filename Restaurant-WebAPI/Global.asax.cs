using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Restaurant_WebAPI.App_Start;
using Serilog;

namespace Restaurant_WebAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("C:\\logs\\restaurant-manager\\restaurant-manager.log",
                          rollingInterval: RollingInterval.Day,
                          retainedFileCountLimit: 7)
            .CreateLogger();

            Log.Information("Application starting up");
            GlobalConfiguration.Configure(WebApiConfig.Register);
            UnityConfig.RegisterComponents();
        }
    }
}
