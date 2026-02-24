namespace CadastroPessoas.Application.DTOs.Request;

public record EnderecoRequest(
    string Cep,
    string Logradouro,
    string Numero,
    string? Complemento,
    string Bairro,
    string Cidade,
    string Uf
);