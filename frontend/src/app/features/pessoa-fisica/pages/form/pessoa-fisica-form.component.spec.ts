import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter, ActivatedRoute } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideAnimations } from '@angular/platform-browser/animations';
import { of } from 'rxjs';
import { MessageService } from 'primeng/api';
import { PessoaFisicaFormComponent } from './pessoa-fisica-form.component';
import { PessoaFisicaService, EnderecoService } from '../../../../core/services/services';

describe('PessoaFisicaFormComponent', () => {
  let component: PessoaFisicaFormComponent;
  let fixture: ComponentFixture<PessoaFisicaFormComponent>;
  let pessoaFisicaServiceSpy: jasmine.SpyObj<PessoaFisicaService>;
  let enderecoServiceSpy: jasmine.SpyObj<EnderecoService>;

  beforeEach(async () => {
    pessoaFisicaServiceSpy = jasmine.createSpyObj('PessoaFisicaService', ['criar', 'atualizar', 'obterPorId']);
    enderecoServiceSpy = jasmine.createSpyObj('EnderecoService', ['consultarCep']);

    await TestBed.configureTestingModule({
      imports: [PessoaFisicaFormComponent],
      providers: [
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting(),
        provideAnimations(),
        MessageService,
        { provide: PessoaFisicaService, useValue: pessoaFisicaServiceSpy },
        { provide: EnderecoService, useValue: enderecoServiceSpy },
        {
          provide: ActivatedRoute,
          useValue: { snapshot: { params: {} } },
        },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(PessoaFisicaFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('deve criar o componente', () => {
    expect(component).toBeTruthy();
  });

  it('deve inicializar o formulário', () => {
    expect(component.form).toBeDefined();
    expect(component.form.get('nome')).toBeTruthy();
    expect(component.form.get('cpf')).toBeTruthy();
    expect(component.form.get('email')).toBeTruthy();
  });

  it('deve marcar campos como inválidos ao submeter formulário vazio', () => {
    component.salvar();
    expect(component.form.invalid).toBeTrue();
    expect(component.form.get('nome')?.touched).toBeTrue();
  });

  it('deve consultar o CEP ao chamar buscarCep', () => {
    const viaCepMock = {
      cep: '01310-100', logradouro: 'Av. Paulista', complemento: '',
      bairro: 'Bela Vista', cidade: 'São Paulo', uf: 'SP', ibge: '', ddd: '11',
    };
    enderecoServiceSpy.consultarCep.and.returnValue(of(viaCepMock));
    component.form.get('endereco.cep')?.setValue('01310100');

    component.buscarCep();

    expect(enderecoServiceSpy.consultarCep).toHaveBeenCalledWith('01310100');
    expect(component.form.get('endereco.logradouro')?.value).toBe('Av. Paulista');
    expect(component.form.get('endereco.cidade')?.value).toBe('São Paulo');
  });

  it('não deve consultar CEP se tiver menos de 8 dígitos', () => {
    component.form.get('endereco.cep')?.setValue('0131');
    component.buscarCep();
    expect(enderecoServiceSpy.consultarCep).not.toHaveBeenCalled();
  });

  it('deve chamar o serviço de criar ao submeter formulário válido', () => {
    const mockPessoa = { id: '1', nome: 'João', cpf: '52998224725', email: 'j@j.com', telefone: '11999887766', dataNascimento: '1990-05-15', idade: 34, ativo: true, criadoEm: new Date().toISOString() };
    pessoaFisicaServiceSpy.criar.and.returnValue(of(mockPessoa as any));

    component.form.patchValue({
      nome: 'João Silva',
      email: 'joao@email.com',
      telefone: '11999887766',
      cpf: '52998224725',
      dataNascimento: new Date('1990-05-15'),
    });

    component.salvar();
    expect(pessoaFisicaServiceSpy.criar).toHaveBeenCalled();
  });
});
