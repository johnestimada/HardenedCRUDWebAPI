using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ProductsApi.Exceptions.Handling;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail, type) = exception switch
        {
            NotFoundException ex => (
                StatusCodes.Status404NotFound,
                "Resource Not Found",
                ex.Message,
                "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.5"),

            ConflictException ex => (
                StatusCodes.Status409Conflict,
                "Conflict",
                ex.Message,
                "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.10"),

            _ => (
                StatusCodes.Status500InternalServerError,
                "Internal Server Error",
                "An unexpected error occurred.",
                "https://datatracker.ietf.org/doc/html/rfc9110#section-15.6.1")
        };

        // The full exception (with stack trace) goes to the server log only.
        // Expected problems (404, 409) are warnings; real failures are errors.
        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Unhandled exception on {Method} {Path}",
                httpContext.Request.Method,
                httpContext.Request.Path);
        }
        else
        {
            _logger.LogWarning(
                exception,
                "{ExceptionType} on {Method} {Path} -> {StatusCode}",
                exception.GetType().Name,
                httpContext.Request.Method,
                httpContext.Request.Path,
                statusCode);
        }

        // The client only ever receives this safe body.
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = type,
            Instance = httpContext.Request.Path
        };

        problem.Extensions["traceId"] = httpContext.TraceIdentifier;

        httpContext.Response.StatusCode = statusCode;

        // WriteAsJsonAsync would overwrite a ContentType set beforehand,
        // so the content type is passed to it directly.
        await httpContext.Response.WriteAsJsonAsync(
            problem,
            options: null,
            contentType: "application/problem+json",
            cancellationToken);

        return true;
    }
}
