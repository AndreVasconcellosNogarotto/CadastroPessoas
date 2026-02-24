using CadastroPessoas.Application.DTOs.Request;
using CadastroPessoas.Domain.Ports.Inbound;
using Microsoft.AspNetCore.Mvc;

namespace CadastroPessoas.API.Controllers;

[ApiController]
[Route("api/v1/pessoas-juridicas")]
[Produces("application/json")]
public class PessoaJuridicaController : ControllerBase
{
    private readonly IPessoaJuridicaUseCase _useCase;

    public PessoaJuridicaController(IPessoaJuridicaUseCase useCase)
    {
        _useCase = useCase;
    }

    /// <summary>Lista todas as pessoas jurídicas com paginação.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var resultado = await _useCase.ListarAsync(page, pageSize, cancellationToken);
        return Ok(resultado);
    }

    /// <summary>Obtém uma pessoa jurídica pelo ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObterPorId(Guid id, CancellationToken cancellationToken = default)
    {
        var resultado = await _useCase.ObterPorIdAsync(id, cancellationToken);
        return Ok(resultado);
    }

    /// <summary>Cria uma nova pessoa jurídica.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Criar([FromBody] CreatePessoaJuridicaRequest request, CancellationToken cancellationToken = default)
    {
        var resultado = await _useCase.CriarAsync(request, cancellationToken);
        return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Id }, resultado);
    }

    /// <summary>Atualiza uma pessoa jurídica existente.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] UpdatePessoaJuridicaRequest request, CancellationToken cancellationToken = default)
    {
        var resultado = await _useCase.AtualizarAsync(id, request, cancellationToken);
        return Ok(resultado);
    }

    /// <summary>Remove uma pessoa jurídica.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deletar(Guid id, CancellationToken cancellationToken = default)
    {
        await _useCase.DeletarAsync(id, cancellationToken);
        return NoContent();
    }
}
