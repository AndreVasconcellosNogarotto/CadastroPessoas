using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Ports.Outbound;
using CadastroPessoas.Infrastructure.Persistence.EF.Context;

namespace CadastroPessoas.Infrastructure.Persistence.EF.Repositories;


public class EnderecoRepository : IEnderecoRepository
{
    private readonly AppDbContext _context;

    public EnderecoRepository(AppDbContext context) => _context = context;

    public async Task<Endereco?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Enderecos.FindAsync(new object[] { id }, cancellationToken);

    public async Task AddAsync(Endereco endereco, CancellationToken cancellationToken = default)
    {
        await _context.Enderecos.AddAsync(endereco, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Endereco endereco, CancellationToken cancellationToken = default)
    {
        _context.Enderecos.Update(endereco);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

