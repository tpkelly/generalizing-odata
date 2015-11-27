using ProductService.Models;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;

namespace ProductService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            ODataModelBuilder builder = new ODataConventionModelBuilder();
            // All mappings of (path => model type) to be used in the app
            builder.EntitySet<Product>("Products");
            builder.EntitySet<Person>("People");

            config.MapODataServiceRoute(
                routeName: "ODataRoute",
                routePrefix: null,
                model: builder.GetEdmModel());

            // Can't use default to look up controller for a model with a generic type controller
            config.Services.Replace(typeof(IHttpControllerSelector), new CustomControllerSelector(config, builder.EntitySets));
        }
    }
}
