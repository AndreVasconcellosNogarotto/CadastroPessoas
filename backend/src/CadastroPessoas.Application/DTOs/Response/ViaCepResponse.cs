namespace CadastroPessoas.Application.DTOs.Response;

public record ViaCepResponse(
    string Cep,
    string Logradouro,
    string Complemento,
    string Bairro,
    string Cidade,
    string Uf,
    string Ibge,
    string Ddd
);
