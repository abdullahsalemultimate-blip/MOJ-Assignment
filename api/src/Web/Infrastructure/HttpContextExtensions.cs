using System.Diagnostics;

namespace InventorySys.Web.Infrastructure;

public static class HttpContextExtensions
{
    public static string GetTraceId(this HttpContext context)
    {
        
        return Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;
    }
}
