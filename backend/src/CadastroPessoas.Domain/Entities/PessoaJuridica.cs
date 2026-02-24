using CadastroPessoas.Domain.Exceptions;

namespace CadastroPessoas.Domain.Entities;

public class PessoaJuridica : Pessoa
{
    public string Cnpj { get; private set; } = string.Empty;
    public string RazaoSocial { get; private set; } = string.Empty;
    public string NomeFantasia { get; private set; } = string.Empty;
    public string InscricaoEstadual { get; private set; } = string.Empty;
    public DateTime DataAbertura { get; private set; }

    private PessoaJuridica() { }

    public PessoaJuridica(string nome, string email, string telefone,
        string cnpj, string razaoSocial, string nomeFantasia,
        string inscricaoEstadual, DateTime dataAbertura)
        : base(nome, email, telefone)
    {
        SetCnpj(cnpj);
        SetRazaoSocial(razaoSocial);
        SetNomeFantasia(nomeFantasia);
        InscricaoEstadual = inscricaoEstadual;
        SetDataAbertura(dataAbertura);
    }

    public void Atualizar(string nome, string email, string telefone,
        string cnpj, string razaoSocial, string nomeFantasia,
        string inscricaoEstadual, DateTime dataAbertura)
    {
        SetNome(nome);
        SetEmail(email);
        SetTelefone(telefone);
        SetCnpj(cnpj);
        SetRazaoSocial(razaoSocial);
        SetNomeFantasia(nomeFantasia);
        InscricaoEstadual = inscricaoEstadual;
        SetDataAbertura(dataAbertura);
        MarcarComoAtualizado();
    }

    private void SetRazaoSocial(string razaoSocial)
    {
        if (string.IsNullOrWhiteSpace(razaoSocial))
            throw new DomainException("Razão Social não pode ser vazia.");
        RazaoSocial = razaoSocial.Trim();
    }

    private void SetNomeFantasia(string nomeFantasia)
    {
        if (string.IsNullOrWhiteSpace(nomeFantasia))
            throw new DomainException("Nome Fantasia não pode ser vazio.");
        NomeFantasia = nomeFantasia.Trim();
    }

    private void SetDataAbertura(DateTime dataAbertura)
    {
        if (dataAbertura > DateTime.UtcNow)
            throw new DomainException("Data de abertura não pode ser no futuro.");
        DataAbertura = dataAbertura;
    }

    private void SetCnpj(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            throw new DomainException("CNPJ não pode ser vazio.");

        var cnpjLimpo = cnpj.Replace(".", "").Replace("/", "").Replace("-", "").Trim();

        if (!ValidarCnpj(cnpjLimpo))
            throw new DomainException("CNPJ inválido.");

        Cnpj = cnpjLimpo;
    }

    private static bool ValidarCnpj(string cnpj)
    {
        if (cnpj.Length != 14) return false;
        if (cnpj.Distinct().Count() == 1) return false;

        int[] multiplicadores1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicadores2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var soma = 0;
        for (var i = 0; i < 12; i++)
            soma += int.Parse(cnpj[i].ToString()) * multiplicadores1[i];

        var resto = soma % 11;
        var digito1 = resto < 2 ? 0 : 11 - resto;

        if (int.Parse(cnpj[12].ToString()) != digito1) return false;

        soma = 0;
        for (var i = 0; i < 13; i++)
            soma += int.Parse(cnpj[i].ToString()) * multiplicadores2[i];

        resto = soma % 11;
        var digito2 = resto < 2 ? 0 : 11 - resto;

        return int.Parse(cnpj[13].ToString()) == digito2;
    }
}
