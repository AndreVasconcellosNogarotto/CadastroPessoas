import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TagModule } from 'primeng/tag';
import { ConfirmationService, MessageService } from 'primeng/api';
import { PessoaFisicaService } from '../../../../core/services/services';
import { PessoaFisica } from '../../../../core/models/models';
import { MaskPipe } from 'src/app/core/pipes/mask.pipe';

@Component({
  selector: 'app-pessoa-fisica-list',
  standalone: true,
  imports: [CommonModule, RouterLink, TableModule, ButtonModule, ConfirmDialogModule, TagModule,MaskPipe],
  providers: [ConfirmationService],
  template: `
    <p-confirmDialog />

    <div class="flex justify-content-between align-items-center mb-3">
      <h2 class="m-0">Pessoas Físicas</h2>
      <p-button label="Nova Pessoa Física" icon="pi pi-plus" routerLink="novo" />
    </div>

    <p-table
      [value]="pessoas"
      [paginator]="true"
      [rows]="pageSize"
      [totalRecords]="total"
      [lazy]="true"
      (onLazyLoad)="onLazyLoad($event)"
      [loading]="loading"
      [rowHover]="true"
      styleClass="p-datatable-gridlines"
      responsiveLayout="scroll">

      <ng-template pTemplate="header">
        <tr>
          <th>Nome</th>
          <th>CPF</th>
          <th>E-mail</th>
          <th>Telefone</th>
          <th>Idade</th>
          <th>Status</th>
          <th style="width: 130px">Ações</th>
        </tr>
      </ng-template>

      <ng-template pTemplate="body" let-pessoa>
        <tr>
          <td>{{ pessoa.nome }}</td>
          <td>{{ pessoa.cpf | mask:'cpf'}}</td>
          <td>{{ pessoa.email }}</td>
          <td>{{ pessoa.telefone | mask:'telefone' }}</td>
          <td>{{ pessoa.idade }} anos</td>
          <td>
            <p-tag
              [value]="pessoa.ativo ? 'Ativo' : 'Inativo'"
              [severity]="pessoa.ativo ? 'success' : 'danger'" />
          </td>
          <td>
            <div class="flex gap-2">
              <p-button icon="pi pi-pencil" [routerLink]="['editar', pessoa.id]"
                severity="secondary" size="small" [text]="true" pTooltip="Editar" />
              <p-button icon="pi pi-trash" severity="danger" size="small" [text]="true"
                pTooltip="Excluir" (onClick)="confirmarExclusao(pessoa)" />
            </div>
          </td>
        </tr>
      </ng-template>

      <ng-template pTemplate="emptymessage">
        <tr>
          <td colspan="7" class="text-center p-4 text-color-secondary">
            <i class="pi pi-inbox text-4xl block mb-2"></i>
            Nenhum registro encontrado.
          </td>
        </tr>
      </ng-template>
    </p-table>
  `,
})
export class PessoaFisicaListComponent implements OnInit {
  private readonly service = inject(PessoaFisicaService);
  private readonly confirmationService = inject(ConfirmationService);
  private readonly messageService = inject(MessageService);

  pessoas: PessoaFisica[] = [];
  loading = false;
  total = 0;
  page = 1;
  pageSize = 10;

  ngOnInit(): void {
    this.carregar();
  }

  carregar(): void {
    this.loading = true;
    this.service.listar(this.page, this.pageSize).subscribe({
      next: (res) => {
        this.pessoas = res.data;
        this.total = res.total;
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }

  onLazyLoad(event: any): void {
    this.page = Math.floor(event.first / event.rows) + 1;
    this.pageSize = event.rows;
    this.carregar();
  }

  confirmarExclusao(pessoa: PessoaFisica): void {
    this.confirmationService.confirm({
      message: `Deseja excluir <strong>${pessoa.nome}</strong>?`,
      header: 'Confirmar Exclusão',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sim, excluir',
      rejectLabel: 'Cancelar',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => this.excluir(pessoa.id),
    });
  }

  excluir(id: string): void {
    this.service.deletar(id).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Sucesso',
          detail: 'Pessoa física excluída com sucesso.',
        });
        this.carregar();
      },
    });
  }
}
