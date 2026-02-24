import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { PessoaFisicaService, EnderecoService } from './services';
import { environment } from '../../../environments/environment';

describe('PessoaFisicaService', () => {
  let service: PessoaFisicaService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [PessoaFisicaService],
    });
    service = TestBed.inject(PessoaFisicaService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('deve ser criado', () => expect(service).toBeTruthy());

  it('deve chamar GET /pessoas-fisicas com paginação', () => {
    service.listar(1, 10).subscribe();
    const req = httpMock.expectOne(`${environment.apiUrl}/pessoas-fisicas?page=1&pageSize=10`);
    expect(req.request.method).toBe('GET');
    req.flush({ data: [], page: 1, pageSize: 10, total: 0, totalPages: 0 });
  });

  it('deve chamar GET /pessoas-fisicas/:id', () => {
    const id = '123';
    service.obterPorId(id).subscribe();
    const req = httpMock.expectOne(`${environment.apiUrl}/pessoas-fisicas/${id}`);
    expect(req.request.method).toBe('GET');
    req.flush({});
  });

  it('deve chamar POST /pessoas-fisicas', () => {
    const dto = { nome: 'João', email: 'j@j.com', telefone: '11999887766', cpf: '52998224725', dataNascimento: '1990-05-15' };
    service.criar(dto as any).subscribe();
    const req = httpMock.expectOne(`${environment.apiUrl}/pessoas-fisicas`);
    expect(req.request.method).toBe('POST');
    req.flush({});
  });

  it('deve chamar PUT /pessoas-fisicas/:id', () => {
    const id = '123';
    const dto = { nome: 'João', email: 'j@j.com', telefone: '11999887766', cpf: '52998224725', dataNascimento: '1990-05-15' };
    service.atualizar(id, dto as any).subscribe();
    const req = httpMock.expectOne(`${environment.apiUrl}/pessoas-fisicas/${id}`);
    expect(req.request.method).toBe('PUT');
    req.flush({});
  });

  it('deve chamar DELETE /pessoas-fisicas/:id', () => {
    const id = '123';
    service.deletar(id).subscribe();
    const req = httpMock.expectOne(`${environment.apiUrl}/pessoas-fisicas/${id}`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});

describe('EnderecoService', () => {
  let service: EnderecoService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [EnderecoService],
    });
    service = TestBed.inject(EnderecoService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('deve ser criado', () => expect(service).toBeTruthy());

  it('deve consultar CEP sem hífen', () => {
    service.consultarCep('01310-100').subscribe();
    const req = httpMock.expectOne(`${environment.apiUrl}/enderecos/cep/01310100`);
    expect(req.request.method).toBe('GET');
    req.flush({});
  });
});
