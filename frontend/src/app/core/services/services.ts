import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PessoaFisica, PagedResponse, ViaCepResponse } from '../models/models';
import { environment } from '../../../environments/environment';

export interface CreatePessoaFisicaDto {
  nome: string;
  email: string;
  telefone: string;
  cpf: string;
  dataNascimento: string;
  rg?: string;
  endereco?: {
    cep: string;
    logradouro: string;
    numero: string;
    complemento?: string;
    bairro: string;
    cidade: string;
    uf: string;
  };
}

@Injectable({ providedIn: 'root' })
export class PessoaFisicaService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/pessoas-fisicas`;

  listar(page = 1, pageSize = 10): Observable<PagedResponse<PessoaFisica>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<PagedResponse<PessoaFisica>>(this.baseUrl, { params });
  }

  obterPorId(id: string): Observable<PessoaFisica> {
    return this.http.get<PessoaFisica>(`${this.baseUrl}/${id}`);
  }

  criar(dto: CreatePessoaFisicaDto): Observable<PessoaFisica> {
    return this.http.post<PessoaFisica>(this.baseUrl, dto);
  }

  atualizar(id: string, dto: CreatePessoaFisicaDto): Observable<PessoaFisica> {
    return this.http.put<PessoaFisica>(`${this.baseUrl}/${id}`, dto);
  }

  deletar(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}

@Injectable({ providedIn: 'root' })
export class PessoaJuridicaService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/pessoas-juridicas`;

  listar(page = 1, pageSize = 10): Observable<PagedResponse<any>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<PagedResponse<any>>(this.baseUrl, { params });
  }

  obterPorId(id: string): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}/${id}`);
  }

  criar(dto: any): Observable<any> {
    return this.http.post<any>(this.baseUrl, dto);
  }

  atualizar(id: string, dto: any): Observable<any> {
    return this.http.put<any>(`${this.baseUrl}/${id}`, dto);
  }

  deletar(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}

@Injectable({ providedIn: 'root' })
export class EnderecoService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/enderecos`;

  consultarCep(cep: string): Observable<ViaCepResponse> {
    const cepLimpo = cep.replace('-', '');
    return this.http.get<ViaCepResponse>(`${this.baseUrl}/cep/${cepLimpo}`);
  }
}
