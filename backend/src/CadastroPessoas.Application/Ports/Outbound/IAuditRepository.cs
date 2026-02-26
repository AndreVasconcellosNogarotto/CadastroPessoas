using CadastroPessoas.Domain.Entities;

namespace CadastroPessoas.Application.Ports.Outbound;

public interface IAuditRepository
{
    Task RegistrarAsync(AuditLog audit, CancellationToken cancellationToken = default);
}
