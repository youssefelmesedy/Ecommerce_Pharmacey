using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Application.Exceptions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Pharmacy.Api.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var statusCode = ex switch
        {
            FluentValidation.ValidationException => StatusCodes.Status400BadRequest,
            ValidateException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            BusinessException => StatusCodes.Status400BadRequest,
            JsonException => StatusCodes.Status400BadRequest,
            DbUpdateException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status403Forbidden,
            ArgumentNullException => StatusCodes.Status400BadRequest,
            ArgumentException => StatusCodes.Status400BadRequest,
            TaskCanceledException => StatusCodes.Status408RequestTimeout,
            TimeoutException => StatusCodes.Status408RequestTimeout,
            AutoMapperMappingException => StatusCodes.Status500InternalServerError,
            InvalidOperationException => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

        // ✅ تأكد إنك تكتب بس لو الـ response لسه مفتوح
        if (!context.Response.HasStarted)
        {
            context.Response.Clear();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
        }

        var problem = new ExceptionProblemDetails
        {
            Status = statusCode,
            Title = ex switch
            {
                FluentValidation.ValidationException => "Validation Failed",
                ValidateException => "Custome Validation Errors",
                NotFoundException => "Not Found Exception",
                BusinessException => "Business Exception",
                JsonException => "Invalid Json Type",
                _ => $"Invalid type Name: {ex.GetType().Name}"
            },
            Detail = _env.IsDevelopment() ? ex.Message : "An unexpected error occurred.",
            Instance = context.Request.Path,
            Type = $"https://httpstatuses.com/{statusCode}",
            ErrorCode = ex.GetType().Name,
            ErrorSource = ex.Source,
            Success = false,
            Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss 'UTC'")

        };

        // ✅ خاص بـ FluentValidation
        if (ex is FluentValidation.ValidationException validationException)
        {
            problem.Errors = validationException.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
        }
        else if(ex is ValidateException customValidator)
        {
            problem.Title = "Validation Error";
            problem.Detail = "One or more validation errors occurred.";
            problem.Errors = customValidator.Errors?.ToDictionary(
        e => e.Key,
                  e => e.Value.ToArray());
        }
        var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        });

        await context.Response.WriteAsync(json);
    }
}