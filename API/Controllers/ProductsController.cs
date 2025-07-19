using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly StoreContext context;

        public ProductsController(StoreContext context)
        {
            this.context = context;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts() //Ye normal function hi hai return type hai ek IEnumerable list Product type ka aur function ka naam hai GetProducts
        {
            return await context.Products.ToListAsync();//async await isiliyr use kr rhe hai kiuki agar maano bot badi query hai toh jb tak wo database se load nahi kr lega tab tak thread block ho jaygi. So database queries ke liye async await use kro.
        }
        [HttpGet("{id:int}")] // api/products/2
        public async Task<ActionResult<Product>> GetProduct(int id) // // This returns http response code like 200 OK with the Product
        {
            var product = await context.Products.FindAsync(id);

            if (product == null) return NotFound(); // agar null hua toh not found return kare 
            
            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product) // Returns either a Product or HTTP error.. We can also write without async Task<> but async Task<> is better for stability as it does not blockk the thread.
        {
            context.Products.Add(product);

            await context.SaveChangesAsync();

            return product;
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product) // This only includes http response code like 200
        {
            if (product.Id != id || !ProductExists(id))
                return BadRequest("Cannot update this Product");

            context.Entry(product).State = EntityState.Modified; // As we are geting product dirctly and not from the database so EF does't know if this already exist in the database
                                                                 // This tells the EF tracker that what we are passing is already in the database and just update it
                                                                 // there are others too like Added,Deleted,Unchanged, Detached.
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id) // This only includes http response code like 200
        {
            var product = await context.Products.FindAsync(id);

            if(product == null) return NotFound();

            context.Products.Remove(product);

            await context.SaveChangesAsync();

            return NoContent();

        }
        private bool ProductExists(int id)
        {
            return context.Products.Any(x=>x.Id == id);
        }
    }
}
