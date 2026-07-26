using System.Diagnostics;

namespace CMDocumentRepository.Presentation.Middleware;

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
        var stopwatch = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString();

        context.Items["RequestId"] = requestId;

        _logger.LogInformation("[{RequestId}] {Method} {Path}",
            requestId, context.Request.Method, context.Request.Path);

        await _next(context);

        stopwatch.Stop();
        _logger.LogInformation("[{RequestId}] {StatusCode} за {ElapsedMs}мс",
            requestId, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
    }
}
