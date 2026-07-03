using System.Net;
using System.Text.Json;
using ProjectManager.Application.Common.Exceptions;

namespace ProjectManager.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        HttpStatusCode statusCode;
        string message;

        try
        {
            await _next(context);
            return;
        }
        catch (NotFoundException ex)
        {
            statusCode = HttpStatusCode.NotFound;
            message = ex.Message;
            _logger.LogWarning(ex, "Resource not found: {Message}", ex.Message);
        }
        catch (ConflictException ex)
        {
            statusCode = HttpStatusCode.Conflict;
            message = ex.Message;
            _logger.LogWarning(ex, "Conflict: {Message}", ex.Message);
        }
        catch (BadRequestException ex)
        {
            statusCode = HttpStatusCode.BadRequest;
            message = ex.Message;
            _logger.LogWarning(ex, "Bad request: {Message}", ex.Message);
        }
        catch (ArgumentException ex)
        {
            statusCode = HttpStatusCode.BadRequest;
            message = ex.Message;
            _logger.LogWarning(ex, "Invalid argument: {Message}", ex.Message);
        }
        catch (Exception ex)
        {
            statusCode = HttpStatusCode.InternalServerError;
            message = "An unexpected error occurred.";
            _logger.LogError(ex, "An unexpected error occurred");
        }

        await HandleExceptionAsync(context, message, statusCode);
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        string message,
        HttpStatusCode statusCode)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            Message = message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}

public static class ErrorHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseErrorHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ErrorHandlingMiddleware>();
    }
}