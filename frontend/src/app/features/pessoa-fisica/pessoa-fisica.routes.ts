import { Routes } from '@angular/router';

// Lazy Loading das rotas de Pessoa Física
export const PESSOA_FISICA_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/list/pessoa-fisica-list.component').then((m) => m.PessoaFisicaListComponent),
  },
  {
    path: 'novo',
    loadComponent: () =>
      import('./pages/form/pessoa-fisica-form.component').then((m) => m.PessoaFisicaFormComponent),
  },
  {
    path: 'editar/:id',
    loadComponent: () =>
      import('./pages/form/pessoa-fisica-form.component').then((m) => m.PessoaFisicaFormComponent),
  },
];
