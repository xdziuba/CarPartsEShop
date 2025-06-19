using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CarPartsEShop.Models;
using CarPartsEShop.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.Text.Json;
using System.Text;
using CarPartsEShop.Repositories;

namespace CarPartsEShop.IntegrationTests.Controllers
{
    public class CategoryControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public CategoryControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb_Category"));
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
                FullName = "Admin User",
                Email = "admin@example.com",
                Password = "admin123",
                Role = "Admin"
            });
            await db.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Email = "admin@example.com",
                Password = "admin123"
            };

            var response = await _client.PostAsJsonAsync("/api/authentication/login", loginDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task Get_ReturnsAllCategories()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Categories.AddRange(
                new Category { Name = "Engine Parts", Deleted = false },
                new Category { Name = "Suspension", Deleted = false }
            );
            await db.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/category");

            // Assert
            response.EnsureSuccessStatusCode();
            var categories = await response.Content.ReadFromJsonAsync<List<Category>>();
            Assert.Equal(2, categories?.Count);
        }

        [Fact]
        public async Task Post_AdminToken_AddsCategory()
        {
            // Arrange
            var token = await GetAdminToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var category = new Category { Name = "Brakes" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/category", category);

            // Assert
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<Category>();
            Assert.Equal("Brakes", created?.Name);
        }

        [Fact]
        public async Task Get_ById_ReturnsCategory()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Categories.Add(new Category { Id = 10, Name = "Filters", Deleted = false });
            await db.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/category/10");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<Category>();
            Assert.Equal("Filters", result?.Name);
        }
    }
}