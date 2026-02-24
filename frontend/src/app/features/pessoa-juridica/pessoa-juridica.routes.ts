import { Routes } from '@angular/router';

// Lazy Loading das rotas de Pessoa Jurídica
export const PESSOA_JURIDICA_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/list/pessoa-juridica-list.component').then((m) => m.PessoaJuridicaListComponent),
  },
  {
    path: 'novo',
    loadComponent: () =>
      import('./pages/form/pessoa-juridica-form.component').then((m) => m.PessoaJuridicaFormComponent),
  },
  {
    path: 'editar/:id',
    loadComponent: () =>
      import('./pages/form/pessoa-juridica-form.component').then((m) => m.PessoaJuridicaFormComponent),
  },
];
