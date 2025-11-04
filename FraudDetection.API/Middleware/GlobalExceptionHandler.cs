using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FluentValidation;
using System.Text.Json;

namespace FraudDetection.API.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(
            exception,
            "An unhandled exception occurred while processing the request. TraceId: {TraceId}",
            httpContext.TraceIdentifier);

        var (statusCode, title, detail, errors) = exception switch
        {
            ValidationException validationException => HandleValidationException(validationException),
            ArgumentException argumentException => HandleArgumentException(argumentException),
            InvalidOperationException invalidOpException => HandleInvalidOperationException(invalidOpException),
            _ => HandleGenericException(exception)
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path,
            Extensions =
            {
                ["traceId"] = httpContext.TraceIdentifier,
                ["timestamp"] = DateTime.UtcNow
            }
        };

        if (errors?.Any() == true)
        {
            problemDetails.Extensions["errors"] = errors;
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true; // Exception handled
    }

    private (int StatusCode, string Title, string Detail, Dictionary<string, string[]>? Errors) HandleValidationException(
        ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.ErrorMessage).ToArray());

        return (
            StatusCodes.Status400BadRequest,
            "Validation Error",
            "One or more validation errors occurred.",
            errors);
    }

    private (int StatusCode, string Title, string Detail, Dictionary<string, string[]>? Errors) HandleArgumentException(
        ArgumentException exception)
    {
        return (
            StatusCodes.Status400BadRequest,
            "Bad Request",
            exception.Message,
            null);
    }

    private (int StatusCode, string Title, string Detail, Dictionary<string, string[]>? Errors) HandleInvalidOperationException(
        InvalidOperationException exception)
    {
        return (
            StatusCodes.Status409Conflict,
            "Conflict",
            exception.Message,
            null);
    }

    private (int StatusCode, string Title, string Detail, Dictionary<string, string[]>? Errors) HandleGenericException(
        Exception exception)
    {
        return (
            StatusCodes.Status500InternalServerError,
            "Internal Server Error",
            "An unexpected error occurred. Please try again later.",
            null);
    }
}