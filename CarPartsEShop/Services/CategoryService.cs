using CarPartsEShop.Models;
using CarPartsEShop.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarPartsEShop.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        public CategoryService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Category> Add(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Where(p => !p.Deleted)
                .ToListAsync();
        }

        public async Task<Category> GetAsync(int id)
        {
            return await _context.Categories
                .Where(p => !p.Deleted)
                .Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Category> Update(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }
    }
}
