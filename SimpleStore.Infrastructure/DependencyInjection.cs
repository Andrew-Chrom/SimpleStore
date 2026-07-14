using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleStore.Application.Interfaces.Auth;
using SimpleStore.Application.Interfaces.Orders;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Application.Interfaces.UnitOfWork;
using SimpleStore.Domain.Entities;
using SimpleStore.Domain.Stripe;
using SimpleStore.Infrastructure;
using SimpleStore.Infrastructure.Auth;
using SimpleStore.Infrastructure.OrderExpirationService;
using SimpleStore.Infrastructure.Repositories;
using SimpleStore.Infrastructure.Repositories.Cached;
using SimpleStore.Infrastructure.Stripe;
using StackExchange.Redis;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<User, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        services.AddScoped<QueryDbContext>();
        services.AddScoped<CommandDbContext>();

        services.AddScoped<ProductsWritableRepository>();
        services.AddScoped<IProductsWritableRepository, CachedProductWritableRepository>();
        services.AddScoped<ProductsReadonlyRepository>();
        services.AddScoped<IProductsReadonlyRepository, CachedProductReadableRepository>();

        services.AddScoped<CategoryRepository>();
        services.AddScoped<ICategoryRepository, CachedCategoryRepository>();

        services.AddScoped<ICartRepository, CartRepository>();
        
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();

        services.AddScoped<IWishlistRepository, WishlistRepository>();

        services.AddStackExchangeRedisCache(opt =>
        {
            string connection = configuration.GetConnectionString("RedisConnection");
            opt.Configuration = connection;
        });

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(configuration.GetConnectionString("RedisConnection")));

        //services.AddSingleton<IConnectionMultiplexer>(sp =>
        //{
        //    var redisConnection = configuration.GetConnectionString("RedisConnection");
        //    return ConnectionMultiplexer.Connect(redisConnection!);
        //});

        services.AddTransient<IOrderExpirationService, OrderExpirationService>();

        services.AddHangfire((sp, config) =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            config.UsePostgreSqlStorage(c =>
                c.UseNpgsqlConnection(connectionString));
        });

        services.AddHangfireServer();

        //RecurringJob.AddOrUpdate<IOrderExpirationService>(
        //    "cancel-expired-orders",
        //    x => x.CancelExpiredOrdersAsync(CancellationToken.None),
        //    Cron.MinuteInterval(1));

        services.AddScoped<IOrderService, StripeService>();
        services.AddScoped<IWebhookParser, StripeWebhook>();
        //services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<CommandDbContext>());
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<IAccessTokenService, AccessTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IRefreshTokenValidator, RefreshTokenValidator>();
        services.AddScoped<ITokenIssuerService, TokenIssuerService>();

        return services;
    }
}