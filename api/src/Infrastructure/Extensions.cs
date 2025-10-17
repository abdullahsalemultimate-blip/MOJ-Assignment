using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigurationExtensions 
{
    public static string GetApplicationDatabaseConnectionString(this IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("InventorySysDb");
        Guard.Against.Null(connectionString, message: "Connection string 'InventorySysDb' not found.");
       return connectionString;
    }

}
