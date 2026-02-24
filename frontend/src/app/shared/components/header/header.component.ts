import { Component } from '@angular/core';
import { ToolbarModule } from 'primeng/toolbar';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [ToolbarModule, ButtonModule],
  template: `
    <p-toolbar styleClass="header-toolbar">
      <ng-template pTemplate="start">
        <div class="flex align-items-center gap-2">
          <i class="pi pi-users text-2xl text-primary"></i>
          <span class="text-xl font-bold">Cadastro de Pessoas</span>
        </div>
      </ng-template>
      <ng-template pTemplate="end">
        <span class="text-sm text-color-secondary">Cadastro de Pessoas Físicas e Jurídicas</span>
      </ng-template>
    </p-toolbar>
  `,
  styles: [`
    :host { display: block; }
    ::ng-deep .header-toolbar {
      border-radius: 0;
      border-left: none;
      border-right: none;
      border-top: none;
    }
  `]
})
export class HeaderComponent {}
