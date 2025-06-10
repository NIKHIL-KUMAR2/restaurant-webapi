using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using Serilog;

namespace Restaurant_WebAPI.Controllers
{
    public class ErrorController : ApiController
    {
        [HttpGet, HttpPost, HttpPut, HttpDelete, HttpPatch]
        public IHttpActionResult Handle404()
        {
            var url = Request.RequestUri.ToString();
            Log.Error("[RESOURCE_NOT_FOUND]: {URL}", url);
            return Content(HttpStatusCode.NotFound, new { message = "Resource not found" });
        }
    }
}
