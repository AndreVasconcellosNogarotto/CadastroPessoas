import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent } from './shared/components/header/header.component';
import { FooterComponent } from './shared/components/footer/footer.component';
import { MenuComponent } from './shared/components/menu/menu.component';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, FooterComponent, MenuComponent, ToastModule],
  providers: [],
  template: `
    <p-toast position="top-right" />
    <app-header />
    <div class="layout-wrapper">
      <app-menu />
      <main class="layout-main">
        <router-outlet />
      </main>
    </div>
    <app-footer />
  `,
  styles: [`
    .layout-wrapper {
      display: flex;
      min-height: calc(100vh - 60px - 50px);
    }
    .layout-main {
      flex: 1;
      padding: 1.5rem;
      overflow: auto;
    }
  `]
})
export class AppComponent {}
