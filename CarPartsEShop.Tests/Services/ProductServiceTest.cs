using CarPartsEShop.Models;
using CarPartsEShop.Repositories;
using CarPartsEShop.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CarPartsEShop.Tests.Services
{
    public class ProductServiceTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unikalna baza na test
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Add_ShouldAddProduct()
        {
            // Arrange
            var context = GetDbContext();
            var service = new ProductService(context);
            var category = new Category { Id = 1, Name = "Brakes" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var product = new Product
            {
                Name = "Brake Pad",
                Price = 120,
                Stock = 5,
                CategoryId = 1,
                Deleted = false
            };

            // Act
            var result = await service.Add(product);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Brake Pad", result.Name);
            Assert.Single(context.Products);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnOnlyNotDeletedProducts()
        {
            // Arrange
            var context = GetDbContext();
            context.Products.AddRange(
                new Product { Name = "Oil Filter", Deleted = false },
                new Product { Name = "Wiper Blade", Deleted = true }
            );
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Oil Filter", result.First().Name);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnProductWithCategory()
        {
            // Arrange
            var context = GetDbContext();
            var category = new Category { Id = 1, Name = "Lighting" };
            var product = new Product
            {
                Id = 1,
                Name = "Headlight",
                CategoryId = 1,
                Deleted = false,
                Category = category
            };

            context.Categories.Add(category);
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            // Act
            var result = await service.GetAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Headlight", result.Name);
            Assert.NotNull(result.Category);
            Assert.Equal("Lighting", result.Category.Name);
        }

        [Fact]
        public async Task Update_ShouldModifyProduct()
        {
            // Arrange
            var context = GetDbContext();
            var product = new Product { Id = 1, Name = "Battery", Stock = 3, Deleted = false };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            // Act
            product.Stock = 10;
            product.Name = "Car Battery";
            var updated = await service.Update(product);

            // Assert
            Assert.Equal("Car Battery", updated.Name);
            Assert.Equal(10, updated.Stock);

            var fromDb = await context.Products.FindAsync(1);
            Assert.Equal("Car Battery", fromDb.Name);
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ShouldReturnCategory()
        {
            // Arrange
            var context = GetDbContext();
            var category = new Category { Id = 1, Name = "Filters" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var service = new ProductService(context);

            // Act
            var result = await service.GetCategoryByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Filters", result.Name);
        }
    }
}
