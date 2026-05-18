using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleStore.Domain.Entities;
using SimpleStore.Infrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SimpleStore.Tests
{
    public class CartIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public CartIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetProductMoreThanAvailableStock_ReturnsBadRequest()
        {
            // Arrange

            var loginRequest = new
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe"
            };
            var registerContent = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");

            var registerResponse = await _client.PostAsync("/api/Auth/register", registerContent);
            registerResponse.EnsureSuccessStatusCode();

            var loginResponse = await _client.PostAsync("/api/Auth/login", registerContent);
            loginResponse.EnsureSuccessStatusCode();

            var loginResultJson = await loginResponse.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(loginResultJson);
            var token = jsonDoc.RootElement.GetProperty("accessToken").GetString();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var productId = Guid.NewGuid();
            using var scope = _factory.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<CommandDbContext>();
            var categoryId = Guid.NewGuid();
            db.Categories.Add(new Domain.Entities.Category { Id = categoryId, Name = "Test Category" });

            db.Products.Add(new Domain.Entities.Product
            {
                Id = productId,
                Name = "Test Product",
                StockQuantity = 5,
                Price = 10,
                Barcode = "1234567890",
                CategoryId = categoryId
            });
            await db.SaveChangesAsync();

            var requestContent = new
            {
                quantity = 10
            };
            var content = new StringContent(JsonSerializer.Serialize(requestContent), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync($"/api/cart/{productId}", null);
            var responseContent = await _client.PatchAsync($"/api/cart/{productId}", content);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, responseContent.StatusCode);
        }

        [Fact]
        public async Task DeleteCartItem_ExpectedNullInDb()
        {
            // Arrange

            var loginRequest = new
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe"
            };
            var registerContent = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");

            var registerResponse = await _client.PostAsync("/api/Auth/register", registerContent);
            registerResponse.EnsureSuccessStatusCode();

            var loginResponse = await _client.PostAsync("/api/Auth/login", registerContent);
            loginResponse.EnsureSuccessStatusCode();

            var loginResultJson = await loginResponse.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(loginResultJson);
            var token = jsonDoc.RootElement.GetProperty("accessToken").GetString();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var productId = Guid.NewGuid();
            using var scope = _factory.Services.CreateScope();

            var db = scope.ServiceProvider.GetRequiredService<CommandDbContext>();
            var categoryId = Guid.NewGuid();
            db.Categories.Add(new Domain.Entities.Category { Id = categoryId, Name = "Test Category" });

            db.Products.Add(new Domain.Entities.Product
            {
                Id = productId,
                Name = "Test Product",
                StockQuantity = 5,
                Price = 10,
                Barcode = "1234567890",
                CategoryId = categoryId
            });
            await db.SaveChangesAsync();

            // Act
            var postResponse = await _client.PostAsync($"/api/cart/{productId}", null);
            postResponse.EnsureSuccessStatusCode();

            var response = await _client.DeleteAsync($"/api/cart/{productId}", default);
            response.EnsureSuccessStatusCode();

            // Assert
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var user = await userManager.FindByEmailAsync("test@example.com");
            var userId = user.Id;

            Assert.Null(await db.CartItems.FirstOrDefaultAsync(i => i.UserId == userId));
        }

    }
}
