using CarPartsEShop.Models;
using CarPartsEShop.Repositories;
using CarPartsEShop.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CarPartsEShop.Tests.Services
{
    public class CustomerServiceTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // izolowany test
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateCustomerAsync_ShouldAddCustomer()
        {
            // Arrange
            var context = GetDbContext();
            var service = new CustomerService(context);
            var customer = new Customer { FullName = "Jan Kowalski" };

            // Act
            var result = await service.CreateCustomerAsync(customer);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Jan Kowalski", result.FullName);
            Assert.Single(context.Customers);
        }

        [Fact]
        public async Task GetCustomerAsync_ShouldReturnCustomerWithCartAndItems()
        {
            // Arrange
            var context = GetDbContext();

            var customer = new Customer { Id = 1, FullName = "Anna Nowak" };
            var product = new Product { Id = 1, Name = "Oil Filter", Stock = 10, Price = 49.99M };
            var cart = new Cart
            {
                Id = 1,
                CustomerId = 1,
                Items = new List<CartItem>
                {
                    new CartItem { ProductId = 1, Quantity = 2, Product = product }
                }
            };

            context.Customers.Add(customer);
            context.Products.Add(product);
            context.Carts.Add(cart);
            await context.SaveChangesAsync();

            var service = new CustomerService(context);

            // Act
            var result = await service.GetCustomerAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Anna Nowak", result.FullName);
            Assert.NotNull(result.Cart);
            Assert.Single(result.Cart.Items);
            Assert.Equal(2, result.Cart.Items.First().Quantity);
        }

        [Fact]
        public async Task GetCustomerAsync_ShouldReturnNull_WhenCustomerNotExists()
        {
            // Arrange
            var context = GetDbContext();
            var service = new CustomerService(context);

            // Act
            var result = await service.GetCustomerAsync(99);

            // Assert
            Assert.Null(result);
        }
    }
}
