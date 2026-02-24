import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { MessageService } from 'primeng/api';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const messageService = inject(MessageService);

  return next(req).pipe(
    catchError((error) => {
      let detail = 'Ocorreu um erro inesperado.';

      if (error.error?.message) {
        detail = error.error.message;
      } else if (error.message) {
        detail = error.message;
      }

      const summaryMap: Record<number, string> = {
        400: 'Requisição inválida',
        404: 'Não encontrado',
        409: 'Conflito',
        500: 'Erro no servidor',
      };

      const summary = summaryMap[error.status] ?? 'Erro';

      messageService.add({ severity: 'error', summary, detail, life: 5000 });

      return throwError(() => error);
    })
  );
};
