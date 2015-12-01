using ProductService.Models;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;

namespace ProductService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
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
