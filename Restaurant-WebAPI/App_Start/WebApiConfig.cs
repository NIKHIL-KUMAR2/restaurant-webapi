using System.Net.Http.Headers;
using System.Web.Http;
using Restaurant_WebAPI.Config;

namespace Restaurant_WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Removing XML Formatter
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.Filters.Add(new LogRequestFilter());

            // Set JSON as the default formatter
            //config.Formatters.JsonFormatter.SupportedMediaTypes
            //    .Add(new MediaTypeHeaderValue("text/html"));

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
        }
    }
}
