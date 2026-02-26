using CadastroPessoas.Application.Ports.Outbound;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Infrastructure.Persistence.EF.Context;

namespace CadastroPessoas.Infrastructure.Persistence.EF.Repositories;

public class AuditRepository : IAuditRepository
{
    private readonly AppDbContext _context;

    public AuditRepository(AppDbContext context) => _context = context;

    public async Task RegistrarAsync(AuditLog audit, CancellationToken cancellationToken = default)
    {
        await _context.AuditLogs.AddAsync(audit, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
