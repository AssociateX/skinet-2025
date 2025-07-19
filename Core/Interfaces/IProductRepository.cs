using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IProductRepository
    {
        Task<IReadOnlyList<Product>> GetProductsAsync();//IReadOnly because we will not using this function to modify anything, we can use IEnumerable,Icollection etc 
        Task<Product?> GetProductByIdAsync(int id);

        void AddProduct(Product product);//These are not async because we are not yet interacting with the databse, we will only be interacting with the DB when we would be saving it. So here we have nothing as a return type and we are passing a product as a parameter
        void UpdateProduct(Product product);

        void DeleteProduct(Product product);

        bool ProductExists(int id);

        Task<bool> SaveChangesAsync();
       
    }
}
