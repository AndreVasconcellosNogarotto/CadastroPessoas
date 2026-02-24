using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace CadastroPessoas.UnitTests.Domain;

public class EnderecoTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarComSucesso()
    {
        var endereco = new Endereco("01310100", "Av. Paulista", "1000", "Bela Vista", "São Paulo", "SP", "Apto 42");

        endereco.Id.Should().NotBeEmpty();
        endereco.Cep.Should().Be("01310100");
        endereco.Logradouro.Should().Be("Av. Paulista");
        endereco.Numero.Should().Be("1000");
        endereco.Bairro.Should().Be("Bela Vista");
        endereco.Cidade.Should().Be("São Paulo");
        endereco.Uf.Should().Be("SP");
        endereco.Complemento.Should().Be("Apto 42");
    }

    [Theory]
    [InlineData("0131010")]   // 7 dígitos
    [InlineData("013101000")] // 9 dígitos
    [InlineData("ABCDEFGH")] // não numérico
    [InlineData("")]
    [InlineData(null)]
    public void Criar_ComCepInvalido_DeveLancarDomainException(string? cep)
    {
        var act = () => new Endereco(cep!, "Logradouro", "1", "Bairro", "Cidade", "SP");
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Criar_ComCepComHifen_DeveRemoverHifen()
    {
        var endereco = new Endereco("01310-100", "Av. Paulista", "1000", "Bela Vista", "São Paulo", "SP");
        endereco.Cep.Should().Be("01310100");
    }

    [Theory]
    [InlineData("A")]
    [InlineData("ABC")]
    [InlineData("")]
    [InlineData(null)]
    public void Criar_ComUfInvalida_DeveLancarDomainException(string? uf)
    {
        var act = () => new Endereco("01310100", "Logradouro", "1", "Bairro", "Cidade", uf!);
        act.Should().Throw<DomainException>().WithMessage("UF deve ter exatamente 2 caracteres.");
    }

    [Fact]
    public void Criar_UfDeveSerConvertidaParaMaiusculo()
    {
        var endereco = new Endereco("01310100", "Logradouro", "1", "Bairro", "Cidade", "sp");
        endereco.Uf.Should().Be("SP");
    }

    [Fact]
    public void Atualizar_ComDadosValidos_DeveAtualizarEMarcarDataAtualizacao()
    {
        var endereco = new Endereco("01310100", "Av. Paulista", "1000", "Bela Vista", "São Paulo", "SP");

        endereco.Atualizar("20040020", "Av. Atlântica", "500", "Copacabana", "Rio de Janeiro", "RJ");

        endereco.Logradouro.Should().Be("Av. Atlântica");
        endereco.Cidade.Should().Be("Rio de Janeiro");
        endereco.Uf.Should().Be("RJ");
        endereco.AtualizadoEm.Should().NotBeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Criar_ComLogradouroVazio_DeveLancarDomainException(string? logradouro)
    {
        var act = () => new Endereco("01310100", logradouro!, "1", "Bairro", "Cidade", "SP");
        act.Should().Throw<DomainException>().WithMessage("Logradouro não pode ser vazio.");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Criar_ComNumeroVazio_DeveLancarDomainException(string? numero)
    {
        var act = () => new Endereco("01310100", "Logradouro", numero!, "Bairro", "Cidade", "SP");
        act.Should().Throw<DomainException>().WithMessage("Número não pode ser vazio.");
    }
}
