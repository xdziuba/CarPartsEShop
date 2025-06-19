using CarPartsEShop.Models;
using CarPartsEShop.Repositories;
using CarPartsEShop.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CarPartsEShop.Tests.Services
{
    public class CartServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // izolacja testów
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task AddItemAsync_ProductExists_AddsItemToCart()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Products.Add(new Product { Id = 1, Name = "Oil Filter", Price = 50, Stock = 10 });
            context.Carts.Add(new Cart { Id = 1, CustomerId = 1 });
            await context.SaveChangesAsync();

            var service = new CartService(context);

            // Act
            var item = await service.AddItemAsync(1, 1, 2);

            // Assert
            Assert.NotNull(item);
            Assert.Equal(1, item.CartId);
            Assert.Equal(1, item.ProductId);
            Assert.Equal(2, item.Quantity);
        }

        [Fact]
        public async Task CheckoutAsync_CartWithSufficientStock_ReturnsSuccess()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var customer = new Customer { Id = 1 };
            var product = new Product { Id = 1, Name = "Brake Pad", Stock = 10, Price = 200 };
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

            var service = new CartService(context);

            // Act
            var result = await service.CheckoutAsync(1);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Checkout successful.", result.Msg);

            var updatedProduct = await context.Products.FindAsync(1);
            Assert.Equal(8, updatedProduct.Stock); // 10 - 2 = 8
            var orders = context.Orders.Include(o => o.Items).ToList();
            Assert.Single(orders);
            Assert.Empty(cart.Items);
        }

        [Fact]
        public async Task CheckoutAsync_CartWithInsufficientStock_ReturnsError()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var customer = new Customer { Id = 1 };
            var product = new Product { Id = 1, Name = "Air Filter", Stock = 1, Price = 100 };
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

            var service = new CartService(context);

            // Act
            var result = await service.CheckoutAsync(1);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("Insufficient stock", result.Msg);
        }
    }
}
