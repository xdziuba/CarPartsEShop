using CarPartsEShop.Models;

namespace CarPartsEShop.Services
{
    public interface IProductService
    {
        public Task<List<Product>> GetAllAsync();
        Task<Product> GetAsync(int id);
        Task<Product> Update(Product product);
        Task<Product> Add(Product product);
        Task<Category?> GetCategoryByIdAsync(int id);
    }
}
