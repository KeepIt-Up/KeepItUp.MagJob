import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

export interface PaginationOptions<T extends { id: string }> {
  pageNumber: number;
  pageSize: number;
  sortField?: keyof T;
  ascending: boolean;
}

export interface PaginatedResponse<T extends { id: string }> {
  items: T[];
  pageNumber: number;
  totalPages: number;
  totalCount: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export function serializePaginationOptions<T extends { id: string }>(paginationOptions: PaginationOptions<T>): Record<string, any> {
  return {
    'options.pageNumber': paginationOptions.pageNumber,
    'options.pageSize': paginationOptions.pageSize,
    'options.sortField': paginationOptions.sortField,
    'options.ascending': paginationOptions.ascending
  }
}

@Component({
  selector: 'app-pagination',
  imports: [CommonModule, FormsModule],
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.scss']
})
export class PaginationComponent<T extends { id: string }> implements OnInit {
  @Input() items: T[] = [];
  @Input() defaultPageSize: number = 10;
  @Input() currentPage: number = 1;
  @Input() totalItems: number = 0;
  @Input() showPageSizeSelector: boolean = false;
  @Input() showStartEnd: boolean = false;
  @Input() buttonsCount: number = 5;

  @Output() pageChange = new EventEmitter<number>();
  @Output() pageSizeChange = new EventEmitter<number>();

  pageSize: number = 10;
  pagesNumbers: number[] = [];

  ngOnInit(): void {
    this.pageSize = this.defaultPageSize;
    this.pagesNumbers = this.getSequence(
      this.currentPage,
      this.totalPages,
      this.buttonsCount
    );
  }

  get totalPages(): number {
    return Math.ceil(this.totalItems / this.pageSize);
  }

  get paginatedItems(): T[] {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    return this.items.slice(startIndex, startIndex + this.pageSize);
  }

  onPageChange(page: number): void {
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.pageChange.emit(page);
    }
    this.pagesNumbers = this.getSequence(
      this.currentPage,
      this.totalPages,
      this.buttonsCount
    );
  }

  onPageSizeChange(newPageSize: number | string): void {
    this.pageSize = Number(newPageSize);
    this.currentPage = 1;
    this.pageSizeChange.emit(this.pageSize);
    this.pageChange.emit(this.currentPage);
  }

  /**
   * Generates a sequence of numbers centered around the current point,
   * except at the ends of the sequence where it adjusts to ensure the current point is included.
   * @param current The current point to center the sequence around.
   * @param max The maximum number in the sequence.
   * @param length The desired length of the sequence.
   * @returns An array of consecutive numbers.
   */
  private getSequence(current: number, max: number, length: number): number[] {
    const sequenceLength = Math.min(max, length);
    const start = Math.max(1, Math.min(current - Math.floor(sequenceLength / 2), max - sequenceLength + 1));
    return Array.from({ length: sequenceLength }, (_, i) => start + i);
  }
}
