import { Component } from '@angular/core';

@Component({
  selector: 'app-footer',
  standalone: true,
  template: `
    <footer class="surface-100 border-top-1 surface-border text-center p-3">
      <span class="text-sm text-color-secondary">
        © {{ currentYear }} Cadastro de Pessoas
      </span>
    </footer>
  `,
})
export class FooterComponent {
  currentYear = new Date().getFullYear();
}
