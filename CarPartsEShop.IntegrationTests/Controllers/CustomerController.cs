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
    public class CustomerControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public CustomerControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb_Customer"));
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
                Email = "admin2@example.com",
                Password = "admin123",
                Role = "Admin"
            });
            await db.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Email = "admin2@example.com",
                Password = "admin123"
            };

            var response = await _client.PostAsJsonAsync("/api/authentication/login", loginDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task Post_AdminToken_CreatesCustomer()
        {
            // Arrange
            var token = await GetAdminToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var dto = new CreateCustomerDto
            {
                FullName = "John Test",
                Email = "john.test@example.com",
                Phone = "123456789",
                Street = "Main St",
                City = "City",
                PostalCode = "00-000",
                Country = "Country",
                Password = "password",
                Role = "Customer"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/customer", dto);

            // Assert
            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<Customer>();
            Assert.Equal("john.test@example.com", created?.Email);
        }

        [Fact]
        public async Task GetCustomer_AdminToken_ReturnsCustomer()
        {
            // Arrange
            var token = await GetAdminToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Customers.Add(new Customer
            {
                Id = 200,
                FullName = "Sample Customer",
                Email = "sample@example.com",
                Role = "Customer"
            });
            await db.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/customer/200");

            // Assert
            response.EnsureSuccessStatusCode();
            var customer = await response.Content.ReadFromJsonAsync<Customer>();
            Assert.Equal("sample@example.com", customer?.Email);
        }
    }
}