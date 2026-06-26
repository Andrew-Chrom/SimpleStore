using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;
using StackExchange.Redis;

namespace SimpleStore.Infrastructure.Repositories.Cached
{
    public class CachedCategoryRepository : ICategoryRepository
    {

        private readonly CategoryRepository _decorator;
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _multiplexer;
        public CachedCategoryRepository(
            CategoryRepository decorator,
            IDistributedCache cache,
            IConnectionMultiplexer multiplexer)
        {
            _decorator = decorator;
            _cache = cache;
            _multiplexer = multiplexer;
        }

        public async Task<List<Category>> GetAllAsync(int page = 1, int pageSize = 25, CancellationToken ct = default)
        {
            string key = $"category:page:{page}:size:{pageSize}";

            string cachedProducts = await _cache.GetStringAsync(key, ct);
            List<Category> products;

            if (string.IsNullOrEmpty(cachedProducts))
            {
                products = await _decorator.GetAllAsync(page, pageSize, ct);

                if (products.Count == 0)
                    return products;
                await _cache.SetStringAsync(
                    key,
                    JsonConvert.SerializeObject(products),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                    },
                ct);

                return products;
            }
            products = JsonConvert.DeserializeObject<List<Category>>(cachedProducts);
            return products;
        }
        public async Task<Category> GetByIdAsync(Guid id, CancellationToken ct)
        {
            string key = "category_" + id.ToString();

            string cachedCategory = await _cache.GetStringAsync(key, ct);
            Category category;

            if (string.IsNullOrEmpty(cachedCategory))
            {
                category = await _decorator.GetByIdAsync(id, ct);
                if (category is null)
                    return category;

                await _cache.SetStringAsync(
                    key,
                    JsonConvert.SerializeObject(category),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                    },
                ct);

                return category;
            }

            category = JsonConvert.DeserializeObject<Category>(cachedCategory);
            return category;
        }
        public async Task<Guid> AddAsync(Category category, CancellationToken ct)
        {
            await InvalidateProductCacheAsync(ct);
            return await _decorator.AddAsync(category, ct);
        }
        public async Task<Guid> UpdateAsync(Category category, CancellationToken ct)
        {
            var id = await _decorator.UpdateAsync(category, ct);
            await InvalidateProductCacheAsync(ct);
            await _cache.RemoveAsync($"category_{id}", ct);
            return id;  
        }
        public async Task DeleteAsync(Category category, CancellationToken ct)
        {
            await _decorator.DeleteAsync(category, ct);
            await InvalidateProductCacheAsync(ct);
            await _cache.RemoveAsync($"category_{category.Id}", ct);
        }
        private async Task InvalidateProductCacheAsync(CancellationToken ct)
        {
            var server = _multiplexer.GetServer(_multiplexer.GetEndPoints().First());
            var db = _multiplexer.GetDatabase();
            var keys = server.Keys(pattern: "category:page:*").ToArray();
            if (keys.Any())
                await db.KeyDeleteAsync(keys);
        }

    }
}
