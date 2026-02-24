using CadastroPessoas.Application.DTOs.Request;
using CadastroPessoas.Application.DTOs.Response;

namespace CadastroPessoas.Domain.Ports.Inbound;

public interface IPessoaJuridicaUseCase
{
    Task<PessoaJuridicaResponse> CriarAsync(CreatePessoaJuridicaRequest request, CancellationToken cancellationToken = default);
    Task<PessoaJuridicaResponse> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponse<PessoaJuridicaResponse>> ListarAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PessoaJuridicaResponse> AtualizarAsync(Guid id, UpdatePessoaJuridicaRequest request, CancellationToken cancellationToken = default);
    Task DeletarAsync(Guid id, CancellationToken cancellationToken = default);
}
