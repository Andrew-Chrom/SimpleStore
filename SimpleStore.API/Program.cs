using FluentValidation;
using Hangfire;
using JasperFx.CodeGeneration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SimpleStore.API.Query.Products;
using SimpleStore.API.Services;
using SimpleStore.Application.Command.Products;
using SimpleStore.Application.Query.Auth;
using SimpleStore.Application.Validators;
using SimpleStore.Domain.Constants;
using SimpleStore.Domain.Options;
using SimpleStore.Infrastructure;
using SimpleStore.Infrastructure.Options;
using SimpleStore.Infrastructure.OrderExpirationService;
using System.Text;
using Wolverine;
using Wolverine.FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Host.SerilogConfiguration();

builder.Services.AddValidatorsFromAssemblyContaining<CreateCategoryCommandValidator>();

builder.Host.UseWolverine(opts =>
{
    opts.UseRuntimeCompilation();

    opts.Discovery.IncludeAssembly(typeof(GetProductsHandler).Assembly);
    opts.Discovery.IncludeAssembly(typeof(CreateProductHandler).Assembly);
    opts.Discovery.IncludeAssembly(typeof(LoginHandler).Assembly);

    //opts.CodeGeneration.TypeLoadMode = TypeLoadMode.Auto;
    opts.CodeGeneration.AlwaysUseServiceLocationFor<Microsoft.EntityFrameworkCore.DbContextOptions<SimpleStore.Infrastructure.ApplicationDbContext>>();
    opts.UseFluentValidation(RegistrationBehavior.ExplicitRegistration);
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddOptions<StripeSettings>()
    .Bind(builder.Configuration.GetSection("StripeSettings"));

builder.Services.AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection("JwtSettings"))
    .ValidateDataAnnotations()
    .Validate(s => s.AccessTokenSecret.Length >= 32, "AccessTokenSecret must be at least 32 characters long")
    .Validate(s => s.RefreshTokenSecret.Length >= 32, "RefreshTokenSecret must be at least 32 characters long")
    .ValidateOnStart();

var jwtSettings = builder.Configuration
    .GetSection("JwtSettings")
    .Get<JwtSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.SaveToken = true;
    x.RequireHttpsMetadata = false;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.AccessTokenSecret)),

        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        if (exception is FluentValidation.ValidationException validationException)
        {
            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            context.Response.ContentType = "application/json";

            var errors = validationException.Errors.Select(e => e.ErrorMessage).ToList();

            await context.Response.WriteAsJsonAsync(new
            {
                Id = "Validation.Error",
                Type = 422,
                Description = string.Join(", ", errors)
            });
        }
    });
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    foreach (var role in new[] { Roles.Admin, Roles.Customer })
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
    }
}

app.UseHangfireDashboard();

var recurringJobManager = app.Services.GetRequiredService<IRecurringJobManager>();
recurringJobManager.AddOrUpdate<OrderExpirationService>(
    "cancel-expired-orders",
    x => x.CancelExpiredOrdersAsync(CancellationToken.None),
    Cron.MinuteInterval(1));

//using (var scope = app.Services.CreateScope())
//{
//    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

//    foreach (var role in new[] { Roles.Admin, Roles.Customer })
//    {
//        if (!await roleManager.RoleExistsAsync(role))
//            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
//    }
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();

    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SimpleStore API V1");
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
