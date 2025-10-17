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
using Microsoft.AspNetCore.Authorization;
using InventorySys.Domain.Constants;
using Microsoft.AspNetCore.Mvc.Authorization;

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

        builder.Services.AddControllers(options =>
        {
            // Fallback Policy for every controller and actions(only Admins are Allowed) .
            var policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .RequireRole(Roles.Administrator)
                .Build();

            options.Filters.Add(new AuthorizeFilter(policy));
        });

        builder.Services.AddJwtAuthentication(builder.Configuration);

        // Customise default API behaviour because i use FluentValidation library
        builder.Services.Configure<ApiBehaviorOptions>(options =>
            options.SuppressModelStateInvalidFilter = true);

     
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddOpenApiDocument((configure, sp) =>
        {
            configure.Title = "InventorySys API";
            configure.Description = @"Default Admin User Credintials 
                UserName : administrator@localhost
                Password : Administrator1!

                token for test: 
    
                Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiI0YzNlODk3NS0wYjhjLTQ3ZTMtODE4NS01ZWE3ZmU5MGFhNTgiLCJzdWIiOiI0YzNlODk3NS0wYjhjLTQ3ZTMtODE4NS01ZWE3ZmU5MGFhNTgiLCJ1bmlxdWVfbmFtZSI6ImFkbWluaXN0cmF0b3JAbG9jYWxob3N0IiwianRpIjoiN2RjZTdhNDUtODdjMC00YzhjLWFmNDQtNDI4M2NkZGUzMDM4Iiwicm9sZSI6IkFkbWluaXN0cmF0b3IiLCJuYmYiOjE3NjA2NjYwNjksImV4cCI6MzkyMDY2NjA2OSwiaWF0IjoxNzYwNjY2MDY5LCJpc3MiOiJJbnZlbnRvcnlTeXMiLCJhdWQiOiJJbnZlbnRvcnlTeXMifQ.IB0TF1PUFBcAIsMVfmXj9w9NB1fFU1Ksv9JdOihN9eY

            ";

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
