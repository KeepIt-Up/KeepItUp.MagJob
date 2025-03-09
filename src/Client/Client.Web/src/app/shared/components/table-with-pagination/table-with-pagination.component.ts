import { Component, input, OnInit, output } from '@angular/core';
import { StatefullContainerComponent } from '../statefull-container/statefull-container.component';
import { State } from '@shared/services/state.service';
import { PaginatedResponse, PaginationComponent, PaginationOptions } from '../pagination/pagination.component';
import { ColumnDefinition, SortBy, TableComponent } from '../table/table.component';

@Component({
  selector: 'app-table-with-pagination',
  imports: [StatefullContainerComponent, TableComponent, PaginationComponent],
  templateUrl: 'table-with-pagination.component.html'
})
export class TableWithPaginationComponent<T extends { id: string }> implements OnInit {
  columnsConfig = input.required<ColumnDefinition<T>[]>();
  state = input.required<State<PaginatedResponse<T>>>();
  paginationOptions = input.required<PaginationOptions<T>>();
  onGetData = output<void>();

  sortBy: SortBy<T> = {
    modelProp: "id",
    ascending: true
  };

  ngOnInit(): void {
    this.update();
  }

  private update() {
    this.onGetData.emit();
  }

  // Pagination events

  onPageChange(pageNumber: number) {
    this.paginationOptions().pageNumber = pageNumber;
    this.update();
  }

  onSortByChange(sortBy: SortBy<T>) {
    this.paginationOptions().sortField = sortBy.modelProp;
    this.paginationOptions().ascending = sortBy.ascending;
    this.update();
  }

  onPageSizeChange(pageSize: number) {
    this.paginationOptions().pageSize = pageSize
    this.update();
  }
}
