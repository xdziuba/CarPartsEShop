using CarPartsEShop.Models;

namespace CarPartsEShop.Services
{
    public interface ICustomerService
    {
        Task<Customer?> GetCustomerAsync(int customerId);
        Task<Customer> CreateCustomerAsync(Customer customer);
    }
}
