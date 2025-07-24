using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext context) // Returns an Asynchronous Task and does not return any value
        {
            if (!context.Products.Any())
            {
                var productsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");

                var products = JsonSerializer.Deserialize<List<Product>>(productsData);//A method that converts (deserializes) the JSON string into a .NET object or collection.productsData-	A string variable that contains JSON data 
                //	Tells the deserializer to convert the JSON into a List<Product> (a collection of Product objects).

                if (products == null)
                {
                    return;
                }
                context.Products.AddRange(products); //AddRange(products):- Adds a collection of Product objects (in this case, the products list from the previous deserialization step) to the context so they are marked for insertion into the database (In Memory).
                await context.SaveChangesAsync(); //(Now the changes are written to the database) without this method the changes will not be written to the database.
            }

        }
    }
}
