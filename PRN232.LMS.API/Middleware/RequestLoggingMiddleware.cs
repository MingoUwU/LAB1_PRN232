using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PRN232.LMS.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            
            await _next(context);

            sw.Stop();
            
            _logger.LogInformation(
                "Request Path: {Path} | HTTP Method: {Method} | Status Code: {StatusCode} | Execution Time: {ElapsedMilliseconds} ms",
                context.Request.Path,
                context.Request.Method,
                context.Response.StatusCode,
                sw.ElapsedMilliseconds);
        }
    }
}
