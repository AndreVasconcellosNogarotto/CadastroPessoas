using CadastroPessoas.Application.DTOs.Response;

namespace CadastroPessoas.Domain.Ports.Inbound
{
    public interface IEnderecoUseCase
    {
        Task<ViaCepResponse?> ConsultarCepAsync(string cep, CancellationToken cancellationToken = default);
    }
}
