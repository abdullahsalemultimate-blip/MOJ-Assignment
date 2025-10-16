using System.Text;
using InventorySys.Application.Common.Interfaces;
using InventorySys.Infrastructure.Data;
using InventorySys.Web.Services;
using Microsoft.AspNetCore.Mvc;
using NSwag;
using NSwag.Generation.Processors.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using InventorySys.Infrastructure.Identity;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddWebServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddScoped<ICurrentUser, CurrentUser>();
        
        builder.Services.AddScoped<IRequestParams, RequestParams>();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddHealthChecks()
            .AddDbContextCheck<ApplicationDbContext>();

        builder.Services.AddExceptionHandler<CustomExceptionHandler>();


        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                corsPolicyBuilder =>
                {
                    corsPolicyBuilder.AllowAnyOrigin(); // This Just For Demo Purpose but in real scenario i will allow specific Origins 
                    corsPolicyBuilder.AllowAnyMethod();
                    corsPolicyBuilder.AllowAnyHeader();
                    corsPolicyBuilder.SetIsOriginAllowed(_ => true);
                });
        });

        builder.Services.AddControllers();

        builder.Services.AddJwtAuthentication(builder.Configuration);

        // Customise default API behaviour because i use FluentValidation library
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

     
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApiDocument((configure, sp) =>
        {
            configure.Title = "InventorySys API";

            configure.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
            {
                Type = OpenApiSecuritySchemeType.ApiKey,
                Name = "Authorization",
                In = OpenApiSecurityApiKeyLocation.Header,
                Description = "Type into the textbox: Bearer {your JWT token}."
            });

            configure.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("JWT"));
        });
    }

    
    private static void AddJwtAuthentication(this IServiceCollection services, IConfigurationManager configuration)
    {
      
        var jwtSettings = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
        Guard.Against.Null(jwtSettings);

        var key = Encoding.ASCII.GetBytes(jwtSettings.Key);
        
        services.AddAuthentication(config =>
        {
            config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme; 
            config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; 
        })
        .AddJwtBearer(config =>
        {
            config.RequireHttpsMetadata = false;
            config.SaveToken = true;
            config.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),

                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,

                ValidateLifetime = true,
            };
        });  
    }
}
