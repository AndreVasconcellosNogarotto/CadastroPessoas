using CadastroPessoas.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CadastroPessoas.Infrastructure.Persistence.EF.Mappings;

public class EnderecoMapping : IEntityTypeConfiguration<Endereco>
{
    public void Configure(EntityTypeBuilder<Endereco> builder)
    {
        builder.ToTable("Enderecos");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Cep).IsRequired().HasMaxLength(8);
        builder.Property(e => e.Logradouro).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Numero).IsRequired().HasMaxLength(20);
        builder.Property(e => e.Complemento).HasMaxLength(100);
        builder.Property(e => e.Bairro).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Cidade).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Uf).IsRequired().HasMaxLength(2);
        builder.Property(e => e.Ibge).HasMaxLength(20);
        builder.Property(e => e.CriadoEm).IsRequired();
    }
}
