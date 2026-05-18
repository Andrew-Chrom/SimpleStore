using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SimpleStore.Infrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace SimpleStore.Tests
{
    public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public AuthIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsOkAndCreatesUser()
        {
            var request = new
            {
                Email = "test@example.com",
                Password = "Password123!",
                FirstName = "John",
                LastName = "Doe"
            };
            var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Auth/register", content);

            response.EnsureSuccessStatusCode(); 

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var userInDb = await db.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
            Assert.NotNull(userInDb);
        }

        [Fact]
        public async Task Roles_WithCustomer_ReturnsForbiddenWhenCreateCategory()
        {
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

            var categoryRequest = new { Name = "Test Category" };
            var categoryContent = new StringContent(JsonSerializer.Serialize(categoryRequest), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/Categories", categoryContent);

            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

    }
}
