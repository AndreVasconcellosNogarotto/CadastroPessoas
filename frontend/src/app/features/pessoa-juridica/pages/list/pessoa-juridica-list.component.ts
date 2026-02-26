import { Component, OnInit, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { TagModule } from 'primeng/tag';
import { ConfirmationService, MessageService } from 'primeng/api';
import { PessoaJuridicaService } from '../../../../core/services/services';
import { PessoaJuridica } from '../../../../core/models/models';
import { MaskPipe } from 'src/app/core/pipes/mask.pipe';

@Component({
  selector: 'app-pessoa-juridica-list',
  standalone: true,
  imports: [CommonModule, RouterLink, TableModule, ButtonModule, ConfirmDialogModule, TagModule,MaskPipe],
  providers: [ConfirmationService],
  template: `
    <p-confirmDialog />

    <div class="flex justify-content-between align-items-center mb-3">
      <h2 class="m-0">Pessoas Jurídicas</h2>
      <p-button label="Nova Pessoa Jurídica" icon="pi pi-plus" routerLink="novo" />
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
          <th>Razão Social</th>
          <th>Nome Fantasia</th>
          <th>CNPJ</th>
          <th>E-mail</th>
          <th>Status</th>
          <th style="width: 130px">Ações</th>
        </tr>
      </ng-template>

      <ng-template pTemplate="body" let-pj>
        <tr>
          <td>{{ pj.razaoSocial }}</td>
          <td>{{ pj.nomeFantasia }}</td>
          <td>{{ pj.cnpj | mask:'cnpj'}}</td>
          <td>{{ pj.email }}</td>
          <td>
            <p-tag
              [value]="pj.ativo ? 'Ativo' : 'Inativo'"
              [severity]="pj.ativo ? 'success' : 'danger'" />
          </td>
          <td>
            <div class="flex gap-2">
              <p-button icon="pi pi-pencil" [routerLink]="['editar', pj.id]"
                severity="secondary" size="small" [text]="true" pTooltip="Editar" />
              <p-button icon="pi pi-trash" severity="danger" size="small" [text]="true"
                pTooltip="Excluir" (onClick)="confirmarExclusao(pj)" />
            </div>
          </td>
        </tr>
      </ng-template>

      <ng-template pTemplate="emptymessage">
        <tr>
          <td colspan="6" class="text-center p-4 text-color-secondary">
            <i class="pi pi-inbox text-4xl block mb-2"></i>
            Nenhum registro encontrado.
          </td>
        </tr>
      </ng-template>
    </p-table>
  `,
})
export class PessoaJuridicaListComponent implements OnInit {
  private readonly service = inject(PessoaJuridicaService);
  private readonly confirmationService = inject(ConfirmationService);
  private readonly messageService = inject(MessageService);

  pessoas: PessoaJuridica[] = [];
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

  confirmarExclusao(pj: PessoaJuridica): void {
    this.confirmationService.confirm({
      message: `Deseja excluir <strong>${pj.razaoSocial}</strong>?`,
      header: 'Confirmar Exclusão',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sim, excluir',
      rejectLabel: 'Cancelar',
      acceptButtonStyleClass: 'p-button-danger',
      accept: () => this.excluir(pj.id),
    });
  }

  excluir(id: string): void {
    this.service.deletar(id).subscribe({
      next: () => {
        this.messageService.add({
          severity: 'success',
          summary: 'Sucesso',
          detail: 'Pessoa jurídica excluída com sucesso.',
        });
        this.carregar();
      },
    });
  }
}
