using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Ports.Outbound;
using CadastroPessoas.Infrastructure.Persistence.EF.Context;
using Microsoft.EntityFrameworkCore;

namespace CadastroPessoas.Infrastructure.Persistence.EF.Repositories;

public class PessoaJuridicaRepository : IPessoaJuridicaRepository
{
    private readonly AppDbContext _context;

    public PessoaJuridicaRepository(AppDbContext context) => _context = context;

    public async Task<PessoaJuridica?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.PessoasJuridicas
            .Include(p => p.Endereco)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<PessoaJuridica?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default) =>
        await _context.PessoasJuridicas
            .Include(p => p.Endereco)
            .FirstOrDefaultAsync(p => p.Cnpj == cnpj, cancellationToken);

    public async Task<IEnumerable<PessoaJuridica>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default) =>
        await _context.PessoasJuridicas
            .Include(p => p.Endereco)
            .OrderByDescending(p => p.CriadoEm)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

    public async Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        await _context.PessoasJuridicas.CountAsync(cancellationToken);

    public async Task AddAsync(PessoaJuridica pessoaJuridica, CancellationToken cancellationToken = default)
    {
        await _context.PessoasJuridicas.AddAsync(pessoaJuridica, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(PessoaJuridica pessoaJuridica, CancellationToken cancellationToken = default)
    {
        _context.PessoasJuridicas.Update(pessoaJuridica);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(PessoaJuridica pessoaJuridica, CancellationToken cancellationToken = default)
    {
        _context.PessoasJuridicas.Remove(pessoaJuridica);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByCnpjAsync(string cnpj, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var cnpjLimpo = cnpj.Replace(".", "").Replace("/", "").Replace("-", "").Trim();
        var query = _context.PessoasJuridicas.Where(p => p.Cnpj == cnpjLimpo);

        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);

        return await query.AnyAsync(cancellationToken);
    }
}

