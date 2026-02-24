using CadastroPessoas.Domain.Exceptions;

namespace CadastroPessoas.Domain.Entities;

public class Endereco
{
    public Guid Id { get; private set; }
    public string Cep { get; private set; } = string.Empty;
    public string Logradouro { get; private set; } = string.Empty;
    public string Numero { get; private set; } = string.Empty;
    public string? Complemento { get; private set; }
    public string Bairro { get; private set; } = string.Empty;
    public string Cidade { get; private set; } = string.Empty;
    public string Uf { get; private set; } = string.Empty;
    public string? Ibge { get; private set; }
    public DateTime CriadoEm { get; private set; }
    public DateTime? AtualizadoEm { get; private set; }

    private Endereco() { }

    public Endereco(string cep, string logradouro, string numero,
        string bairro, string cidade, string uf,
        string? complemento = null, string? ibge = null)
    {
        Id = Guid.NewGuid();
        SetCep(cep);
        SetLogradouro(logradouro);
        SetNumero(numero);
        SetBairro(bairro);
        SetCidade(cidade);
        SetUf(uf);
        Complemento = complemento;
        Ibge = ibge;
        CriadoEm = DateTime.UtcNow;
    }

    public void Atualizar(string cep, string logradouro, string numero,
        string bairro, string cidade, string uf,
        string? complemento = null, string? ibge = null)
    {
        SetCep(cep);
        SetLogradouro(logradouro);
        SetNumero(numero);
        SetBairro(bairro);
        SetCidade(cidade);
        SetUf(uf);
        Complemento = complemento;
        Ibge = ibge;
        AtualizadoEm = DateTime.UtcNow;
    }

    private void SetCep(string cep)
    {
        if (string.IsNullOrWhiteSpace(cep))
            throw new DomainException("CEP não pode ser vazio.");

        var cepLimpo = cep.Replace("-", "").Trim();
        if (cepLimpo.Length != 8 || !cepLimpo.All(char.IsDigit))
            throw new DomainException("CEP inválido. Informe 8 dígitos numéricos.");

        Cep = cepLimpo;
    }

    private void SetLogradouro(string logradouro)
    {
        if (string.IsNullOrWhiteSpace(logradouro))
            throw new DomainException("Logradouro não pode ser vazio.");
        Logradouro = logradouro.Trim();
    }

    private void SetNumero(string numero)
    {
        if (string.IsNullOrWhiteSpace(numero))
            throw new DomainException("Número não pode ser vazio.");
        Numero = numero.Trim();
    }

    private void SetBairro(string bairro)
    {
        if (string.IsNullOrWhiteSpace(bairro))
            throw new DomainException("Bairro não pode ser vazio.");
        Bairro = bairro.Trim();
    }

    private void SetCidade(string cidade)
    {
        if (string.IsNullOrWhiteSpace(cidade))
            throw new DomainException("Cidade não pode ser vazia.");
        Cidade = cidade.Trim();
    }

    private void SetUf(string uf)
    {
        if (string.IsNullOrWhiteSpace(uf) || uf.Length != 2)
            throw new DomainException("UF deve ter exatamente 2 caracteres.");
        Uf = uf.ToUpper().Trim();
    }
}
