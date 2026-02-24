import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideAnimations } from '@angular/platform-browser/animations';
import { of, throwError } from 'rxjs';
import { MessageService, ConfirmationService } from 'primeng/api';
import { PessoaFisicaListComponent } from './pessoa-fisica-list.component';
import { PessoaFisicaService } from '../../../../core/services/services';

describe('PessoaFisicaListComponent', () => {
  let component: PessoaFisicaListComponent;
  let fixture: ComponentFixture<PessoaFisicaListComponent>;
  let serviceSpy: jasmine.SpyObj<PessoaFisicaService>;

  const mockPagedResponse = {
    data: [
      {
        id: '1',
        nome: 'João Silva',
        cpf: '52998224725',
        email: 'joao@email.com',
        telefone: '11999887766',
        dataNascimento: '1990-05-15',
        idade: 34,
        ativo: true,
        criadoEm: new Date().toISOString(),
      }
    ],
    page: 1,
    pageSize: 10,
    total: 1,
    totalPages: 1,
  };

  beforeEach(async () => {
    serviceSpy = jasmine.createSpyObj('PessoaFisicaService', ['listar', 'deletar']);
    serviceSpy.listar.and.returnValue(of(mockPagedResponse));

    await TestBed.configureTestingModule({
      imports: [PessoaFisicaListComponent],
      providers: [
        provideRouter([]),
        provideHttpClient(),
        provideHttpClientTesting(),
        provideAnimations(),
        MessageService,
        ConfirmationService,
        { provide: PessoaFisicaService, useValue: serviceSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(PessoaFisicaListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('deve criar o componente', () => {
    expect(component).toBeTruthy();
  });

  it('deve carregar a lista de pessoas físicas ao inicializar', () => {
    expect(serviceSpy.listar).toHaveBeenCalledWith(1, 10);
    expect(component.pessoas.length).toBe(1);
    expect(component.total).toBe(1);
    expect(component.loading).toBeFalse();
  });

  it('deve definir loading como false mesmo com erro no serviço', () => {
    serviceSpy.listar.and.returnValue(throwError(() => new Error('Server error')));
    component.carregar();
    expect(component.loading).toBeFalse();
  });

  it('deve recarregar ao executar onLazyLoad', () => {
    component.onLazyLoad({ first: 10, rows: 10 });
    expect(component.page).toBe(2);
    expect(component.pageSize).toBe(10);
    expect(serviceSpy.listar).toHaveBeenCalledWith(2, 10);
  });

  it('deve chamar deletar e recarregar a lista', () => {
    serviceSpy.deletar.and.returnValue(of(void 0));
    component.excluir('1');
    expect(serviceSpy.deletar).toHaveBeenCalledWith('1');
    expect(serviceSpy.listar).toHaveBeenCalledTimes(2);
  });
});
