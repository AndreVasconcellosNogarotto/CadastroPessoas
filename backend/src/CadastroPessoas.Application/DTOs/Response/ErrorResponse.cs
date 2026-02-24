namespace CadastroPessoas.Application.DTOs.Response;

public record ErrorResponse(
    string Type,
    string Message,
    IEnumerable<string>? Errors = null
);
