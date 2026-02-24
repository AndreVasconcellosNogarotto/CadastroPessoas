using CadastroPessoas.Application.DTOs.Response;
using CadastroPessoas.Domain.Ports.Inbound;
using CadastroPessoas.Domain.Ports.Outbound;

namespace CadastroPessoas.Application.UseCases.Endereco;

public class EnderecoUseCase : IEnderecoUseCase
{
    private readonly IViaCepService _viaCepService;

    public EnderecoUseCase(IViaCepService viaCepService)
    {
        _viaCepService = viaCepService;
    }

    public async Task<ViaCepResponse?> ConsultarCepAsync(string cep, CancellationToken cancellationToken = default)
    {
        var cepLimpo = cep.Replace("-", "").Trim();

        if (cepLimpo.Length != 8 || !cepLimpo.All(char.IsDigit))
            throw new Domain.Exceptions.DomainException("CEP inválido. Informe 8 dígitos numéricos.");

        var resultado = await _viaCepService.ConsultarCepAsync(cepLimpo, cancellationToken);

        if (resultado is null) return null;

        return new ViaCepResponse(
            resultado.Cep,
            resultado.Logradouro,
            resultado.Complemento,
            resultado.Bairro,
            resultado.Localidade,
            resultado.Uf,
            resultado.Ibge,
            resultado.Ddd
        );
    }
}
