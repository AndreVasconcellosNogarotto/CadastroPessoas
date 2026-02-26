import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
  AbstractControl,
} from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { InputTextModule } from 'primeng/inputtext';
import { CalendarModule } from 'primeng/calendar';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { DividerModule } from 'primeng/divider';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { MessageService } from 'primeng/api';
import { PessoaJuridicaService, EnderecoService } from '../../../../core/services/services';
import { MaskPipe } from '../../../../core/pipes/mask.pipe';

// ── Validator: CEP deve ter exatamente 8 dígitos ────────────────────────────
function cepValidator(control: AbstractControl) {
  const digits = (control.value ?? '').replace(/\D/g, '');
  if (!digits) return null;
  return digits.length === 8 ? null : { cepInvalido: true };
}
// ─────────────────────────────────────────────────────────────────────────────

@Component({
  selector: 'app-pessoa-juridica-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    RouterLink,
    InputTextModule,
    CalendarModule,
    ButtonModule,
    CardModule,
    DividerModule,
    ProgressSpinnerModule,
    MaskPipe,
  ],
  providers: [],
  styles: [`
    .campo-bloqueado {
      background-color: #f5f5f5 !important;
      color: #888 !important;
      cursor: not-allowed !important;
      pointer-events: none;
    }
  `],
  template: `
    <div class="flex align-items-center gap-2 mb-3">
      <p-button icon="pi pi-arrow-left" [text]="true" routerLink="/pessoas-juridicas" />
      <h2 class="m-0">{{ isEdit ? 'Editar' : 'Nova' }} Pessoa Jurídica</h2>
    </div>

    <div *ngIf="loading" class="flex justify-content-center p-5">
      <p-progressSpinner />
    </div>

    <form *ngIf="!loading" [formGroup]="form" (ngSubmit)="salvar()">
      <p-card header="Dados da Empresa" styleClass="mb-3">
        <div class="formgrid grid">

          <div class="field col-12 md:col-6">
            <label for="razaoSocial">Razão Social *</label>
            <input pInputText id="razaoSocial" formControlName="razaoSocial" class="w-full" maxlength="300"
              [ngClass]="{ 'ng-invalid ng-dirty': isInvalid('razaoSocial') }" />
            <div class="flex justify-content-between">
              <small *ngIf="isInvalid('razaoSocial')" class="p-error">Razão Social é obrigatória.</small>
              <small class="text-color-secondary ml-auto">{{ form.get('razaoSocial')?.value?.length || 0 }}/300</small>
            </div>
          </div>

          <div class="field col-12 md:col-6">
            <label for="nomeFantasia">Nome Fantasia *</label>
            <input pInputText id="nomeFantasia" formControlName="nomeFantasia" class="w-full" maxlength="300"
              [ngClass]="{ 'ng-invalid ng-dirty': isInvalid('nomeFantasia') }" />
            <div class="flex justify-content-between">
              <small *ngIf="isInvalid('nomeFantasia')" class="p-error">Nome Fantasia é obrigatório.</small>
              <small class="text-color-secondary ml-auto">{{ form.get('nomeFantasia')?.value?.length || 0 }}/300</small>
            </div>
          </div>

          <div class="field col-12 md:col-4">
            <label for="cnpj">CNPJ *</label>
            <input pInputText id="cnpj" formControlName="cnpj" class="w-full"
              maxlength="18" placeholder="00.000.000/0000-00"
              (input)="aplicarMascara('cnpj', 'cnpj')"
              [ngClass]="{
                'ng-invalid ng-dirty': isInvalid('cnpj'),
                'campo-bloqueado': isEdit
              }" />
            <small *ngIf="isInvalid('cnpj')" class="p-error">CNPJ é obrigatório.</small>
            <small *ngIf="isEdit" class="text-color-secondary">
              <i class="pi pi-lock" style="font-size:0.75rem"></i> CNPJ não pode ser alterado
            </small>
          </div>

          <div class="field col-12 md:col-4">
            <label for="inscricaoEstadual">Inscrição Estadual</label>
            <input pInputText id="inscricaoEstadual" formControlName="inscricaoEstadual" class="w-full"
              maxlength="20" placeholder="Opcional" />
            <small class="text-color-secondary">{{ form.get('inscricaoEstadual')?.value?.length || 0 }}/20</small>
          </div>

          <div class="field col-12 md:col-4">
            <label for="dataAbertura">Data de Abertura *</label>
            <p-calendar id="dataAbertura" formControlName="dataAbertura" class="w-full"
              dateFormat="dd/mm/yy" [showIcon]="true" [maxDate]="hoje"
              [disabled]="isEdit"
              [ngClass]="{ 'ng-invalid ng-dirty': isInvalid('dataAbertura') }" />
            <small *ngIf="isInvalid('dataAbertura')" class="p-error">Data de abertura é obrigatória.</small>
            <small *ngIf="isEdit" class="text-color-secondary">
              <i class="pi pi-lock" style="font-size:0.75rem"></i> Data de abertura não pode ser alterada
            </small>
          </div>

          <div class="field col-12 md:col-6">
            <label for="nome">Nome do Responsável *</label>
            <input pInputText id="nome" formControlName="nome" class="w-full" maxlength="200"
              [ngClass]="{ 'ng-invalid ng-dirty': isInvalid('nome') }" />
            <div class="flex justify-content-between">
              <small *ngIf="isInvalid('nome')" class="p-error">Nome é obrigatório.</small>
              <small class="text-color-secondary ml-auto">{{ form.get('nome')?.value?.length || 0 }}/200</small>
            </div>
          </div>

          <div class="field col-12 md:col-6">
            <label for="email">E-mail *</label>
            <input pInputText id="email" formControlName="email" class="w-full"
              type="email" maxlength="254"
              [ngClass]="{ 'ng-invalid ng-dirty': isInvalid('email') }" />
            <small *ngIf="isInvalid('email')" class="p-error">E-mail inválido.</small>
          </div>

          <div class="field col-12 md:col-6">
            <label for="telefone">Telefone *</label>
            <input pInputText id="telefone" formControlName="telefone" class="w-full"
              maxlength="15" placeholder="(00) 00000-0000"
              (input)="aplicarMascara('telefone', 'telefone')"
              [ngClass]="{ 'ng-invalid ng-dirty': isInvalid('telefone') }" />
            <small *ngIf="isInvalid('telefone')" class="p-error">Telefone é obrigatório.</small>
          </div>

        </div>
      </p-card>

      <p-card header="Endereço" styleClass="mb-3">
        <ng-container formGroupName="endereco">
          <div class="formgrid grid">

            <div class="field col-12 md:col-4">
              <label for="cep">CEP *</label>
              <div class="p-inputgroup">
                <input pInputText id="cep" formControlName="cep"
                  maxlength="9" placeholder="00000-000"
                  (input)="aplicarMascara('cep', 'endereco.cep')"
                  (blur)="buscarCep()"
                  [ngClass]="{ 'ng-invalid ng-dirty': isInvalid('endereco.cep') }" />
                <p-button icon="pi pi-search" [loading]="buscandoCep"
                  (onClick)="buscarCep()" severity="secondary" />
              </div>
              <small *ngIf="form.get('endereco.cep')?.hasError('required') && form.get('endereco.cep')?.touched" class="p-error">
                CEP é obrigatório.
              </small>
              <small *ngIf="form.get('endereco.cep')?.hasError('cepInvalido') && form.get('endereco.cep')?.touched" class="p-error">
                CEP inválido — informe os 8 dígitos.
              </small>
            </div>

            <div class="field col-12 md:col-8">
              <label for="logradouro">Logradouro</label>
              <input pInputText id="logradouro" formControlName="logradouro" class="w-full" maxlength="200" />
            </div>

            <div class="field col-12 md:col-2">
              <label for="numero">Número *</label>
              <input pInputText id="numero" formControlName="numero" class="w-full" maxlength="20"
                [ngClass]="{ 'ng-invalid ng-dirty': isInvalid('endereco.numero') }" />
              <small *ngIf="isInvalid('endereco.numero')" class="p-error">Número é obrigatório.</small>
            </div>

            <div class="field col-12 md:col-4">
              <label for="complemento">Complemento</label>
              <input pInputText id="complemento" formControlName="complemento" class="w-full" maxlength="100" />
            </div>

            <div class="field col-12 md:col-6">
              <label for="bairro">Bairro</label>
              <input pInputText id="bairro" formControlName="bairro" class="w-full" maxlength="100" />
            </div>

            <div class="field col-12 md:col-8">
              <label for="cidade">Cidade</label>
              <input pInputText id="cidade" formControlName="cidade" class="w-full" maxlength="100" />
            </div>

            <div class="field col-12 md:col-4">
              <label for="uf">UF</label>
              <input pInputText id="uf" formControlName="uf" class="w-full" maxlength="2"
                style="text-transform: uppercase" />
            </div>

          </div>
        </ng-container>
      </p-card>

      <div class="flex gap-2 justify-content-end">
        <p-button label="Cancelar" severity="secondary" routerLink="/pessoas-juridicas" />
        <p-button label="Salvar" type="submit" [loading]="salvando" icon="pi pi-check" />
      </div>
    </form>
  `,
})
export class PessoaJuridicaFormComponent implements OnInit {
  private readonly fb              = inject(FormBuilder);
  private readonly service         = inject(PessoaJuridicaService);
  private readonly enderecoService = inject(EnderecoService);
  private readonly messageService  = inject(MessageService);
  private readonly router          = inject(Router);
  private readonly route           = inject(ActivatedRoute);

  form!: FormGroup;
  loading     = false;
  salvando    = false;
  buscandoCep = false;
  isEdit      = false;
  hoje        = new Date();
  private id?: string;

  private readonly camposLabel: Record<string, string> = {
    razaoSocial:        "Razão Social",
    nomeFantasia:       "Nome Fantasia",
    cnpj:               "CNPJ",
    dataAbertura:       "Data de Abertura",
    nome:               "Nome do Responsável",
    email:              "E-mail",
    telefone:           "Telefone",
    "endereco.cep":     "CEP",
    "endereco.numero":  "Número",
  };

  ngOnInit(): void {
    this.buildForm();
    this.id    = this.route.snapshot.params['id'];
    this.isEdit = !!this.id;

    if (this.isEdit) {
      this.loading = true;
      this.service.obterPorId(this.id!).subscribe({
        next: (p) => {
          this.form.patchValue({
            ...p,
            cnpj:         this.formatarCnpj(p.cnpj),
            telefone:     this.formatarTelefone(p.telefone),
            dataAbertura: new Date(p.dataAbertura),
            endereco: p.endereco
              ? { ...p.endereco, cep: this.formatarCep(p.endereco.cep) }
              : {},
          });
          this.form.get('cnpj')?.disable();
          this.form.get('dataAbertura')?.disable();
          this.loading = false;
        },
        error: () => (this.loading = false),
      });
    }
  }

  buildForm(): void {
    this.form = this.fb.group({
      nome:              ['', [Validators.required, Validators.maxLength(200)]],
      email:             ['', [Validators.required, Validators.email, Validators.maxLength(254)]],
      telefone:          ['', [Validators.required]],
      cnpj:              ['', [Validators.required]],
      razaoSocial:       ['', [Validators.required, Validators.maxLength(300)]],
      nomeFantasia:      ['', [Validators.required, Validators.maxLength(300)]],
      inscricaoEstadual: [''],
      dataAbertura:      [null, [Validators.required]],
      endereco: this.fb.group({
        cep:         ['', [Validators.required, cepValidator]],
        logradouro:  [''],
        numero:      ['', [Validators.required]],
        complemento: [''],
        bairro:      [''],
        cidade:      [''],
        uf:          [''],
      }),
    });
  }

  isInvalid(field: string): boolean {
    const control = this.form.get(field);
    return !!(control?.invalid && (control.dirty || control.touched));
  }

  // ── Formatadores ──────────────────────────────────────────────────────────
  private formatarCnpj(cnpj: string): string {
    const d = (cnpj ?? '').replace(/\D/g, '').slice(0, 14);
    return d
      .replace(/(\d{2})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d)/, '$1.$2')
      .replace(/(\d{3})(\d)/, '$1/$2')
      .replace(/(\d{4})(\d{1,2})$/, '$1-$2');
  }

  private formatarTelefone(tel: string): string {
    const d = (tel ?? '').replace(/\D/g, '').slice(0, 11);
    return d.length <= 10
      ? d.replace(/(\d{2})(\d{4})(\d{0,4})/, '($1) $2-$3')
      : d.replace(/(\d{2})(\d{5})(\d{0,4})/, '($1) $2-$3');
  }

  private formatarCep(cep: string): string {
    const d = (cep ?? '').replace(/\D/g, '').slice(0, 8);
    return d.replace(/(\d{5})(\d{0,3})/, '$1-$2');
  }
  // ─────────────────────────────────────────────────────────────────────────

  aplicarMascara(tipo: 'cnpj' | 'telefone' | 'cep', campo: string): void {
    const control = this.form.get(campo);
    if (!control) return;
    const digits = (control.value ?? '').replace(/\D/g, '');
    let formatted = digits;

    switch (tipo) {
      case 'cnpj':
        formatted = digits.slice(0, 14)
          .replace(/(\d{2})(\d)/, '$1.$2')
          .replace(/(\d{3})(\d)/, '$1.$2')
          .replace(/(\d{3})(\d)/, '$1/$2')
          .replace(/(\d{4})(\d{1,2})$/, '$1-$2');
        break;
      case 'telefone': {
        const t = digits.slice(0, 11);
        formatted = t.length <= 10
          ? t.replace(/(\d{2})(\d{4})(\d{0,4})/, '($1) $2-$3')
          : t.replace(/(\d{2})(\d{5})(\d{0,4})/, '($1) $2-$3');
        break;
      }
      case 'cep':
        formatted = digits.slice(0, 8).replace(/(\d{5})(\d{0,3})/, '$1-$2');
        break;
    }

    control.setValue(formatted, { emitEvent: false });
  }

  buscarCep(): void {
    const cep    = this.form.get('endereco.cep')?.value ?? '';
    const digits = cep.replace(/\D/g, '');
    if (digits.length !== 8) return;

    this.buscandoCep = true;
    this.enderecoService.consultarCep(digits).subscribe({
      next: (res) => {
        this.form.get('endereco')?.patchValue({
          logradouro: res.logradouro ?? '',
          bairro:     res.bairro     ?? '',
          cidade:     res.cidade     ?? '',
          uf:         res.uf         ?? '',
        });
        this.buscandoCep = false;
      },
      error: () => {
        this.form.get('endereco')?.patchValue({
          logradouro: '', numero: '', complemento: '', bairro: '', cidade: '', uf: '',
        });
        this.messageService.add({
          severity: 'warn',
          summary:  'CEP não encontrado',
          detail:   'Nenhum endereço foi localizado para o CEP informado. Verifique e tente novamente.',
          life:     5000,
        });
        this.buscandoCep = false;
      },
    });
  }

  salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();

      const faltando = Object.keys(this.camposLabel)
        .filter((campo) => this.form.get(campo)?.invalid)
        .map((campo) => this.camposLabel[campo]);

      this.messageService.add({
        severity: 'warn',
        summary:  'Campos obrigatórios não preenchidos',
        detail:   `Preencha corretamente: ${faltando.join(', ')}.`,
        life:     6000,
      });
      return;
    }

    const f = this.form.getRawValue();
    if (!f.dataAbertura) return;

    const cepDigits   = (f.endereco?.cep ?? '').replace(/\D/g, '');
    const temEndereco = cepDigits.length === 8;

    const payload: any = {
      nome:              f.nome,
      email:             f.email,
      telefone:          (f.telefone ?? '').replace(/\D/g, ''),
      cnpj:              (f.cnpj     ?? '').replace(/\D/g, ''),
      razaoSocial:       f.razaoSocial,
      nomeFantasia:      f.nomeFantasia,
      inscricaoEstadual: f.inscricaoEstadual ?? '',
      dataAbertura:      (f.dataAbertura as Date).toISOString(),
    };

    if (temEndereco) {
      payload.endereco = { cep: cepDigits };
      if (f.endereco.logradouro)  payload.endereco.logradouro  = f.endereco.logradouro;
      if (f.endereco.numero)      payload.endereco.numero      = f.endereco.numero;
      if (f.endereco.bairro)      payload.endereco.bairro      = f.endereco.bairro;
      if (f.endereco.cidade)      payload.endereco.cidade      = f.endereco.cidade;
      if (f.endereco.uf)          payload.endereco.uf          = f.endereco.uf;
      if (f.endereco.complemento) payload.endereco.complemento = f.endereco.complemento;
    }

    this.salvando = true;
    const req$ = this.isEdit
      ? this.service.atualizar(this.id!, payload)
      : this.service.criar(payload);

    req$.subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary:  'Sucesso',
          detail:   `Pessoa jurídica ${this.isEdit ? 'atualizada' : 'criada'} com sucesso.`,
        });
        this.router.navigate(['/pessoas-juridicas']);
      },
      error: () => (this.salvando = false),
    });
  }
}