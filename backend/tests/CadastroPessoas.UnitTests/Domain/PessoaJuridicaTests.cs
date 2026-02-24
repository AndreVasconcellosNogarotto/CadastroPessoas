using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace CadastroPessoas.UnitTests.Domain;

public class PessoaJuridicaTests
{
    private const string CnpjValido = "11222333000181";
    private const string NomeValido = "Empresa Teste LTDA";
    private const string EmailValido = "empresa@email.com";
    private const string TelefoneValido = "1133221100";
    private const string RazaoSocialValida = "Empresa Teste Ltda";
    private const string NomeFantasiaValido = "Empresa Teste";
    private const string InscricaoEstadualValida = "SP-123456789";
    private readonly DateTime DataAberturaValida = new DateTime(2010, 1, 15);

    private PessoaJuridica CriarPJ() =>
        new PessoaJuridica(NomeValido, EmailValido, TelefoneValido,
            CnpjValido, RazaoSocialValida, NomeFantasiaValido,
            InscricaoEstadualValida, DataAberturaValida);

    [Fact]
    public void Criar_ComDadosValidos_DeveCriarComSucesso()
    {
        var pj = CriarPJ();

        pj.Id.Should().NotBeEmpty();
        pj.Nome.Should().Be(NomeValido);
        pj.Cnpj.Should().Be(CnpjValido);
        pj.RazaoSocial.Should().Be(RazaoSocialValida);
        pj.NomeFantasia.Should().Be(NomeFantasiaValido);
        pj.Ativo.Should().BeTrue();
    }

    [Theory]
    [InlineData("00000000000000")]
    [InlineData("11111111111111")]
    [InlineData("1234567890123")]
    [InlineData("abc")]
    public void Criar_ComCnpjInvalido_DeveLancarDomainException(string cnpjInvalido)
    {
        var act = () => new PessoaJuridica(NomeValido, EmailValido, TelefoneValido,
            cnpjInvalido, RazaoSocialValida, NomeFantasiaValido,
            InscricaoEstadualValida, DataAberturaValida);

        act.Should().Throw<DomainException>().WithMessage("CNPJ inválido.");
    }

    [Fact]
    public void Criar_ComCnpjFormatado_DeveRemoverMascara()
    {
        var pj = new PessoaJuridica(NomeValido, EmailValido, TelefoneValido,
            "11.222.333/0001-81", RazaoSocialValida, NomeFantasiaValido,
            InscricaoEstadualValida, DataAberturaValida);

        pj.Cnpj.Should().Be(CnpjValido);
    }

    [Fact]
    public void Criar_ComDataAberturaNoFuturo_DeveLancarDomainException()
    {
        var act = () => new PessoaJuridica(NomeValido, EmailValido, TelefoneValido,
            CnpjValido, RazaoSocialValida, NomeFantasiaValido,
            InscricaoEstadualValida, DateTime.UtcNow.AddDays(1));

        act.Should().Throw<DomainException>().WithMessage("Data de abertura não pode ser no futuro.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Criar_ComRazaoSocialVazia_DeveLancarDomainException(string? razaoSocial)
    {
        var act = () => new PessoaJuridica(NomeValido, EmailValido, TelefoneValido,
            CnpjValido, razaoSocial!, NomeFantasiaValido,
            InscricaoEstadualValida, DataAberturaValida);

        act.Should().Throw<DomainException>().WithMessage("Razão Social não pode ser vazia.");
    }

    [Fact]
    public void Atualizar_ComDadosValidos_DeveAtualizarEMarcarDataAtualizacao()
    {
        var pj = CriarPJ();

        pj.Atualizar("Novo Nome", "novo@email.com", "1144332211",
            CnpjValido, "Nova Razao Social Ltda", "Nova Fantasia",
            "RJ-999888777", DataAberturaValida);

        pj.Nome.Should().Be("Novo Nome");
        pj.RazaoSocial.Should().Be("Nova Razao Social Ltda");
        pj.AtualizadoEm.Should().NotBeNull();
    }

    [Fact]
    public void Desativar_DeveDefinirAtivoComoFalse()
    {
        var pj = CriarPJ();
        pj.Desativar();
        pj.Ativo.Should().BeFalse();
    }
}
