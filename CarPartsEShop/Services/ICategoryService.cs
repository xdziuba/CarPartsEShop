using CarPartsEShop.Models;

namespace CarPartsEShop.Services
{
    public interface ICategoryService
    {
        public Task<List<Category>> GetAllAsync();
        Task<Category> GetAsync(int id);
        Task<Category> Update(Category category);
        Task<Category> Add(Category category);
    }
}
