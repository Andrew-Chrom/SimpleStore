using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;
using StackExchange.Redis;

namespace SimpleStore.Infrastructure.Repositories.Cached
{
    public class CachedProductWritableRepository : IProductsWritableRepository
    {
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _multiplexer;
        private readonly ProductsWritableRepository _decorator;
        public CachedProductWritableRepository(QueryDbContext db,
            ProductsWritableRepository decorator,
            IDistributedCache cache,
            IConnectionMultiplexer multiplexer)
        {
            _decorator = decorator;
            _cache = cache;
            _multiplexer = multiplexer;
        }
        public async Task<Product> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            string key = "product_" + id.ToString();

            string cachedProduct = await _cache.GetStringAsync(key, cancellationToken);
            Product product;

            if (string.IsNullOrEmpty(cachedProduct))
            {
                product = await _decorator.GetByIdAsync(id, cancellationToken);

                if (product is null)
                    return product;
                await _cache.SetStringAsync(
                    key,
                    JsonConvert.SerializeObject(product),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)
                    },
                cancellationToken);

                return product;
            }

            product = JsonConvert.DeserializeObject<Product>(cachedProduct);
            return product;
        }
        public async Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct)
        {
            return await _decorator.GetByIdsAsync(ids, ct);
        }
        public async Task UpdateRangeAsync(IEnumerable<Product> products, CancellationToken ct)
        {
            await _decorator.UpdateRangeAsync(products, ct);
        }
        public async Task UpdateAsync(Product product, CancellationToken ct)
        {
            await _decorator.UpdateAsync(product, ct);
            
            await _cache.RemoveAsync($"product_{product.Id}", ct);
            await InvalidateProductCacheAsync(ct);
        }

        public async Task DeleteAsync(Product product, CancellationToken ct)
        {
            await _decorator.DeleteAsync(product, ct);

            await _cache.RemoveAsync($"product_{product.Id}", ct);
            await InvalidateProductCacheAsync(ct);
        }

        public async Task<Guid> CreateAsync(Product product, CancellationToken ct)
        {
            await InvalidateProductCacheAsync(ct);
            return await _decorator.CreateAsync(product, ct);
        }
        private async Task InvalidateProductCacheAsync(CancellationToken ct)
        {
            var server = _multiplexer.GetServer(_multiplexer.GetEndPoints().First());
            var db = _multiplexer.GetDatabase();
            var keys = server.Keys(pattern: "products:page:*").ToArray();
            if (keys.Any())
                await db.KeyDeleteAsync(keys);
        }

    }
}
