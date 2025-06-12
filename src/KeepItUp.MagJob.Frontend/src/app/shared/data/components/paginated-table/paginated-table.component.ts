import {
  Component,
  DestroyRef,
  OnInit,
  TemplateRef,
  inject,
  input,
  model,
  output,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { PaginatorModule, PaginatorState } from 'primeng/paginator';
import { SortMeta } from 'primeng/api';
import { PaginatedTableConfig } from './models/paginated-table-config.model';
import { PaginationParameters } from '../../models/pagination-parameters.model';
import { DataGridColumn } from './models/data-grid-column.model';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { SkeletonModule } from 'primeng/skeleton';

@Component({
  selector: 'app-paginated-table',
  standalone: true,
  imports: [CommonModule, TableModule, ButtonModule, PaginatorModule, SkeletonModule],
  templateUrl: './paginated-table.component.html',
  styleUrls: ['./paginated-table.component.scss'],
})
export class PaginatedTableComponent<T, P extends PaginationParameters = PaginationParameters>
  implements OnInit
{
  config = model.required<PaginatedTableConfig<T, P>>();
  columnTemplates = input.required<{ [key: string]: TemplateRef<any> }>();
  rowSelect = output<T>();

  private readonly destroyRef = inject(DestroyRef);

  ngOnInit(): void {
    this.config()
      .dataSource(this.config().params)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe();
  }

  /**
   * Handle row selection
   */
  handleRowSelect(event: any): void {
    this.rowSelect.emit(event.data);
  }

  /**
   * Handle sort changes
   */
  onSortChange(event: SortMeta): void {
    if (!event.field) {
      return;
    }

    const order = event.order === 1 ? true : false;
    const isNewOrder =
      order !== this.config().params.ascending || event.field !== this.config().params.sortField;

    this.config().params.sortField = event.field;
    this.config().params.ascending = order;

    if (isNewOrder) {
      this.config()
        .dataSource(this.config().params)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe();
    }
  }

  /**
   * Handle page changes
   */
  onPageChange(event: PaginatorState): void {
    console.log(event);
    this.config().params.pageSize = event.rows ?? 10;
    this.config().params.pageNumber = (event.first ?? 0) / (event.rows ?? 10) + 1;

    this.config()
      .dataSource(this.config().params)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe();
  }

  /**
   * Get cell value with formatting
   */
  getCellValue(row: T, column: DataGridColumn<T>): string {
    if (!column.field) {
      return '';
    }

    const value = row[column.field];

    if (column.formatter) {
      return column.formatter(value, row);
    }

    return value !== undefined && value !== null ? value.toString() : '';
  }
}
