import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'pessoas-fisicas',
    pathMatch: 'full',
  },
  {
    // Lazy Loading - módulo de Pessoa Física carregado sob demanda
    path: 'pessoas-fisicas',
    loadChildren: () =>
      import('./features/pessoa-fisica/pessoa-fisica.routes').then((m) => m.PESSOA_FISICA_ROUTES),
  },
  {
    // Lazy Loading - módulo de Pessoa Jurídica carregado sob demanda
    path: 'pessoas-juridicas',
    loadChildren: () =>
      import('./features/pessoa-juridica/pessoa-juridica.routes').then((m) => m.PESSOA_JURIDICA_ROUTES),
  },
  {
    path: '**',
    redirectTo: 'pessoas-fisicas',
  },
];
