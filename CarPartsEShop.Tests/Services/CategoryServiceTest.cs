using CarPartsEShop.Models;
using CarPartsEShop.Repositories;
using CarPartsEShop.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CarPartsEShop.Tests.Services
{
    public class CategoryServiceTests
    {
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // izolacja testów
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Add_ShouldAddCategory()
        {
            // Arrange
            var context = GetDbContext();
            var service = new CategoryService(context);
            var category = new Category { Name = "Brakes", Deleted = false };

            // Act
            var result = await service.Add(category);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Brakes", result.Name);
            Assert.False(result.Deleted);
            Assert.Single(context.Categories);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnOnlyNotDeletedCategories()
        {
            // Arrange
            var context = GetDbContext();
            context.Categories.AddRange(
                new Category { Name = "Engine", Deleted = false },
                new Category { Name = "Suspension", Deleted = true }
            );
            await context.SaveChangesAsync();

            var service = new CategoryService(context);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Engine", result[0].Name);
        }

        [Fact]
        public async Task GetAsync_ShouldReturnCategoryById()
        {
            // Arrange
            var context = GetDbContext();
            context.Categories.Add(new Category { Id = 1, Name = "Wheels", Deleted = false });
            await context.SaveChangesAsync();

            var service = new CategoryService(context);

            // Act
            var category = await service.GetAsync(1);

            // Assert
            Assert.NotNull(category);
            Assert.Equal("Wheels", category.Name);
        }

        [Fact]
        public async Task Update_ShouldModifyCategory()
        {
            // Arrange
            var context = GetDbContext();
            var original = new Category { Id = 1, Name = "Lights", Deleted = false };
            context.Categories.Add(original);
            await context.SaveChangesAsync();

            var service = new CategoryService(context);

            // Act
            original.Name = "Lighting";
            var updated = await service.Update(original);

            // Assert
            Assert.Equal("Lighting", updated.Name);
            var categoryFromDb = await context.Categories.FindAsync(1);
            Assert.Equal("Lighting", categoryFromDb.Name);
        }
    }
}
