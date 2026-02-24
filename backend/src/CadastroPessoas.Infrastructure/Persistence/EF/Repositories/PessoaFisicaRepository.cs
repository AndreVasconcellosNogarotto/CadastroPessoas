using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Ports.Outbound;
using CadastroPessoas.Infrastructure.Persistence.EF.Context;
using Microsoft.EntityFrameworkCore;

namespace CadastroPessoas.Infrastructure.Persistence.EF.Repositories;

public class PessoaFisicaRepository : IPessoaFisicaRepository
{
    private readonly AppDbContext _context;

    public PessoaFisicaRepository(AppDbContext context) => _context = context;

    public async Task<PessoaFisica?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.PessoasFisicas
            .Include(p => p.Endereco)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<PessoaFisica?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default) =>
        await _context.PessoasFisicas
            .Include(p => p.Endereco)
            .FirstOrDefaultAsync(p => p.Cpf == cpf, cancellationToken);

    public async Task<IEnumerable<PessoaFisica>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default) =>
        await _context.PessoasFisicas
            .Include(p => p.Endereco)
            .OrderByDescending(p => p.CriadoEm)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        await _context.PessoasFisicas.CountAsync(cancellationToken);

    public async Task AddAsync(PessoaFisica pessoaFisica, CancellationToken cancellationToken = default)
    {
        await _context.PessoasFisicas.AddAsync(pessoaFisica, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(PessoaFisica pessoaFisica, CancellationToken cancellationToken = default)
    {
        _context.PessoasFisicas.Update(pessoaFisica);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(PessoaFisica pessoaFisica, CancellationToken cancellationToken = default)
    {
        _context.PessoasFisicas.Remove(pessoaFisica);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCpfAsync(string cpf, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var cpfLimpo = cpf.Replace(".", "").Replace("-", "").Trim();
        var query = _context.PessoasFisicas.Where(p => p.Cpf == cpfLimpo);

        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }
}

