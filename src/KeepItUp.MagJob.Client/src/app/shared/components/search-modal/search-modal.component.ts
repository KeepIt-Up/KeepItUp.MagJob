import { Component, input, output } from '@angular/core';
import { State } from '@shared/services/state.service';
import { PaginationOptions } from '../pagination/pagination.component';
import { InfiniteListComponent } from '../infinite-list/infinite-list.component';
import { ModalComponent } from '../modal/modal.component';
import { SearchInputComponent } from '../search-input/search-input.component';
import { NgClass, NgTemplateOutlet } from '@angular/common';
import { ButtonComponent } from '../button/button.component';

@Component({
  selector: 'app-search-modal',
  imports: [
    ModalComponent,
    InfiniteListComponent,
    SearchInputComponent,
    ButtonComponent,
    NgTemplateOutlet,
  ],
  template: `
    <app-modal [isOpen]="isOpen()" [title]="title()" (onCloseClick)="close.emit($event)">
      <div class="p-4 md:p-5">
        <!-- Move search input to separate ng-template -->
        <ng-container *ngTemplateOutlet="searchSection"></ng-container>

        <!-- Move list to separate ng-template -->
        <app-infinite-list
          [paginationOptions$]="paginationOptions$()"
          [state$]="state$()"
          (onLoad)="onLoad()"
        >
          <ng-container *ngTemplateOutlet="itemsList"></ng-container>
        </app-infinite-list>
      </div>
    </app-modal>

    <!-- Search section template -->
    <ng-template #searchSection>
      <div class="mb-4">
        <app-search-input
          [placeholder]="searchPlaceholder()"
          (valueChange)="onSearch($event)"
        ></app-search-input>
      </div>
    </ng-template>

    <!-- Items list template -->
    <ng-template #itemsList>
      <div class="space-y-2 max-h-60 overflow-y-auto">
        @for (item of state$().data; track item.id) {
          <div
            class="flex items-center justify-between p-3 rounded-lg border border-gray-200 dark:border-gray-700 bg-white dark:bg-gray-800"
          >
            <div class="flex items-center overflow-hidden">
              <div class="ml-2 overflow-hidden">
                <p class="text-sm font-medium text-gray-900 dark:text-white truncate">
                  {{ displayFn()(item) }}
                </p>
              </div>
            </div>
            <app-button
              [size]="'sm'"
              [color]="isSelected(item) ? 'danger' : 'primary'"
              (clicked)="selectItem(item)"
            >
              {{ isSelected(item) ? 'Remove' : 'Add' }}
            </app-button>
          </div>
        } @empty {
          <div
            class="p-3 text-sm text-center rounded-lg text-gray-600 dark:text-gray-400 bg-gray-50 dark:bg-gray-800"
          >
            No results found
          </div>
        }
      </div>
    </ng-template>
  `,
})
export class SearchModalComponent<T extends { id: string }> {
  isOpen = input.required<boolean>();
  paginationOptions$ = input.required<PaginationOptions<T>>();
  state$ = input.required<State<T[], { endOfData: boolean }>>();

  title = input<string>('');
  searchPlaceholder = input<string>('');
  selectedItems = input<T[]>([]);
  displayFn = input<(item: T) => string>((item: T) => item.id || 'Unknown');
  trackBy = input<(item: T) => any>();
  compareFn = input<(a: T, b: T) => boolean>((a: T, b: T) => a.id === b.id);

  close = output<void>();
  search = output<string>();
  selectionChange = output<T>();
  loadMore = output<void>();

  onSearch(query: string): void {
    this.search.emit(query);
  }

  selectItem(item: T): void {
    this.selectionChange.emit(item);
  }

  isSelected(item: T): boolean {
    return this.selectedItems().some(selected => this.compareFn()(selected, item));
  }

  onLoad(): void {
    this.loadMore.emit();
  }
}
