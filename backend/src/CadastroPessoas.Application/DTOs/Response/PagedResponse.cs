namespace CadastroPessoas.Application.DTOs.Response;


public record PagedResponse<T>(
    IEnumerable<T> Data,
    int Page,
    int PageSize,
    int Total,
    int TotalPages
);
