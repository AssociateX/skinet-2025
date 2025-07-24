using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController(IProductRepository repo) : ControllerBase //Primary constructor approach in dependancy injection; we deleted the ctor which took private field and added a class parameter
    {
        
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Product>>> GetProducts(string? brand,string? type, string? sort) //Ye normal function hi hai return type hai ek IReadOnlyList list Product type ka aur function ka naam hai GetProducts
        {
            //return await context.Products.ToListAsync();   //async await isiliyr use kr rhe hai kiuki agar maano bot badi query hai toh jb tak wo database se load nahi kr lega tab tak thread block ho jaygi. So database queries ke liye async await use kro.
           
            return Ok(await repo.GetProductsAsync(brand, type, sort)); // isme type issue aata hai isiliye Ok() ke andar wrap krdia hai; Ye repository pattern ke baad ke changes hai
        }
        
        [HttpGet("{id:int}")] // api/products/2
        public async Task<ActionResult<Product>> GetProduct(int id) // // This returns http response code like 200 OK with the Product
        {
            var product = await repo.GetProductByIdAsync(id);

            if (product == null) return NotFound(); // agar null hua toh not found return kare 
            
            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product) // Returns either a Product or HTTP error.. We can also write without async Task<> but async Task<> is better for stability as it does not blockk the thread.
        {
            repo.AddProduct(product);

            if (await repo.SaveChangesAsync()) //This saves changes to the database. True/false (success/failed)
            {
                return CreatedAtAction("GetProduct", new { id = product.Id }, product); //CreatedAtAction() is a helper method, actionName: Name of the action method that returns the created resource (usually a GET method like GetProduct), routeValues: An object containing route parameters (e.g., new { id = product.Id }), value: The response body (usually the newly created object)..
            }

            return BadRequest("Problem creating product");
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product) // This only includes http response code like 200
        {
            if (product.Id != id || !ProductExists(id))
                return BadRequest("Cannot update this Product");

            repo.UpdateProduct(product); // As we are geting product dirctly and not from the database so EF does't know if this already exist in the database
                                         // This tells the EF tracker that what we are passing is already in the database and just update it
                                         // there are others too like Added,Deleted,Unchanged, Detached.
            if (await repo.SaveChangesAsync()) //This saves changes to the database. True/false (success/failed)
            {
                return NoContent(); 
            }

            return BadRequest("Problem updating the product");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id) // This only includes http response code like 200
        {
            var product = await repo.GetProductByIdAsync(id);

            if(product == null) return NotFound();

            repo.DeleteProduct(product);

            if (await repo.SaveChangesAsync()) //This saves changes to the database. True/false (success/failed)
            {
                return NoContent();
            }

            return BadRequest("Problem deleting the product");

        }
        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetBrands()
        {
            return Ok(await repo.GetBrandsAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<string>>> GetTypes()
        {
            return Ok(await repo.GetTypesAsync());
        }
        private bool ProductExists(int id)
        {
            return repo.ProductExists(id);
        }
    }
}
