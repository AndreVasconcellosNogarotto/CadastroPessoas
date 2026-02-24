using CadastroPessoas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadastroPessoas.Infrastructure.Persistence.EF.Mappings;

public class PessoaJuridicaMapping : IEntityTypeConfiguration<PessoaJuridica>
{
    public void Configure(EntityTypeBuilder<PessoaJuridica> builder)
    {
        builder.ToTable("PessoasJuridicas");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Email).IsRequired().HasMaxLength(254);
        builder.Property(p => p.Telefone).IsRequired().HasMaxLength(20);
        builder.Property(p => p.Cnpj).IsRequired().HasMaxLength(14);
        builder.Property(p => p.RazaoSocial).IsRequired().HasMaxLength(300);
        builder.Property(p => p.NomeFantasia).IsRequired().HasMaxLength(300);
        builder.Property(p => p.InscricaoEstadual).HasMaxLength(20);
        builder.Property(p => p.DataAbertura).IsRequired();
        builder.Property(p => p.Ativo).IsRequired();
        builder.Property(p => p.CriadoEm).IsRequired();

        builder.HasIndex(p => p.Cnpj).IsUnique();
        builder.HasIndex(p => p.Email);

        builder.HasOne(p => p.Endereco)
            .WithMany()
            .HasForeignKey("EnderecoId")
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
