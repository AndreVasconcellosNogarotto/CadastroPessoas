using System.Text.Json;
using System.Text.Json.Serialization;
using CadastroPessoas.Domain.Ports.Outbound;
using Microsoft.Extensions.Logging;

namespace CadastroPessoas.Infrastructure.ExternalServices.ViaCep;

internal record ViaCepApiResponse(
    [property: JsonPropertyName("cep")] string Cep,
    [property: JsonPropertyName("logradouro")] string Logradouro,
    [property: JsonPropertyName("complemento")] string Complemento,
    [property: JsonPropertyName("bairro")] string Bairro,
    [property: JsonPropertyName("localidade")] string Localidade,
    [property: JsonPropertyName("uf")] string Uf,
    [property: JsonPropertyName("ibge")] string Ibge,
    [property: JsonPropertyName("ddd")] string Ddd,
    [property: JsonPropertyName("erro")] bool? Erro
);

// Adaptador de saída (Secondary Adapter) para o serviço externo ViaCEP
public class ViaCepAdapter : IViaCepService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ViaCepAdapter> _logger;

    public ViaCepAdapter(HttpClient httpClient, ILogger<ViaCepAdapter> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ViaCepResult?> ConsultarCepAsync(string cep, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"https://viacep.com.br/ws/{cep}/json/";
            var response = await _httpClient.GetAsync(url, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("ViaCEP retornou status {StatusCode} para o CEP {Cep}", response.StatusCode, cep);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            var apiResponse = JsonSerializer.Deserialize<ViaCepApiResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (apiResponse is null || apiResponse.Erro == true)
            {
                _logger.LogWarning("CEP {Cep} não encontrado no ViaCEP.", cep);
                return null;
            }

            return new ViaCepResult(
                apiResponse.Cep,
                apiResponse.Logradouro,
                apiResponse.Complemento,
                apiResponse.Bairro,
                apiResponse.Localidade,
                apiResponse.Uf,
                apiResponse.Ibge,
                apiResponse.Ddd
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao consultar o CEP {Cep} no ViaCEP.", cep);
            return null;
        }
    }
}
