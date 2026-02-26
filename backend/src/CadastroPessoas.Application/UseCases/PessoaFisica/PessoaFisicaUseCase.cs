using CadastroPessoas.Application.DTOs.Request;
using CadastroPessoas.Application.DTOs.Response;
using CadastroPessoas.Application.Ports.Outbound;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.Ports.Inbound;
using CadastroPessoas.Domain.Ports.Outbound;
using Microsoft.Extensions.Logging;

namespace CadastroPessoas.Application.UseCases.PessoaFisica;

public class PessoaFisicaUseCase : IPessoaFisicaUseCase
{
    private readonly IPessoaFisicaRepository _repository;
    private readonly IEnderecoRepository _enderecoRepository;
    private readonly ILogger<PessoaFisicaUseCase> _logger;
    private readonly IAuditRepository _auditRepository;


    public PessoaFisicaUseCase(IPessoaFisicaRepository repository, IEnderecoRepository enderecoRepository, ILogger<PessoaFisicaUseCase> logger, IAuditRepository auditRepository)
    {
        _repository = repository;
        _enderecoRepository = enderecoRepository;
        _logger = logger;
        _auditRepository = auditRepository;

    }

    public async Task<PessoaFisicaResponse> CriarAsync(CreatePessoaFisicaRequest request, CancellationToken cancellationToken = default)
    {
        if (await _repository.ExistsByCpfAsync(request.Cpf, cancellationToken: cancellationToken))
            throw new ConflictException($"Já existe um cadastro com o CPF '{request.Cpf}'.");

        var pessoaFisica = new Domain.Entities.PessoaFisica(
            request.Nome,
            request.Email,
            request.Telefone,
            request.Cpf,
            request.DataNascimento,
            request.RG
        );

        if (request.Endereco is not null)
        {
            var endereco = new Domain.Entities.Endereco(
                request.Endereco.Cep,
                request.Endereco.Logradouro,
                request.Endereco.Numero,
                request.Endereco.Bairro,
                request.Endereco.Cidade,
                request.Endereco.Uf,
                request.Endereco.Complemento
            );
            await _enderecoRepository.AddAsync(endereco, cancellationToken);
            pessoaFisica.AssociarEndereco(endereco);
        }

        await _repository.AddAsync(pessoaFisica, cancellationToken);

        return MapToResponse(pessoaFisica);
    }

    public async Task<PessoaFisicaResponse> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pessoaFisica = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Pessoa Física com id '{id}' não encontrada.");

        return MapToResponse(pessoaFisica);
    }

    public async Task<PagedResponse<PessoaFisicaResponse>> ListarAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var total = await _repository.CountAsync(cancellationToken);
        var items = await _repository.GetAllAsync(page, pageSize, cancellationToken);
        var totalPages = (int)Math.Ceiling((double)total / pageSize);

        return new PagedResponse<PessoaFisicaResponse>(
            items.Select(MapToResponse),
            page, pageSize, total, totalPages
        );
    }

    public async Task<PessoaFisicaResponse> AtualizarAsync(Guid id, UpdatePessoaFisicaRequest request, CancellationToken cancellationToken = default)
    {
        var pessoaFisica = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Pessoa Física com id '{id}' não encontrada.");

        if (await _repository.ExistsByCpfAsync(request.Cpf, id, cancellationToken))
            throw new ConflictException($"Já existe outro cadastro com o CPF '{request.Cpf}'.");

        pessoaFisica.Atualizar(
            request.Nome, request.Email, request.Telefone,
            request.Cpf, request.DataNascimento, request.RG
        );

        if (request.Endereco is not null)
        {
            if (pessoaFisica.Endereco is not null)
            {
                pessoaFisica.Endereco.Atualizar(
                    request.Endereco.Cep, request.Endereco.Logradouro,
                    request.Endereco.Numero, request.Endereco.Bairro,
                    request.Endereco.Cidade, request.Endereco.Uf,
                    request.Endereco.Complemento
                );
                await _enderecoRepository.UpdateAsync(pessoaFisica.Endereco, cancellationToken);
            }
            else
            {
                var novoEndereco = new Domain.Entities.Endereco(
                    request.Endereco.Cep, request.Endereco.Logradouro,
                    request.Endereco.Numero, request.Endereco.Bairro,
                    request.Endereco.Cidade, request.Endereco.Uf,
                    request.Endereco.Complemento
                );
                await _enderecoRepository.AddAsync(novoEndereco, cancellationToken);
                pessoaFisica.AssociarEndereco(novoEndereco);
            }
        }

        await _repository.UpdateAsync(pessoaFisica, cancellationToken);

        return MapToResponse(pessoaFisica);
    }
    public async Task DeletarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pessoa = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Pessoa Física com Id '{id}' não encontrada.");

        await _repository.DeleteAsync(pessoa, cancellationToken);

        var audit = new AuditLog(pessoa.Id, "PessoaFisica");
        await _auditRepository.RegistrarAsync(audit, cancellationToken);
    }

    private static PessoaFisicaResponse MapToResponse(Domain.Entities.PessoaFisica p) =>
        new(
            p.Id, p.Nome, p.Email, p.Telefone, p.Cpf,
            p.DataNascimento, p.Idade, p.RG, p.Ativo,
            p.CriadoEm, p.AtualizadoEm,
            p.Endereco is null ? null : new EnderecoResponse(
                p.Endereco.Id, p.Endereco.Cep, p.Endereco.Logradouro,
                p.Endereco.Numero, p.Endereco.Complemento, p.Endereco.Bairro,
                p.Endereco.Cidade, p.Endereco.Uf
            )
        );
}
