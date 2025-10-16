using InventorySys.Application.Common.Interfaces;
using InventorySys.Domain.Constants;
using InventorySys.Infrastructure.Data;
using InventorySys.Infrastructure.Data.Interceptors;
using InventorySys.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("InventorySysDb");
        Guard.Against.Null(connectionString, message: "Connection string 'InventorySysDb' not found.");

        builder.Services.AddScoped<ISaveChangesInterceptor, SoftDeleteInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseSqlServer(connectionString);
            options.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        });


        builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

        builder.Services.AddScoped<ApplicationDbContextInitialiser>();

        builder.Services.AddAuthentication()
            .AddBearerToken(IdentityConstants.BearerScheme);

        builder.Services.AddAuthorizationBuilder();

        builder.Services
            .AddOptions<JwtOptions>()
            .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart(); 

        builder.Services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = IdentityConsts.PasswordRequireDigit;
                options.Password.RequireLowercase = IdentityConsts.PasswordRequireLowercase;
                options.Password.RequireNonAlphanumeric = IdentityConsts.PasswordRequireNonAlphanumeric;
                options.Password.RequireUppercase = IdentityConsts.PasswordRequireUppercase;
                options.Password.RequiredLength = IdentityConsts.PasswordRequiredLength;
                options.Password.RequiredUniqueChars = IdentityConsts.PasswordRequiredUniqueChars;

                options.Lockout.DefaultLockoutTimeSpan = IdentityConsts.DefaultLockoutTimeSpan;
                options.Lockout.MaxFailedAccessAttempts = IdentityConsts.MaxFailedAccessAttempts;
                options.Lockout.AllowedForNewUsers = IdentityConsts.LockoutAllowedForNewUsers;

                options.User.RequireUniqueEmail = IdentityConsts.UserRequireUniqueEmail;

                options.SignIn.RequireConfirmedEmail = IdentityConsts.SignInRequireConfirmedEmail;
                
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddSignInManager();

        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();
        builder.Services.AddTransient<ITokenService, TokenService>();

        builder.Services.AddAuthorization();
    }
}
