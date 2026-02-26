namespace CadastroPessoas.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; private set; }
    public Guid EntidadeId { get; private set; }
    public string Entidade { get; private set; }
    public DateTime RealizadoEm { get; private set; }

    private AuditLog() { }  // EF Core

    public AuditLog(Guid entidadeId, string entidade)
    {
        Id = Guid.NewGuid();
        EntidadeId = entidadeId;
        Entidade = entidade;
        RealizadoEm = DateTime.UtcNow;
    }
}
