using CadastroPessoas.Application.UseCases.Endereco;
using CadastroPessoas.Application.UseCases.PessoaFisica;
using CadastroPessoas.Application.UseCases.PessoaJuridica;
using CadastroPessoas.Domain.Ports.Inbound;
using CadastroPessoas.Domain.Ports.Outbound;
using CadastroPessoas.Infrastructure.ExternalServices.ViaCep;
using CadastroPessoas.Infrastructure.Persistence.EF.Context;
using CadastroPessoas.Infrastructure.Persistence.EF.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CadastroPessoas.API.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Application Use Cases (Primary Ports implementations)
        services.AddScoped<IPessoaFisicaUseCase, PessoaFisicaUseCase>();
        services.AddScoped<IPessoaJuridicaUseCase, PessoaJuridicaUseCase>();
        services.AddScoped<IEnderecoUseCase, EnderecoUseCase>();

        return services;
    }

    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // EF Core (SQL Server)
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("SqlServer"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            )
        );

        // Repositories (Secondary Port implementations - SQL)
        services.AddScoped<IPessoaFisicaRepository, PessoaFisicaRepository>();
        services.AddScoped<IPessoaJuridicaRepository, PessoaJuridicaRepository>();
        services.AddScoped<IEnderecoRepository, EnderecoRepository>();

        // ViaCEP (Secondary Adapter - External Service)
        services.AddHttpClient<IViaCepService, ViaCepAdapter>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        return services;
    }
}
