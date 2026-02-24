using CadastroPessoas.Application.UseCases.Endereco;
using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.Ports.Outbound;
using FluentAssertions;
using Moq;
using Xunit;

namespace CadastroPessoas.UnitTests.Application.Endereco;

public class EnderecoUseCaseTests
{
    private readonly Mock<IViaCepService> _viaCepServiceMock = new();
    private readonly EnderecoUseCase _useCase;

    public EnderecoUseCaseTests()
    {
        _useCase = new EnderecoUseCase(_viaCepServiceMock.Object);
    }

    [Fact]
    public async Task ConsultarCepAsync_ComCepValido_DeveRetornarViaCepResponse()
    {
        var viaCepResult = new ViaCepResult(
            "01310-100", "Av. Paulista", "", "Bela Vista", "São Paulo", "SP", "3550308", "11"
        );

        _viaCepServiceMock.Setup(s => s.ConsultarCepAsync("01310100", default))
            .ReturnsAsync(viaCepResult);

        var result = await _useCase.ConsultarCepAsync("01310100");

        result.Should().NotBeNull();
        result!.Cep.Should().Be("01310-100");
        result.Cidade.Should().Be("São Paulo");
        result.Uf.Should().Be("SP");
    }

    [Fact]
    public async Task ConsultarCepAsync_ComHifen_DeveLimparEConsultar()
    {
        _viaCepServiceMock.Setup(s => s.ConsultarCepAsync("01310100", default))
            .ReturnsAsync(new ViaCepResult("01310-100", "Av. Paulista", "", "Bela Vista", "São Paulo", "SP", "3550308", "11"));

        var result = await _useCase.ConsultarCepAsync("01310-100");

        result.Should().NotBeNull();
        _viaCepServiceMock.Verify(s => s.ConsultarCepAsync("01310100", default), Times.Once);
    }

    [Theory]
    [InlineData("1234567")]   // 7 dígitos
    [InlineData("123456789")] // 9 dígitos
    [InlineData("ABCDEFGH")] // letras
    [InlineData("")]
    public async Task ConsultarCepAsync_ComCepInvalido_DeveLancarDomainException(string cepInvalido)
    {
        var act = async () => await _useCase.ConsultarCepAsync(cepInvalido);
        await act.Should().ThrowAsync<DomainException>().WithMessage("CEP inválido*");
    }

    [Fact]
    public async Task ConsultarCepAsync_CepNaoEncontrado_DeveRetornarNull()
    {
        _viaCepServiceMock.Setup(s => s.ConsultarCepAsync("99999999", default))
            .ReturnsAsync((ViaCepResult?)null);

        var result = await _useCase.ConsultarCepAsync("99999999");

        result.Should().BeNull();
    }
}
