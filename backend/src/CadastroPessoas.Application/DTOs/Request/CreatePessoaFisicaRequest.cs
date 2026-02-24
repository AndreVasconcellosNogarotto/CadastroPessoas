namespace CadastroPessoas.Application.DTOs.Request;

public record CreatePessoaFisicaRequest(
    string Nome,
    string Email,
    string Telefone,
    string Cpf,
    DateTime DataNascimento,
    string? RG,
    EnderecoRequest? Endereco
);

public record UpdatePessoaFisicaRequest(
    string Nome,
    string Email,
    string Telefone,
    string Cpf,
    DateTime DataNascimento,
    string? RG,
    EnderecoRequest? Endereco
);
