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
        private IDictionary<string, HttpControllerDescriptor> _controllerMappings;

        public CustomControllerSelector(HttpConfiguration configuration, IEnumerable<EntitySetConfiguration> entitySets)
        {
            _controllerMappings = GenerateMappings(configuration, entitySets);
        }

        private IDictionary<string, HttpControllerDescriptor> GenerateMappings(HttpConfiguration config, IEnumerable<EntitySetConfiguration> entitySets)
        {
            IDictionary<string, HttpControllerDescriptor> dictionary = new Dictionary<string, HttpControllerDescriptor>();

            // Map root controller
            dictionary.Add("", new HttpControllerDescriptor(config, "Index", typeof(IndexController)));

            foreach (EntitySetConfiguration set in entitySets)
            {
                var genericControllerDescription = new HttpControllerDescriptor(config, set.Name, typeof(GenericController<>).MakeGenericType(set.ClrType));
                dictionary.Add(set.Name, genericControllerDescription);
            }

            return dictionary;
        }

        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var path = request.RequestUri.LocalPath.Split('/','(');
            return _controllerMappings[path[1]];
        }

        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            return _controllerMappings;
        }
    }
}