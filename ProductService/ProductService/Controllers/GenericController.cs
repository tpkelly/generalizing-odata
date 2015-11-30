using ProductService.Models;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

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

        // E.g. GET http://localhost/Products
        [EnableQuery] // EnableQuery allows filter, sort, page, top, etc.
        public IQueryable<T> Get()
        {
            return TableForT();
        }

        // E.g. GET http://localhost/Products(1)
        [EnableQuery]
        public SingleResult<T> Get([FromODataUri] long key)
        {
            IQueryable<T> result = Get().Where(p => p.Id == key);
            return SingleResult.Create(result);
        }

        // E.g. POST http://localhost/Products
        public async Task<IHttpActionResult> Post(T obj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            TableForT().Add(obj);
            await db.SaveChangesAsync();
            return Created(obj);
        }

        // E.g. PATCH http://localhost/Products(1)
        public async Task<IHttpActionResult> Patch([FromODataUri] long key, Delta<T> delta)
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

            delta.Patch(entity);

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

        // E.g. PUT http://localhost/Products(1)
        public async Task<IHttpActionResult> Put([FromODataUri] long key, T obj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != obj.Id)
            {
                return BadRequest();
            }

            db.Entry(obj).State = EntityState.Modified;

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

            return Updated(obj);
        }

        // E.g. DELETE http://localhost/Products(1)
        public async Task<IHttpActionResult> Delete([FromODataUri] long key)
        {
            var entity = await TableForT().FindAsync(key);
            if (entity == null)
            {
                return NotFound();
            }

            TableForT().Remove(entity);
            await db.SaveChangesAsync();
            return StatusCode(HttpStatusCode.NoContent);
        }

        #endregion
    }
}