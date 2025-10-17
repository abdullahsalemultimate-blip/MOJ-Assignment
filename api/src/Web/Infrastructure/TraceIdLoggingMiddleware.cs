using Serilog.Context;

namespace InventorySys.Web.Infrastructure;

// Middleware that enriches Serilog logs with the current request TraceId.
public class TraceIdLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public TraceIdLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var traceId = context.GetTraceId();

            // Push TraceId into Serilog LogContext for the lifetime of this request
            using (LogContext.PushProperty("TraceId", traceId))
            {
                await _next(context);
            }
        }
    }
