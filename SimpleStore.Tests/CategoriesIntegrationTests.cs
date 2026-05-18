using Microsoft.Extensions.DependencyInjection;
using SimpleStore.Domain.Entities;
using SimpleStore.Infrastructure;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Tests
{
    public class CategoriesIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public CategoriesIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetCategories_CachesResultInRedis()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CommandDbContext>();
            db.Categories.Add(new Category { Id = Guid.NewGuid(), Name = "Laptops" });
            await db.SaveChangesAsync();

            var redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>().GetDatabase();

            // Act
            var response = await _client.GetAsync("/api/categories?page=1&pageSize=25");
            response.EnsureSuccessStatusCode();

            // Assert
            var cachedData = await redis.HashGetAsync("category:page:1:size:25", "data");

            Assert.True(cachedData.HasValue);
            Assert.Contains("Laptops", cachedData.ToString());
        }
    }
}
