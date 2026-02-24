namespace CadastroPessoas.Application.DTOs.Response;

public record PessoaJuridicaResponse(
    Guid Id,
    string Nome,
    string Email,
    string Telefone,
    string Cnpj,
    string RazaoSocial,
    string NomeFantasia,
    string InscricaoEstadual,
    DateTime DataAbertura,
    bool Ativo,
    DateTime CriadoEm,
    DateTime? AtualizadoEm,
    EnderecoResponse? Endereco
);
