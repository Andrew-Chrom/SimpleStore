

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleStore.Domain.Entities;
using SimpleStore.Infrastructure;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SimpleStore.Tests
{
    public class OrdersIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public OrdersIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task OrdersWebhook_ChangesStatesFromPendingToCompleted()
        {
            // Arrange
            var loginRequest = new
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe"
            };
            var content = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");

            var registerResponse = await _client.PostAsync("/api/Auth/register", content);
            registerResponse.EnsureSuccessStatusCode();

            var loginResponse = await _client.PostAsync("/api/Auth/login", content);
            loginResponse.EnsureSuccessStatusCode();

            var loginResultJson = await loginResponse.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(loginResultJson);
            var token = jsonDoc.RootElement.GetProperty("accessToken").GetString();

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CommandDbContext>();

            var categoryId = Guid.NewGuid();
            db.Categories.Add(new Category { Id = categoryId, Name = "Laptops" });

            var productId = Guid.NewGuid();
            db.Products.Add(new Product
            {
                Id = productId,
                Name = "Gaming Laptop",
                CategoryId = categoryId,
                Barcode = "1234567890",
                Description = "A powerful gaming laptop",
                Price = 100.00m,
                StockQuantity = 9
            });

            await db.SaveChangesAsync();

            // Act

            var cartResponse = await _client.PostAsync($"/api/cart/{productId}", null);
            cartResponse.EnsureSuccessStatusCode();

            var changeQuantity = new
            {
                Quantity = 2
            };
            var request = new StringContent(JsonSerializer.Serialize(changeQuantity), Encoding.UTF8, "application/json");
            var quantityCartResponse = await _client.PatchAsync($"/api/cart/{productId}", request);
            var orderResponse = await _client.PostAsync("/api/orders", null);
            orderResponse.EnsureSuccessStatusCode();

            // Assert
            
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
            Assert.NotNull(user);

            Assert.Equal(orderResponse.StatusCode, System.Net.HttpStatusCode.OK);
            
            var order = await db.Orders.FirstOrDefaultAsync(o => o.UserId == user.Id);
            Assert.NotNull(order);

            Assert.Empty(await db.CartItems.Where(ci => ci.UserId == user.Id).ToListAsync());
            
            db.ChangeTracker.Clear();
            Assert.Equal(7, (await db.Products.FirstOrDefaultAsync(p => p.Id == productId)).StockQuantity);
            Assert.Equal(OrderStatus.Pending, order.Status);
        }
    }
}