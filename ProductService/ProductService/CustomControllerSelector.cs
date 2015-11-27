using ProductService.Controllers;
using ProductService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.OData.Builder;

namespace ProductService
{
    public class CustomControllerSelector : IHttpControllerSelector
    {
        private HttpConfiguration _config;
        private IEnumerable<EntitySetConfiguration> _entitySets;

        public IHttpControllerSelector PreviousSelector { get; set; }

        public CustomControllerSelector(HttpConfiguration configuration, IEnumerable<EntitySetConfiguration> entitySets)
        {
            _config = configuration;
            _entitySets = entitySets;
        }

        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var controllers = GetControllerMapping();
            var path = request.RequestUri.LocalPath.Split('/','(');
            if (path.Length < 2)
            {
                // TODO: Should return home controller
            }

            return controllers[path[1]];
        }

        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            var dictionary = new Dictionary<string, HttpControllerDescriptor>();
            foreach (EntitySetConfiguration set in _entitySets) {
                var genericControllerDescription = new HttpControllerDescriptor(_config, set.Name, typeof(GenericController<>).MakeGenericType(set.ClrType));
                dictionary.Add(set.Name, genericControllerDescription);
            }

            return dictionary;
        }
    }
}