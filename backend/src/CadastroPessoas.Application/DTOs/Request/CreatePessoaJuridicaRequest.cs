namespace CadastroPessoas.Application.DTOs.Request;

public record CreatePessoaJuridicaRequest(
    string Nome,
    string Email,
    string Telefone,
    string Cnpj,
    string RazaoSocial,
    string NomeFantasia,
    string InscricaoEstadual,
    DateTime DataAbertura,
    EnderecoRequest? Endereco
);

public record UpdatePessoaJuridicaRequest(
    string Nome,
    string Email,
    string Telefone,
    string Cnpj,
    string RazaoSocial,
    string NomeFantasia,
    string InscricaoEstadual,
    DateTime DataAbertura,
    EnderecoRequest? Endereco
);
