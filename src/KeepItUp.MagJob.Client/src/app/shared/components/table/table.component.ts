import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, Type } from '@angular/core';
import { DynamicPipe } from '../../pipes/dynamic.pipe';
import { NgComponentOutlet } from '@angular/common';

export type ComponentCellDefinition<T> = {
  type: Type<any>;
  inputs?: (row: T) => Record<string, any>;
};

export type ColumnDefinition<T extends Record<any, any>> = {
  title: string;
  modelProp: keyof T;
  isSortable?: boolean;
  computeValue?: (row: T) => string;
  component?: ComponentCellDefinition<T>;
  pipe?: Type<any>;
  pipeArgs?: any[];
};

export type SortBy<T> = {
  modelProp?: keyof T;
  ascending: boolean;
}

@Component({
    selector: 'app-table',
    imports: [DynamicPipe, NgComponentOutlet],
    templateUrl: './table.component.html'
})
export class TableComponent<T extends { id: K }, K = string> implements OnChanges {
  @Input({ required: true }) rows!: T[];
  @Input({ required: true }) columnsConfig!: ColumnDefinition<T>[];
  @Input() showCheckboxes = false;
  @Output() sortBy = new EventEmitter<SortBy<T>>();

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['rows']) {
      this.selectedRows.clear();
    }
  }

  selectedRows = new Set<K>();

  toggleAllRows(checked: boolean) {
    if (checked) {
      this.rows.forEach(row => this.selectedRows.add(row.id));
    } else {
      this.selectedRows.clear();
    }
  }

  isRowSelected(id: K): boolean {
    return this.selectedRows.has(id);
  }

  isAllRowsSelected(): boolean {
    return this.selectedRows.size === this.rows.length;
  }

  toggleRow(id: K, checked: boolean) {
    if (checked) {
      this.selectedRows.add(id);
    } else {
      this.selectedRows.delete(id);
    }
    if (this.selectedRows.size === this.rows.length) {
      this.toggleAllRows(true);
    }
  }

  _sortBy: SortBy<T> = {
    ascending: true,
    modelProp: "id"
  }

  updateSortBy(modelProp: keyof T) {
    if (modelProp === this._sortBy.modelProp) {
      this._sortBy.ascending = !this._sortBy.ascending;
    }
    else {
      this._sortBy = {
        ascending: true,
        modelProp: modelProp
      };
    }

    this.sortBy.emit(this._sortBy);
  }
}
