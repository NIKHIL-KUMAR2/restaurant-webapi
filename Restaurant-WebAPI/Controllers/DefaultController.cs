using System;
using System.Collections.Generic;

using System.Web.Http;
using Restaurant_WebAPI.Util;
using Serilog;

namespace Restaurant_WebAPI.Controllers
{
    [RoutePrefix(Constants.HomeRoute)]
    public class DefaultController : ApiController
    {
        [HttpGet]
        [Route("")]
        public string Get()
        {
            return "Working!";
        }
    }
}
