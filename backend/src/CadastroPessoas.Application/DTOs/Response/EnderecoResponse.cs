namespace CadastroPessoas.Application.DTOs.Response;

public record EnderecoResponse(
    Guid Id,
    string Cep,
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string Cidade,
    string Uf
);
