using CadastroPessoas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadastroPessoas.Infrastructure.Persistence.EF.Mappings;

public class PessoaFisicaMapping : IEntityTypeConfiguration<PessoaFisica>
{
    public void Configure(EntityTypeBuilder<PessoaFisica> builder)
    {
        builder.ToTable("PessoasFisicas");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Email).IsRequired().HasMaxLength(254);
        builder.Property(p => p.Telefone).IsRequired().HasMaxLength(20);
        builder.Property(p => p.Cpf).IsRequired().HasMaxLength(11);
        builder.Property(p => p.DataNascimento).IsRequired();
        builder.Property(p => p.RG).HasMaxLength(20);
        builder.Property(p => p.Ativo).IsRequired();
        builder.Property(p => p.CriadoEm).IsRequired();

        builder.HasIndex(p => p.Cpf).IsUnique();
        builder.HasIndex(p => p.Email);

        builder.HasOne(p => p.Endereco)
            .WithMany()
            .HasForeignKey("EnderecoId")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
