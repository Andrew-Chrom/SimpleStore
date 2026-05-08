using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using SimpleStore.Application.Dto.Product;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;


namespace SimpleStore.Infrastructure.Repositories.Cached
{
    public class CachedProductReadableRepository : IProductsReadonlyRepository
    {
        private readonly ProductsReadonlyRepository _decorator;
        private readonly IDistributedCache _cache;
        public CachedProductReadableRepository(
            ProductsReadonlyRepository decorator,
            IDistributedCache cache)
        {
            _decorator = decorator;
            _cache = cache;
        }
        public async Task<List<ProductListItemDto>> GetAllAsync(int page = 1, int pageSize = 25, CancellationToken ct = default)
        {
            // i guess i need to make filtering by pagination and somehow manage situation
            // when there are a few product pagination intersect
            string key = $"products:page:{page}:size:{pageSize}";

            string cachedProducts = await _cache.GetStringAsync(key, ct);
            List<ProductListItemDto> products;

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
            products = JsonConvert.DeserializeObject<List<ProductListItemDto>>(cachedProducts);
            return products;
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
    }
}
