using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CarPartsEShop.Models;
using CarPartsEShop.Dtos;
using CarPartsEShop.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CarPartsEShop.IntegrationTests.Controllers
{
    public class ProductControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ProductControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb_Products"));
                });
            });

            _client = _factory.CreateClient();
        }

        private async Task<string> GetAdminToken()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Customers.Add(new Customer
            {
                FullName = "Admin Product",
                Email = "adminproduct@example.com",
                Password = "admin123",
                Role = "Admin"
            });
            await db.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Email = "adminproduct@example.com",
                Password = "admin123"
            };

            var response = await _client.PostAsJsonAsync("/api/authentication/login", loginDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task Get_ReturnsAllProducts()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Products.AddRange(
                new Product { Name = "Engine Oil", Deleted = false },
                new Product { Name = "Air Filter", Deleted = false }
            );
            await db.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/product");

            // Assert
            response.EnsureSuccessStatusCode();
            var products = await response.Content.ReadFromJsonAsync<List<Product>>();
            Assert.Equal(2, products?.Count);
        }

        [Fact]
        public async Task Get_ById_ReturnsProduct()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Products.Add(new Product { Id = 100, Name = "Coolant", Deleted = false });
            await db.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/product/100");

            // Assert
            response.EnsureSuccessStatusCode();
            var product = await response.Content.ReadFromJsonAsync<Product>();
            Assert.Equal("Coolant", product?.Name);
        }

        [Fact]
        public async Task Post_AdminToken_CreatesProduct()
        {
            // Arrange
            var token = await GetAdminToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var category = new Category { Id = 10, Name = "Lighting", Deleted = false };
            db.Categories.Add(category);
            await db.SaveChangesAsync();

            var productDto = new ProductDto
            {
                Name = "Headlamp",
                Description = "Front headlamp",
                Ean = "1234567890123",
                Price = 300,
                Stock = 15,
                CategoryId = 10,
                Deleted = false
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/product", productDto);

            // Assert
            response.EnsureSuccessStatusCode();
            var product = await response.Content.ReadFromJsonAsync<Product>();
            Assert.Equal("Headlamp", product?.Name);
        }
    }
}