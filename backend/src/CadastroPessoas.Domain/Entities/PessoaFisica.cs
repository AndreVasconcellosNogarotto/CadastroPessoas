using CadastroPessoas.Domain.Exceptions;

namespace CadastroPessoas.Domain.Entities;

public class PessoaFisica : Pessoa
{
    public string Cpf { get; private set; } = string.Empty;
    public DateTime DataNascimento { get; private set; }
    public string? RG { get; private set; }

    private PessoaFisica() { }

    public PessoaFisica(string nome, string email, string telefone, string cpf, DateTime dataNascimento, string? rg = null)
        : base(nome, email, telefone)
    {
        SetCpf(cpf);
        SetDataNascimento(dataNascimento);
        RG = rg;
    }

    public void Atualizar(string nome, string email, string telefone, string cpf, DateTime dataNascimento, string? rg)
    {
        SetNome(nome);
        SetEmail(email);
        SetTelefone(telefone);
        SetCpf(cpf);
        SetDataNascimento(dataNascimento);
        RG = rg;
        MarcarComoAtualizado();
    }

    private void SetCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new DomainException("CPF não pode ser vazio.");

        var cpfLimpo = cpf.Replace(".", "").Replace("-", "").Trim();

        if (!ValidarCpf(cpfLimpo))
            throw new DomainException("CPF inválido.");

        Cpf = cpfLimpo;
    }

    private void SetDataNascimento(DateTime dataNascimento)
    {
        if (dataNascimento > DateTime.UtcNow)
            throw new DomainException("Data de nascimento não pode ser no futuro.");

        if (dataNascimento < DateTime.UtcNow.AddYears(-150))
            throw new DomainException("Data de nascimento inválida.");

        DataNascimento = dataNascimento;
    }

    public int Idade => (int)((DateTime.UtcNow - DataNascimento).TotalDays / 365.25);

    private static bool ValidarCpf(string cpf)
    {
        if (cpf.Length != 11) return false;
        if (cpf.Distinct().Count() == 1) return false;

        var soma = 0;
        for (var i = 0; i < 9; i++)
            soma += int.Parse(cpf[i].ToString()) * (10 - i);

        var resto = soma % 11;
        var digito1 = resto < 2 ? 0 : 11 - resto;

        if (int.Parse(cpf[9].ToString()) != digito1) return false;

        soma = 0;
        for (var i = 0; i < 10; i++)
            soma += int.Parse(cpf[i].ToString()) * (11 - i);

        resto = soma % 11;
        var digito2 = resto < 2 ? 0 : 11 - resto;

        return int.Parse(cpf[10].ToString()) == digito2;
    }
}
