namespace CadastroPessoas.Domain.Ports.Outbound;

public record ViaCepResult(
    string Cep,
    string Logradouro,
    string Complemento,
    string Bairro,
    string Localidade,
    string Uf,
    string Ibge,
    string Ddd
);

// Port de saída para serviço externo ViaCEP (Secondary Port)
public interface IViaCepService
{
    Task<ViaCepResult?> ConsultarCepAsync(string cep, CancellationToken cancellationToken = default);
}
