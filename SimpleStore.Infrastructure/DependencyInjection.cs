// SimpleStore.Infrastructure/DependencyInjection.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleStore.Application.Interfaces.Auth;
using SimpleStore.Application.Interfaces.Repositories;
using SimpleStore.Domain.Entities;
using SimpleStore.Infrastructure;
using SimpleStore.Infrastructure.Auth;
using SimpleStore.Infrastructure.Repositories;

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


        services.AddScoped<IProductsWritableRepository, ProductsWritableRepository>();
        services.AddScoped<IProductsReadonlyRepository, ProductsReadonlyRepository>();

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICartRepository, CartRepository>();


        services.AddScoped<ITokenGenerator, TokenGenerator>();
        services.AddScoped<IAccessTokenService, AccessTokenService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IRefreshTokenValidator, RefreshTokenValidator>();
        services.AddScoped<ITokenIssuerService, TokenIssuerService>();

        return services;
    }
}