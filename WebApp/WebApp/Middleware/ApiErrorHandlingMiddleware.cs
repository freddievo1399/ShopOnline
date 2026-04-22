using ShopOnline.Abstractions.Constants;
using ShopOnline.Abstractions.Wrappers;

namespace WebApp.Middleware;

public class ApiErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiErrorHandlingMiddleware> _logger;

    public ApiErrorHandlingMiddleware(RequestDelegate next, ILogger<ApiErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
            if (!context.Response.HasStarted)
            {
                switch (context.Response.StatusCode)
                {
                    case StatusCodes.Status404NotFound:
                        await WriteError(context, ResultCode.NotFound);
                        break;
                    case StatusCodes.Status401Unauthorized:
                        await WriteError(context, ResultCode.Unauthenticated);
                        break;
                    case StatusCodes.Status403Forbidden:
                        await WriteError(context, ResultCode.AccessDenied);
                        break;
                    default:
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            // 3. Handle Exception bất ngờ (Lỗi 500)
            _logger.LogError(ex, "Hệ thống gặp sự cố tại: {Path}", context.Request.Path);
            Console.WriteLine(ex.Message);
            await WriteError(context, ResultCode.SystemError);
        }
    }

    private async Task WriteError(HttpContext context, string message)
    {
        context.Response.ContentType = "application/json";

        context.Response.StatusCode = StatusCodes.Status200OK;

        var result = new Result
        {
            Success = false,
            Message = message
        };

        await context.Response.WriteAsJsonAsync(result);
    }
}
