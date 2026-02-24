using CadastroPessoas.Domain.Ports.Inbound;
using Microsoft.AspNetCore.Mvc;

namespace CadastroPessoas.API.Controllers;

[ApiController]
[Route("api/v1/enderecos")]
[Produces("application/json")]
public class EnderecoController : ControllerBase
{
    private readonly IEnderecoUseCase _useCase;

    public EnderecoController(IEnderecoUseCase useCase)
    {
        _useCase = useCase;
    }

    /// <summary>Consulta um endereço pelo CEP utilizando a API ViaCEP.</summary>
    [HttpGet("cep/{cep}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ConsultarCep(string cep, CancellationToken cancellationToken = default)
    {
        var resultado = await _useCase.ConsultarCepAsync(cep, cancellationToken);

        if (resultado is null)
            return NotFound(new { message = $"CEP '{cep}' não encontrado." });

        return Ok(resultado);
    }
}
