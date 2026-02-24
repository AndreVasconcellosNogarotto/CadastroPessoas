namespace CadastroPessoas.Domain.Entities;

public abstract class Pessoa
{
    public Guid Id { get; protected set; }
    public string Nome { get; protected set; } = string.Empty;
    public string Email { get; protected set; } = string.Empty;
    public string Telefone { get; protected set; } = string.Empty;
    public bool Ativo { get; protected set; }
    public DateTime CriadoEm { get; protected set; }
    public DateTime? AtualizadoEm { get; protected set; }

    public Endereco? Endereco { get; protected set; }

    protected Pessoa() { }

    protected Pessoa(string nome, string email, string telefone)
    {
        Id = Guid.NewGuid();
        SetNome(nome);
        SetEmail(email);
        SetTelefone(telefone);
        Ativo = true;
        CriadoEm = DateTime.UtcNow;
    }

    public void SetNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new Domain.Exceptions.DomainException("Nome não pode ser vazio.");
        Nome = nome.Trim();
    }

    public void SetEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new Domain.Exceptions.DomainException("E-mail não pode ser vazio.");
        Email = email.Trim().ToLower();
    }

    public void SetTelefone(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            throw new Domain.Exceptions.DomainException("Telefone não pode ser vazio.");
        Telefone = telefone.Trim();
    }

    public void AssociarEndereco(Endereco endereco)
    {
        Endereco = endereco ?? throw new Domain.Exceptions.DomainException("Endereço não pode ser nulo.");
    }

    public void Desativar()
    {
        Ativo = false;
        AtualizadoEm = DateTime.UtcNow;
    }

    public void MarcarComoAtualizado()
    {
        AtualizadoEm = DateTime.UtcNow;
    }
}
