using System.Net;
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
    public class AuthenticationControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public AuthenticationControllerIntegrationTests(WebApplicationFactory<Program> factory)
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

        [Fact]
        public async Task Register_ValidCustomer_ReturnsOk()
        {
            // Arrange
            var dto = new RegisterDto
            {
                FullName = "Pawel Dziuba",
                Email = "pawel@kochamuek.com",
                Password = "bezpieczne-haslo"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/authentication/register", dto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Successfully registered", content);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var dto = new LoginDto
            {
                Email = "panpawel@wp.pl",
                Password = "1234"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/authentication/login", dto);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Customers.Add(new Customer
            {
                FullName = "Pawel Dziuba",
                Email = "pawel@kochamuek.com",
                Password = "bezpieczne-haslo",
                Role = "Customer"
            });
            db.SaveChanges();

            var dto = new LoginDto
            {
                Email = "pawel@kochamuek.com",
                Password = "bezpieczne-haslo"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/authentication/login", dto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var token = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(token));
        }
    }
}
