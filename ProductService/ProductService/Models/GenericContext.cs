using System.Data.Entity;

namespace ProductService.Models
{
    public class GenericContext : DbContext
    {
        public GenericContext() : base("name=GenericContext") { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Person> People { get; set; }
    }
}