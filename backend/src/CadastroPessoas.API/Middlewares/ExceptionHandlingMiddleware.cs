using System.Net;
using System.Text.Json;
using CadastroPessoas.Application.DTOs.Response;
using CadastroPessoas.Domain.Exceptions;

namespace CadastroPessoas.API.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, type, message) = exception switch
        {
            DomainException => (HttpStatusCode.BadRequest, "DomainError", exception.Message),
            NotFoundException => (HttpStatusCode.NotFound, "NotFound", exception.Message),
            ConflictException => (HttpStatusCode.Conflict, "Conflict", exception.Message),
            _ => (HttpStatusCode.InternalServerError, "InternalServerError", "Ocorreu um erro interno no servidor.")
        };

        var requestPath = context.Request.Path;
        var requestMethod = context.Request.Method;
        var requestId = context.TraceIdentifier;

        switch (statusCode)
        {
            case HttpStatusCode.BadRequest:
                _logger.LogWarning(
                    "[{RequestId}] DomainError em {Method} {Path} → {Message}",
                    requestId, requestMethod, requestPath, exception.Message);
                break;

            case HttpStatusCode.NotFound:
                _logger.LogWarning(
                    "[{RequestId}] NotFound em {Method} {Path} → {Message}",
                    requestId, requestMethod, requestPath, exception.Message);
                break;

            case HttpStatusCode.Conflict:
                _logger.LogWarning(
                    "[{RequestId}] Conflict em {Method} {Path} → {Message}",
                    requestId, requestMethod, requestPath, exception.Message);
                break;

            default:
                _logger.LogError(
                    exception,
                    "[{RequestId}] Erro não tratado em {Method} {Path} → {ExceptionType}: {Message}",
                    requestId, requestMethod, requestPath,
                    exception.GetType().Name, exception.Message);
                break;
        }

        var response = new ErrorResponse(type, message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}