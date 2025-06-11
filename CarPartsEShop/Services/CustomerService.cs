using CarPartsEShop.Models;
using CarPartsEShop.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarPartsEShop.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ApplicationDbContext _context;
        public CustomerService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Customer?> GetCustomerAsync(int customerId)
        {
            var customer = await _context.Customers
                .Include(c => c.Cart)
                .ThenInclude(cart => cart.Items)
                .FirstOrDefaultAsync(c => c.Id == customerId);
            return customer ?? null;
        }

        public async Task<Customer> CreateCustomerAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return customer;
        }
    }
}
