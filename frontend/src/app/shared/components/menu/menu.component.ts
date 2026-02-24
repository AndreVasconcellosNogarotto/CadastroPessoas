import { Component } from '@angular/core';
import { PanelMenuModule } from 'primeng/panelmenu';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-menu',
  standalone: true,
  imports: [PanelMenuModule],
  template: `
    <nav class="layout-sidebar surface-50 border-right-1 surface-border p-2" style="min-width: 220px;">
      <p-panelMenu [model]="menuItems" styleClass="w-full border-none" />
    </nav>
  `,
  styles: [`
    :host { display: block; }
    ::ng-deep .p-panelmenu .p-panelmenu-header-link {
      border-radius: 6px;
    }
  `]
})
export class MenuComponent {
  menuItems: MenuItem[] = [
    {
      label: 'Cadastros',
      icon: 'pi pi-fw pi-database',
      expanded: true,
      items: [
        {
          label: 'Pessoa Física',
          icon: 'pi pi-fw pi-user',
          routerLink: ['/pessoas-fisicas'],
        },
        {
          label: 'Pessoa Jurídica',
          icon: 'pi pi-fw pi-building',
          routerLink: ['/pessoas-juridicas'],
        },
      ],
    },
  ];
}
