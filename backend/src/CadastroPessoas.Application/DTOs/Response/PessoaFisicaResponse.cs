namespace CadastroPessoas.Application.DTOs.Response;


public record PessoaFisicaResponse(
    Guid Id,
    string Nome,
    string Email,
    string Telefone,
    string Cpf,
    DateTime DataNascimento,
    int Idade,
    string? RG,
    bool Ativo,
    DateTime CriadoEm,
    DateTime? AtualizadoEm,
    EnderecoResponse? Endereco
);
