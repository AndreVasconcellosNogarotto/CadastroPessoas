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

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Erro não tratado.");

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
