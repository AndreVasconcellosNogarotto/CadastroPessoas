export interface Endereco {
  id?: string;
  cep: string;
  logradouro: string;
  numero: string;
  complemento?: string;
  bairro: string;
  cidade: string;
  uf: string;
}

export interface PessoaFisica {
  id: string;
  nome: string;
  email: string;
  telefone: string;
  cpf: string;
  dataNascimento: string;
  idade: number;
  rg?: string;
  ativo: boolean;
  criadoEm: string;
  atualizadoEm?: string;
  endereco?: Endereco;
}

export interface PessoaJuridica {
  id: string;
  nome: string;
  email: string;
  telefone: string;
  cnpj: string;
  razaoSocial: string;
  nomeFantasia: string;
  inscricaoEstadual: string;
  dataAbertura: string;
  ativo: boolean;
  criadoEm: string;
  atualizadoEm?: string;
  endereco?: Endereco;
}

export interface PagedResponse<T> {
  data: T[];
  page: number;
  pageSize: number;
  total: number;
  totalPages: number;
}

export interface ViaCepResponse {
  cep: string;
  logradouro: string;
  complemento: string;
  bairro: string;
  cidade: string;
  uf: string;
  ibge: string;
  ddd: string;
}

export interface ErrorResponse {
  type: string;
  message: string;
  errors?: string[];
}
