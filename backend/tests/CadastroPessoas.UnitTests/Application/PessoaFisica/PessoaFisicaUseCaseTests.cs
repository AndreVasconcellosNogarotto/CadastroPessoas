using CadastroPessoas.Application.DTOs.Request;
using CadastroPessoas.Application.UseCases.PessoaFisica;
using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.Ports.Outbound;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using EnderecoEntity = CadastroPessoas.Domain.Entities.Endereco;
using PessoaFisicaEntity = CadastroPessoas.Domain.Entities.PessoaFisica;

namespace CadastroPessoas.UnitTests.Application.PessoaFisica;

public class PessoaFisicaUseCaseTests
{
    private readonly Mock<IPessoaFisicaRepository> _repositoryMock = new();
    private readonly Mock<IEnderecoRepository> _enderecoRepositoryMock = new();
    private readonly Mock<ILogger<PessoaFisicaUseCase>> _loggerMock = new();
    private readonly PessoaFisicaUseCase _useCase;

    private const string CpfValido = "52998224725";

    public PessoaFisicaUseCaseTests()
    {
        _useCase = new PessoaFisicaUseCase(_repositoryMock.Object, _enderecoRepositoryMock.Object, _loggerMock.Object);
    }

    private static CreatePessoaFisicaRequest CriarRequest(string cpf = CpfValido) =>
        new("João Silva", "joao@email.com", "11999887766", cpf, new DateTime(1990, 5, 15), null, null);

    [Fact]
    public async Task CriarAsync_ComDadosValidos_DeveRetornarResponse()
    {
        // Arrange
        _repositoryMock.Setup(r => r.ExistsByCpfAsync(It.IsAny<string>(), null, default))
            .ReturnsAsync(false);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<PessoaFisicaEntity>(), default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _useCase.CriarAsync(CriarRequest());

        // Assert
        result.Should().NotBeNull();
        result.Nome.Should().Be("João Silva");
        result.Cpf.Should().Be(CpfValido);
        result.Ativo.Should().BeTrue();

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<PessoaFisicaEntity>(), default), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_ComCpfDuplicado_DeveLancarConflictException()
    {
        _repositoryMock.Setup(r => r.ExistsByCpfAsync(It.IsAny<string>(), null, default))
            .ReturnsAsync(true);

        var act = async () => await _useCase.CriarAsync(CriarRequest());

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage($"*{CpfValido}*");
    }

    [Fact]
    public async Task CriarAsync_ComEndereco_DeveSalvarEnderecoEAssociar()
    {
        var request = CriarRequest() with
        {
            Endereco = new EnderecoRequest("01310100", "Av. Paulista", "1000", null, "Bela Vista", "São Paulo", "SP")
        };

        _repositoryMock.Setup(r => r.ExistsByCpfAsync(It.IsAny<string>(), null, default))
            .ReturnsAsync(false);
        _enderecoRepositoryMock.Setup(r => r.AddAsync(It.IsAny<EnderecoEntity>(), default))
            .Returns(Task.CompletedTask);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<PessoaFisicaEntity>(), default))
            .Returns(Task.CompletedTask);

        var result = await _useCase.CriarAsync(request);

        result.Endereco.Should().NotBeNull();
        result.Endereco!.Logradouro.Should().Be("Av. Paulista");

        _enderecoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<EnderecoEntity>(), default), Times.Once);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdInexistente_DeveLancarNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((PessoaFisicaEntity?)null);

        var act = async () => await _useCase.ObterPorIdAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdValido_DeveRetornarResponse()
    {
        var pf = new PessoaFisicaEntity("João", "joao@email.com", "11999887766", CpfValido, new DateTime(1990, 5, 15));

        _repositoryMock.Setup(r => r.GetByIdAsync(pf.Id, default))
            .ReturnsAsync(pf);

        var result = await _useCase.ObterPorIdAsync(pf.Id);

        result.Should().NotBeNull();
        result.Id.Should().Be(pf.Id);
    }

    [Fact]
    public async Task DeletarAsync_ComIdInexistente_DeveLancarNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((PessoaFisicaEntity?)null);

        var act = async () => await _useCase.DeletarAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task DeletarAsync_ComIdValido_DeveChamarDeleteNoRepositorio()
    {
        var pf = new PessoaFisicaEntity("João", "joao@email.com", "11999887766", CpfValido, new DateTime(1990, 5, 15));

        _repositoryMock.Setup(r => r.GetByIdAsync(pf.Id, default)).ReturnsAsync(pf);
        _repositoryMock.Setup(r => r.DeleteAsync(It.IsAny<PessoaFisicaEntity>(), default)).Returns(Task.CompletedTask);

        await _useCase.DeletarAsync(pf.Id);

        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<PessoaFisicaEntity>(), default), Times.Once);
    }

    [Fact]
    public async Task ListarAsync_DeveRetornarPagedResponse()
    {
        var pfs = new List<PessoaFisicaEntity>
        {
            new("João", "joao@email.com", "11999887766", CpfValido, new DateTime(1990, 5, 15))
        };

        _repositoryMock.Setup(r => r.CountAsync(default)).ReturnsAsync(1);
        _repositoryMock.Setup(r => r.GetAllAsync(1, 10, default)).ReturnsAsync(pfs);

        var result = await _useCase.ListarAsync(1, 10);

        result.Total.Should().Be(1);
        result.Page.Should().Be(1);
        result.Data.Should().HaveCount(1);
    }
}
