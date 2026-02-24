using CadastroPessoas.Domain.Entities;

namespace CadastroPessoas.Domain.Ports.Outbound;

public interface IEnderecoRepository
{
    Task<Endereco?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Endereco endereco, CancellationToken cancellationToken = default);
    Task UpdateAsync(Endereco endereco, CancellationToken cancellationToken = default);
}
