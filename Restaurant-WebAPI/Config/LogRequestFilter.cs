using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Serilog;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Restaurant_WebAPI.Config
{
    public class LogRequestFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var request = actionContext.Request;
            var method = request.Method.Method;
            var uri = request.RequestUri.ToString();

            Log.Information("Incoming {Method} request to {URL}", method, uri);
        }
    }
}