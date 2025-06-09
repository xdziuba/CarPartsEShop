using CarPartsEShop.Models;
using CarPartsEShop.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarPartsEShop.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        public ProductService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Product> Add(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => !p.Deleted)
                .ToListAsync();
        }

        public async Task<Product> GetAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Where(x => x.Id == id && !x.Deleted).FirstOrDefaultAsync();
        }

        public async Task<Product> Update(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }
    }
}
