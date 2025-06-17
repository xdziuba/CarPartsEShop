using CarPartsEShop.Models;
using CarPartsEShop.Repositories;

namespace CarPartsEShop.Seeders.Seeders
{
    public class CustomerSeeder : ISeeder
    {
        public async Task Seed(ApplicationDbContext context)
        {
            if (context.Customers.Any()) return;

            context.Customers.AddRange(
                new Customer
                {
                    FullName = "Test User",
                    Email = "user@example.com",
                    Phone = "123456789",
                    Street = "Testowa 1",
                    City = "Miastoq",
                    PostalCode = "00-000",
                    Country = "PL",
                    Role = "User",
                    Password = "123"
                },
                new Customer
                {
                    FullName = "Admin",
                    Email = "admin@example.com",
                    Phone = "987654321",
                    Street = "Adminowa 2",
                    City = "Miasto2",
                    PostalCode = "00-001",
                    Country = "PL",
                    Role = "Admin",
                    Password = "admin"
                }
            );

            await context.SaveChangesAsync();
        }
    }
}
