using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace CadastroPessoas.UnitTests.Domain;

public class PessoaFisicaTests
{
    private const string CpfValido = "52998224725"; // CPF válido para testes
    private const string NomeValido = "João da Silva";
    private const string EmailValido = "joao@email.com";
    private const string TelefoneValido = "11999887766";
    private readonly DateTime DataNascimentoValida = new DateTime(1990, 5, 15);

    [Fact]
    public void Criar_ComDadosValidos_DeveCriarComSucesso()
    {
        // Arrange & Act
        var pf = new PessoaFisica(NomeValido, EmailValido, TelefoneValido, CpfValido, DataNascimentoValida);

        // Assert
        pf.Id.Should().NotBeEmpty();
        pf.Nome.Should().Be(NomeValido);
        pf.Email.Should().Be(EmailValido);
        pf.Telefone.Should().Be(TelefoneValido);
        pf.Cpf.Should().Be(CpfValido);
        pf.DataNascimento.Should().Be(DataNascimentoValida);
        pf.Ativo.Should().BeTrue();
        pf.CriadoEm.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Criar_ComNomeVazio_DeveLancarDomainException(string? nome)
    {
        // Act
        var act = () => new PessoaFisica(nome!, EmailValido, TelefoneValido, CpfValido, DataNascimentoValida);

        // Assert
        act.Should().Throw<DomainException>().WithMessage("Nome não pode ser vazio.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Criar_ComEmailVazio_DeveLancarDomainException(string? email)
    {
        var act = () => new PessoaFisica(NomeValido, email!, TelefoneValido, CpfValido, DataNascimentoValida);
        act.Should().Throw<DomainException>().WithMessage("E-mail não pode ser vazio.");
    }

    [Theory]
    [InlineData("00000000000")]
    [InlineData("11111111111")]
    [InlineData("123456789")]
    [InlineData("abcdefghijk")]
    public void Criar_ComCpfInvalido_DeveLancarDomainException(string cpfInvalido)
    {
        var act = () => new PessoaFisica(NomeValido, EmailValido, TelefoneValido, cpfInvalido, DataNascimentoValida);
        act.Should().Throw<DomainException>().WithMessage("CPF inválido.");
    }

    [Fact]
    public void Criar_ComDataNascimentoNoFuturo_DeveLancarDomainException()
    {
        var dataFutura = DateTime.UtcNow.AddDays(1);
        var act = () => new PessoaFisica(NomeValido, EmailValido, TelefoneValido, CpfValido, dataFutura);
        act.Should().Throw<DomainException>().WithMessage("Data de nascimento não pode ser no futuro.");
    }

    [Fact]
    public void Criar_ComCpfFormatado_DeveRemoverMascara()
    {
        // CPF com máscara
        var pf = new PessoaFisica(NomeValido, EmailValido, TelefoneValido, "529.982.247-25", DataNascimentoValida);
        pf.Cpf.Should().Be(CpfValido);
    }

    [Fact]
    public void Atualizar_ComDadosValidos_DeveAtualizarEMarcarDataAtualizacao()
    {
        var pf = new PessoaFisica(NomeValido, EmailValido, TelefoneValido, CpfValido, DataNascimentoValida);

        pf.Atualizar("Novo Nome", "novo@email.com", "11888776655", CpfValido, DataNascimentoValida, "MG-123");

        pf.Nome.Should().Be("Novo Nome");
        pf.Email.Should().Be("novo@email.com");
        pf.AtualizadoEm.Should().NotBeNull();
        pf.AtualizadoEm!.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Desativar_DeveDefinirAtivoComoFalse()
    {
        var pf = new PessoaFisica(NomeValido, EmailValido, TelefoneValido, CpfValido, DataNascimentoValida);
        pf.Desativar();
        pf.Ativo.Should().BeFalse();
    }

    [Fact]
    public void AssociarEndereco_ComEnderecoValido_DeveAssociarCorretamente()
    {
        var pf = new PessoaFisica(NomeValido, EmailValido, TelefoneValido, CpfValido, DataNascimentoValida);
        var endereco = new Endereco("01310100", "Av. Paulista", "1000", "Bela Vista", "São Paulo", "SP");

        pf.AssociarEndereco(endereco);

        pf.Endereco.Should().NotBeNull();
        pf.Endereco!.Logradouro.Should().Be("Av. Paulista");
    }

    [Fact]
    public void AssociarEndereco_ComEnderecoNulo_DeveLancarDomainException()
    {
        var pf = new PessoaFisica(NomeValido, EmailValido, TelefoneValido, CpfValido, DataNascimentoValida);
        var act = () => pf.AssociarEndereco(null!);
        act.Should().Throw<DomainException>().WithMessage("Endereço não pode ser nulo.");
    }

    [Fact]
    public void Idade_DeveCalcularCorretamente()
    {
        var dataNasc = DateTime.UtcNow.AddYears(-30);
        var pf = new PessoaFisica(NomeValido, EmailValido, TelefoneValido, CpfValido, dataNasc);
        pf.Idade.Should().BeInRange(29, 30);
    }
}
