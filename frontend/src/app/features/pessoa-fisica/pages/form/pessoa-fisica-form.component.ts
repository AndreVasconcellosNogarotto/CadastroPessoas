import { Component, OnInit, inject } from "@angular/core";
import { CommonModule } from "@angular/common";
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from "@angular/forms";
import { Router, ActivatedRoute, RouterLink } from "@angular/router";
import { InputTextModule } from "primeng/inputtext";
import { CalendarModule } from "primeng/calendar";
import { ButtonModule } from "primeng/button";
import { CardModule } from "primeng/card";
import { DividerModule } from "primeng/divider";
import { ProgressSpinnerModule } from "primeng/progressspinner";
import { MessageService } from "primeng/api";
import { MaskPipe } from '../../../../core/pipes/mask.pipe';

import {
  PessoaFisicaService,
  EnderecoService,
} from "../../../../core/services/services";

@Component({
  selector: "app-pessoa-fisica-form",
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
  ],providers: [],
  template: `
    <div class="flex align-items-center gap-2 mb-3">
      <p-button
        icon="pi pi-arrow-left"
        [text]="true"
        routerLink="/pessoas-fisicas"
      />
      <h2 class="m-0">{{ isEdit ? "Editar" : "Nova" }} Pessoa Física</h2>
    </div>

    <div *ngIf="loading" class="flex justify-content-center p-5">
      <p-progressSpinner />
    </div>

    <form *ngIf="!loading" [formGroup]="form" (ngSubmit)="salvar()">
      <p-card header="Dados Pessoais" styleClass="mb-3">
        <div class="formgrid grid">
          <div class="field col-12 md:col-6">
            <label for="nome">Nome *</label>
            <input
              pInputText
              id="nome"
              formControlName="nome"
              class="w-full"
              maxlength="200"
              [ngClass]="{ 'ng-invalid ng-dirty': isInvalid('nome') }"
            />
            <div class="flex justify-content-between">
              <small *ngIf="isInvalid('nome')" class="p-error"
                >Nome é obrigatório.</small
              >
              <small class="text-color-secondary ml-auto"
                >{{ form.get("nome")?.value?.length || 0 }}/200</small
              >
            </div>
          </div>

          <div class="field col-12 md:col-6">
            <label for="cpf">CPF *</label>
            <input
              pInputText
              id="cpf"
              formControlName="cpf"
              class="w-full"
              maxlength="14"
              placeholder="000.000.000-00"
              (input)="aplicarMascara('cpf', 'cpf')"
              [ngClass]="{ 'ng-invalid ng-dirty': isInvalid('cpf') }"
            />
            <small *ngIf="isInvalid('cpf')" class="p-error"
              >CPF é obrigatório.</small
            >
          </div>

          <div class="field col-12 md:col-6">
            <label for="email">E-mail *</label>
            <input
              pInputText
              id="email"
              formControlName="email"
              class="w-full"
              type="email"
              maxlength="254"
              [ngClass]="{ 'ng-invalid ng-dirty': isInvalid('email') }"
            />
            <small *ngIf="isInvalid('email')" class="p-error"
              >E-mail inválido.</small
            >
          </div>

          <div class="field col-12 md:col-6">
            <label for="telefone">Telefone *</label>
            <input
              pInputText
              id="telefone"
              formControlName="telefone"
              class="w-full"
              maxlength="15"
              placeholder="(00) 00000-0000"
              (input)="aplicarMascara('telefone', 'telefone')"
              [ngClass]="{ 'ng-invalid ng-dirty': isInvalid('telefone') }"
            />
            <small *ngIf="isInvalid('telefone')" class="p-error"
              >Telefone é obrigatório.</small
            >
          </div>

          <div class="field col-12 md:col-6">
            <label for="dataNascimento">Data de Nascimento *</label>
            <p-calendar
              id="dataNascimento"
              formControlName="dataNascimento"
              class="w-full"
              dateFormat="dd/mm/yy"
              [showIcon]="true"
              [maxDate]="hoje"
              [ngClass]="{ 'ng-invalid ng-dirty': isInvalid('dataNascimento') }"
            />
            <small *ngIf="isInvalid('dataNascimento')" class="p-error"
              >Data de nascimento é obrigatória.</small
            >
          </div>

          <div class="field col-12 md:col-6">
            <label for="rg">RG</label>
            <input
              pInputText
              id="rg"
              formControlName="rg"
              class="w-full"
              maxlength="20"
              placeholder="Opcional"
            />
            <small class="text-color-secondary"
              >{{ form.get("rg")?.value?.length || 0 }}/20</small
            >
          </div>
        </div>
      </p-card>

      <p-card header="Endereço" styleClass="mb-3">
        <ng-container formGroupName="endereco">
          <div class="formgrid grid">
            <div class="field col-12 md:col-4">
              <label for="cep">CEP</label>
              <div class="p-inputgroup">
                <input
                  pInputText
                  id="cep"
                  formControlName="cep"
                  maxlength="9"
                  placeholder="00000-000"
                  (input)="aplicarMascara('cep', 'endereco.cep')"
                  (blur)="buscarCep()"
                />
                <p-button
                  icon="pi pi-search"
                  [loading]="buscandoCep"
                  (onClick)="buscarCep()"
                  severity="secondary"
                />
              </div>
            </div>

            <div class="field col-12 md:col-8">
              <label for="logradouro">Logradouro</label>
              <input
                pInputText
                id="logradouro"
                formControlName="logradouro"
                class="w-full"
                maxlength="200"
              />
            </div>

            <div class="field col-12 md:col-2">
              <label for="numero">Número</label>
              <input
                pInputText
                id="numero"
                formControlName="numero"
                class="w-full"
                maxlength="20"
              />
            </div>

            <div class="field col-12 md:col-4">
              <label for="complemento">Complemento</label>
              <input
                pInputText
                id="complemento"
                formControlName="complemento"
                class="w-full"
                maxlength="100"
              />
            </div>

            <div class="field col-12 md:col-6">
              <label for="bairro">Bairro</label>
              <input
                pInputText
                id="bairro"
                formControlName="bairro"
                class="w-full"
                maxlength="100"
              />
            </div>

            <div class="field col-12 md:col-8">
              <label for="cidade">Cidade</label>
              <input
                pInputText
                id="cidade"
                formControlName="cidade"
                class="w-full"
                maxlength="100"
              />
            </div>

            <div class="field col-12 md:col-4">
              <label for="uf">UF</label>
              <input
                pInputText
                id="uf"
                formControlName="uf"
                class="w-full"
                maxlength="2"
                style="text-transform: uppercase"
              />
            </div>
          </div>
        </ng-container>
      </p-card>

      <div class="flex gap-2 justify-content-end">
        <p-button
          label="Cancelar"
          severity="secondary"
          routerLink="/pessoas-fisicas"
        />
        <p-button
          label="Salvar"
          type="submit"
          [loading]="salvando"
          icon="pi pi-check"
        />
      </div>
    </form>
  `,
})
export class PessoaFisicaFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly service = inject(PessoaFisicaService);
  private readonly enderecoService = inject(EnderecoService);
  private readonly messageService = inject(MessageService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  form!: FormGroup;
  loading = false;
  salvando = false;
  buscandoCep = false;
  isEdit = false;
  hoje = new Date();
  private id?: string;

  ngOnInit(): void {
    this.buildForm();
    this.id = this.route.snapshot.params["id"];
    this.isEdit = !!this.id;

    if (this.isEdit) {
      this.loading = true;
      this.service.obterPorId(this.id!).subscribe({
        next: (p) => {
          this.form.patchValue({
            ...p,
            dataNascimento: new Date(p.dataNascimento),
          });
          this.loading = false;
        },
        error: () => (this.loading = false),
      });
    }
  }

  buildForm(): void {
    this.form = this.fb.group({
      nome: ["", [Validators.required, Validators.maxLength(200)]],
      email: [
        "",
        [Validators.required, Validators.email, Validators.maxLength(254)],
      ],
      telefone: ["", [Validators.required]],
      cpf: ["", [Validators.required]],
      dataNascimento: [null, [Validators.required]],
      rg: [""],
      endereco: this.fb.group({
        cep: [""],
        logradouro: [""],
        numero: [""],
        complemento: [""],
        bairro: [""],
        cidade: [""],
        uf: [""],
      }),
    });
  }

  isInvalid(field: string): boolean {
    const control = this.form.get(field);
    return !!(control?.invalid && (control.dirty || control.touched));
  }

  aplicarMascara(
    tipo: "cpf" | "cnpj" | "telefone" | "cep",
    campo: string,
  ): void {
    const control = this.form.get(campo);
    if (!control) return;
    const digits = (control.value ?? "").replaceAll(/\D/g, "");
    let formatted = digits;

    switch (tipo) {
      case "cpf":
        formatted = digits
          .slice(0, 11)
          .replace(/(\d{3})(\d)/, "$1.$2")
          .replace(/(\d{3})(\d)/, "$1.$2")
          .replace(/(\d{3})(\d{1,2})$/, "$1-$2");
        break;
      case "telefone": {
        const t = digits.slice(0, 11);
        formatted =
          t.length <= 10
            ? t.replace(/(\d{2})(\d{4})(\d{0,4})/, "($1) $2-$3")
            : t.replace(/(\d{2})(\d{5})(\d{0,4})/, "($1) $2-$3");
        break;
      }
      case "cep":
        formatted = digits.slice(0, 8).replace(/(\d{5})(\d{0,3})/, "$1-$2");
        break;
    }

    control.setValue(formatted, { emitEvent: false });
  }

  buscarCep(): void {
    const cep = this.form.get("endereco.cep")?.value ?? "";
    const digits = cep.replaceAll(/\D/g, "");
    if (digits.length !== 8) return;

    this.buscandoCep = true;
    this.enderecoService.consultarCep(digits).subscribe({
      next: (res) => {
        this.form.get("endereco")?.patchValue({
          logradouro: res.logradouro ?? "",
          bairro: res.bairro ?? "",
          cidade: res.cidade ?? "",
          uf: res.uf ?? "",
        });
        this.buscandoCep = false;
      },
      error: () => (this.buscandoCep = false),
    });
  }

  salvar(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const f = this.form.getRawValue();
    if (!f.dataNascimento) return;

const cepDigits = ((f.endereco?.cep) ?? '').replaceAll(/\D/g, '');
    const temEndereco = cepDigits.length === 8;

    const payload: any = {
      nome: f.nome,
      email: f.email,
      telefone: (f.telefone ?? "").replaceAll(/\D/g, ""),
      cpf: (f.cpf ?? "").replaceAll(/\D/g, ""),
      dataNascimento: (f.dataNascimento as Date).toISOString(),
      rg: f.rg || null,
      endereco: temEndereco
        ? {
            cep: cepDigits,
            logradouro: f.endereco.logradouro || null,
            numero: f.endereco.numero || null,
            complemento: f.endereco.complemento || null,
            bairro: f.endereco.bairro || null,
            cidade: f.endereco.cidade || null,
            uf: f.endereco.uf || null,
          }
        : null,
    };

    this.salvando = true;
    const req$ = this.isEdit
      ? this.service.atualizar(this.id!, payload as any)
      : this.service.criar(payload as any);

    req$.subscribe({
      next: () => {
        this.messageService.add({
          severity: "success",
          summary: "Sucesso",
          detail: `Pessoa física ${this.isEdit ? "atualizada" : "criada"} com sucesso.`,
        });
        this.router.navigate(["/pessoas-fisicas"]);
      },
      error: () => (this.salvando = false),
    });
  }
}
