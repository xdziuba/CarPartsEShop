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
    public class CartControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public CartControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null) services.Remove(descriptor);
                    
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb"));
                });
            });

            _client = _factory.CreateClient();
        }

        private async Task<string> GetAuthToken()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Customers.Add(new Customer
            {
                FullName = "Pan Testowy",
                Email = "customer@wp.pl",
                Password = "1234",
                Role = "Customer"
            });
            await db.SaveChangesAsync();

            var loginDto = new LoginDto
            {
                Email = "customer@wp.pl",
                Password = "1234"
            };

            var response = await _client.PostAsJsonAsync("/api/authentication/login", loginDto);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task CreateCart_AsCustomer_ReturnsOk()
        {
            // Arrange
            var token = await GetAuthToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsync("/api/cart/create", null);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var cart = await response.Content.ReadFromJsonAsync<Cart>();
            Assert.NotNull(cart);
        }

        [Fact]
        public async Task GetMyCart_AsCustomer_ReturnsNotFoundOrOk()
        {
            // Arrange
            var token = await GetAuthToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/cart/my-cart");

            // Assert
            Assert.True(response.StatusCode is HttpStatusCode.NotFound or HttpStatusCode.OK);
        }

        [Fact]
        public async Task Checkout_WithoutCart_ReturnsBadRequest()
        {
            // Arrange
            var token = await GetAuthToken();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.PostAsync("/api/cart/checkout", null);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
