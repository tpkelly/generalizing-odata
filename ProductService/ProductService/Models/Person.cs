using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductService.Models
{
    public class Person : IndexedModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}