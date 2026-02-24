import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'mask', standalone: true })
export class MaskPipe implements PipeTransform {
  transform(value: string | null | undefined, type: 'cpf' | 'cnpj' | 'telefone' | 'cep'): string {
    if (!value) return '';
    const v = value.replaceAll(/\D/g, '');
    switch (type) {
      case 'cpf':
        return v.replace(/(\d{3})(\d{3})(\d{3})(\d{2})/, '$1.$2.$3-$4');
      case 'cnpj':
        return v.replace(/(\d{2})(\d{3})(\d{3})(\d{4})(\d{2})/, '$1.$2.$3/$4-$5');
      case 'telefone':
        return v.length === 11
          ? v.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3')
          : v.replace(/(\d{2})(\d{4})(\d{4})/, '($1) $2-$3');
      case 'cep':
        return v.replace(/(\d{5})(\d{3})/, '$1-$2');
      default:
        return value;
    }
  }
}