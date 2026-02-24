using CadastroPessoas.Domain.Entities;

namespace CadastroPessoas.Domain.Ports.Outbound;


public interface IPessoaFisicaRepository
{
    Task<PessoaFisica?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PessoaFisica?> GetByCpfAsync(string cpf, CancellationToken cancellationToken = default);
    Task<IEnumerable<PessoaFisica>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task AddAsync(PessoaFisica pessoaFisica, CancellationToken cancellationToken = default);
    Task UpdateAsync(PessoaFisica pessoaFisica, CancellationToken cancellationToken = default);
    Task DeleteAsync(PessoaFisica pessoaFisica, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCpfAsync(string cpf, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
