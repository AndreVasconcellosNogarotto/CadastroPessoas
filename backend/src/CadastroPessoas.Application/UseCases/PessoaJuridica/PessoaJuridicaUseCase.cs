using CadastroPessoas.Application.DTOs.Request;
using CadastroPessoas.Application.DTOs.Response;
using CadastroPessoas.Domain.Entities;
using CadastroPessoas.Domain.Exceptions;
using CadastroPessoas.Domain.Ports.Inbound;
using CadastroPessoas.Domain.Ports.Outbound;

namespace CadastroPessoas.Application.UseCases.PessoaJuridica;

public class PessoaJuridicaUseCase : IPessoaJuridicaUseCase
{
    private readonly IPessoaJuridicaRepository _repository;
    private readonly IEnderecoRepository _enderecoRepository;

    public PessoaJuridicaUseCase(IPessoaJuridicaRepository repository, IEnderecoRepository enderecoRepository)
    {
        _repository = repository;
        _enderecoRepository = enderecoRepository;
    }

    public async Task<PessoaJuridicaResponse> CriarAsync(CreatePessoaJuridicaRequest request, CancellationToken cancellationToken = default)
    {
        if (await _repository.ExistsByCnpjAsync(request.Cnpj, cancellationToken: cancellationToken))
            throw new ConflictException($"Já existe um cadastro com o CNPJ '{request.Cnpj}'.");

        var pessoaJuridica = new Domain.Entities.PessoaJuridica(
            request.Nome, request.Email, request.Telefone,
            request.Cnpj, request.RazaoSocial, request.NomeFantasia,
            request.InscricaoEstadual, request.DataAbertura
        );

        if (request.Endereco is not null)
        {
            var endereco = new Domain.Entities.Endereco(
                request.Endereco.Cep, request.Endereco.Logradouro,
                request.Endereco.Numero, request.Endereco.Bairro,
                request.Endereco.Cidade, request.Endereco.Uf,
                request.Endereco.Complemento
            );
            await _enderecoRepository.AddAsync(endereco, cancellationToken);
            pessoaJuridica.AssociarEndereco(endereco);
        }

        await _repository.AddAsync(pessoaJuridica, cancellationToken);

        return MapToResponse(pessoaJuridica);
    }

    public async Task<PessoaJuridicaResponse> ObterPorIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pj = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Pessoa Jurídica com id '{id}' não encontrada.");

        return MapToResponse(pj);
    }

    public async Task<PagedResponse<PessoaJuridicaResponse>> ListarAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var total = await _repository.CountAsync(cancellationToken);
        var items = await _repository.GetAllAsync(page, pageSize, cancellationToken);
        var totalPages = (int)Math.Ceiling((double)total / pageSize);

        return new PagedResponse<PessoaJuridicaResponse>(
            items.Select(MapToResponse),
            page, pageSize, total, totalPages
        );
    }

    public async Task<PessoaJuridicaResponse> AtualizarAsync(Guid id, UpdatePessoaJuridicaRequest request, CancellationToken cancellationToken = default)
    {
        var pj = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Pessoa Jurídica com id '{id}' não encontrada.");

        if (await _repository.ExistsByCnpjAsync(request.Cnpj, id, cancellationToken))
            throw new ConflictException($"Já existe outro cadastro com o CNPJ '{request.Cnpj}'.");

        pj.Atualizar(
            request.Nome, request.Email, request.Telefone,
            request.Cnpj, request.RazaoSocial, request.NomeFantasia,
            request.InscricaoEstadual, request.DataAbertura
        );

        if (request.Endereco is not null)
        {
            if (pj.Endereco is not null)
            {
                pj.Endereco.Atualizar(
                    request.Endereco.Cep, request.Endereco.Logradouro,
                    request.Endereco.Numero, request.Endereco.Bairro,
                    request.Endereco.Cidade, request.Endereco.Uf,
                    request.Endereco.Complemento
                );
                await _enderecoRepository.UpdateAsync(pj.Endereco, cancellationToken);
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
                pj.AssociarEndereco(novoEndereco);
            }
        }

        await _repository.UpdateAsync(pj, cancellationToken);

        return MapToResponse(pj);
    }

    public async Task DeletarAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var pj = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Pessoa Jurídica com id '{id}' não encontrada.");

        await _repository.DeleteAsync(pj, cancellationToken);
    }

    private static PessoaJuridicaResponse MapToResponse(Domain.Entities.PessoaJuridica pj) =>
        new(
            pj.Id, pj.Nome, pj.Email, pj.Telefone, pj.Cnpj,
            pj.RazaoSocial, pj.NomeFantasia, pj.InscricaoEstadual,
            pj.DataAbertura, pj.Ativo, pj.CriadoEm, pj.AtualizadoEm,
            pj.Endereco is null ? null : new EnderecoResponse(
                pj.Endereco.Id, pj.Endereco.Cep, pj.Endereco.Logradouro,
                pj.Endereco.Numero, pj.Endereco.Complemento, pj.Endereco.Bairro,
                pj.Endereco.Cidade, pj.Endereco.Uf
            )
        );
}
