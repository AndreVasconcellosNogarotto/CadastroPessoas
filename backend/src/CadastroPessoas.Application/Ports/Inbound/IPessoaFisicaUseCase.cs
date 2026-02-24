using CadastroPessoas.Application.DTOs.Request;
using CadastroPessoas.Application.DTOs.Response;

namespace CadastroPessoas.Domain.Ports.Inbound;

public interface IPessoaFisicaUseCase
{
    Task<PessoaFisicaResponse> CriarAsync(CreatePessoaFisicaRequest request, CancellationToken cancellationToken = default);
    Task<PessoaFisicaResponse> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResponse<PessoaFisicaResponse>> ListarAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<PessoaFisicaResponse> AtualizarAsync(Guid id, UpdatePessoaFisicaRequest request, CancellationToken cancellationToken = default);
    Task DeletarAsync(Guid id, CancellationToken cancellationToken = default);
}
