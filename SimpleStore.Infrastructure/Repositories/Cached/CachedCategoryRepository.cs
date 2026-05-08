using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;

namespace SimpleStore.Infrastructure.Repositories.Cached
{
    public class CachedCategoryRepository : ICategoryRepository
    {

        private readonly CategoryRepository _decorator;
        private readonly IDistributedCache _cache;
        public CachedCategoryRepository(
            CategoryRepository decorator,
            IDistributedCache cache)
        {
            _decorator = decorator;
            _cache = cache;
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
            return await _decorator.AddAsync(category, ct);
        }
        public async Task<Guid> UpdateAsync(Category category, CancellationToken ct)
        {
            var id = await _decorator.UpdateAsync(category, ct);
            await _cache.RemoveAsync($"category_{id}", ct);
            return id;  
        }
        public async Task DeleteAsync(Category category, CancellationToken ct)
        {
            await _decorator.DeleteAsync(category, ct);
            await _cache.RemoveAsync($"category_{category.Id}", ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            await _decorator.SaveChangesAsync(ct);
        }

        private async Task InvalidateProductCacheAsync(CancellationToken ct)
        {
            var server = _multiplexer.GetServer(_multiplexer.GetEndPoints().First());
            var keys = server.Keys(pattern: "products:page:*").ToArray();
            await _db.KeyDeleteAsync(keys);
        }

    }
}
