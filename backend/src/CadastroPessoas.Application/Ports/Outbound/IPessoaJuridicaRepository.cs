using CadastroPessoas.Domain.Entities;

namespace CadastroPessoas.Domain.Ports.Outbound;

public interface IPessoaJuridicaRepository
{
    Task<PessoaJuridica?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PessoaJuridica?> GetByCnpjAsync(string cnpj, CancellationToken cancellationToken = default);
    Task<IEnumerable<PessoaJuridica>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task AddAsync(PessoaJuridica pessoaJuridica, CancellationToken cancellationToken = default);
    Task UpdateAsync(PessoaJuridica pessoaJuridica, CancellationToken cancellationToken = default);
    Task DeleteAsync(PessoaJuridica pessoaJuridica, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCnpjAsync(string cnpj, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
