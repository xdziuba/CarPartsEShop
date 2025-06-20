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
    public class OrdersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public OrdersControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb_Orders"));
                });
            });

            _client = _factory.CreateClient();
        }

        private async Task<string> GetCustomerToken()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var customer = new Customer
            {
                Id = 300,
                FullName = "Order Tester",
                Email = "ordertest@example.com",
                Password = "test123",
                Role = "Customer"
            };
            db.Customers.Add(customer);
            db.Orders.Add(new Order
            {
                CustomerId = 300,
                Items = new List<OrderItem>
                {
                    new OrderItem
                    {
                        Product = new Product { Name = "Brake Disc", Price = 250, Stock = 10 },
                        Quantity = 2,
                        Price = 250
                    }
                }
            });
            await db.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Email = "ordertest@example.com",
                Password = "test123"
            };

            var response = await _client.PostAsJsonAsync("/api/authentication/login", loginDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task GetOrders_WithValidCustomerToken_ReturnsOrders()
        {
            // Arrange
            var token = await GetCustomerToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/orders");

            // Assert
            response.EnsureSuccessStatusCode();
            var orders = await response.Content.ReadFromJsonAsync<List<Order>>();
            Assert.NotNull(orders);
            Assert.Single(orders);
            Assert.Equal("Brake Disc", orders[0].Items[0].Product.Name);
        }

        [Fact]
        public async Task GetOrders_WithNoOrders_ReturnsNotFound()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Customers.Add(new Customer
            {
                Id = 301,
                FullName = "Empty Customer",
                Email = "empty@example.com",
                Password = "nopass",
                Role = "Customer"
            });
            await db.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Email = "empty@example.com",
                Password = "nopass"
            };
            var loginResponse = await _client.PostAsJsonAsync("/api/authentication/login", loginDto);
            loginResponse.EnsureSuccessStatusCode();
            var token = await loginResponse.Content.ReadAsStringAsync();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/orders");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
