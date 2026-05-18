using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.DependencyInjection;
using SimpleStore.Domain.Entities;
using SimpleStore.Infrastructure;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace SimpleStore.Tests
{
    public class ProductsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public ProductsIntegrationTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetProductsAndEditProduct_CachesResultInRedis_ReturnsNull()
        {
            // Arrange
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CommandDbContext>();
            
            var categoryId = Guid.NewGuid();
            db.Categories.Add(new Category { Id = categoryId, Name = "Laptops" });
            
            var productId = Guid.NewGuid();
            db.Products.Add(new Product { 
                Id = productId, 
                Name = "Gaming Laptop", 
                CategoryId = categoryId,
                Barcode = "1234567890",
                Description = "A powerful gaming laptop",
                Price = 100.00m,
                StockQuantity = 9
            });

            await db.SaveChangesAsync();

            var redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>().GetDatabase();

            // Act
            var getResponse = await _client.GetAsync("/api/products?page=1&pageSize=25");
            getResponse.EnsureSuccessStatusCode();
            
            var changeProduct = new Product {
                Name = "Gaming Laptop",
                CategoryId = db.Categories.First().Id,
                Barcode = "1234567890",
                Description = "A powerful gaming laptop",
                Price = 150.00m,
                StockQuantity = 9
            };
            var request = new StringContent(JsonSerializer.Serialize(changeProduct), Encoding.UTF8, "application/json");
            var putResponse = await _client.PutAsync($"/api/products/{productId}", request);

            // Assert
            var cachedData = await redis.HashGetAsync("product:page:1:size:25", "data");
            Assert.True(cachedData.IsNull);
        }
    }
}
