using ProductService.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;

namespace ProductService.Controllers
{
    public class GenericController<T> : ODataController where T: class, IndexedModel
    {
        GenericContext db = new GenericContext();

        private bool Exists(long key)
        {
            return TableForT().Any(p => p.Id == key);
        }

        private DbSet<T> TableForT()
        {
            return db.Set<T>();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        #region CRUD

        // E.g. GET http://localhost:55934/Products
        [EnableQuery] // EnableQuery allows filter, sort, page, top, etc.
        public IQueryable<T> Get()
        {
            return TableForT();
        }

        // E.g. GET http://localhost:55934/Products(1)
        [EnableQuery]
        public SingleResult<T> Get([FromODataUri] long key)
        {
            IQueryable<T> result = TableForT().Where(p => p.Id == key);
            return SingleResult.Create(result);
        }

        // E.g. POST http://localhost:55934/Products
        public async Task<IHttpActionResult> Post(T product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TableForT().Add(product);
            await db.SaveChangesAsync();
            return Created(product);
        }

        // E.g. PATCH http://localhost:55934/Products(1)
        public async Task<IHttpActionResult> Patch([FromODataUri] long key, Delta<T> product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await TableForT().FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }

            product.Patch(entity);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(entity);
        }

        // E.g. PUT http://localhost:55934/Products(1)
        public async Task<IHttpActionResult> Put([FromODataUri] long key, T update)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != update.Id)
            {
                return BadRequest();
            }

            db.Entry(update).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(key))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Updated(update);
        }

        // E.g. DELETE http://localhost:55934/Products(1)
        public async Task<IHttpActionResult> Delete([FromODataUri] long key)
        {
            var product = await TableForT().FindAsync(key);
            if (product == null)
            {
                return NotFound();
            }

            TableForT().Remove(product);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }

        #endregion
    }
}