import { Component, input, output } from '@angular/core';
import { State } from '@shared/services/state.service';
import { PaginationOptions } from '../pagination/pagination.component';
import { InfiniteListComponent } from '../infinite-list/infinite-list.component';
import { ModalComponent } from '../modal/modal.component';
import { SearchInputComponent } from '../search-input/search-input.component';
import { NgClass } from '@angular/common';


@Component({
  selector: 'app-search-modal',
  imports: [ModalComponent, InfiniteListComponent, SearchInputComponent, NgClass],
  template: `
    <app-modal [isOpen]="isOpen()" [title]="title()" (onCloseClick)="close.emit($event)">
      <div class="p-4 md:p-5">
        <!-- Move search input to separate ng-template -->
        <ng-container *ngTemplateOutlet="searchSection"></ng-container>
        
        <!-- Move list to separate ng-template -->
        <app-infinite-list [paginationOptions$]="paginationOptions$()" [state$]="state$()" (onLoad)="onLoad()">
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
          <div class="flex items-center justify-between p-3" [ngClass]="itemContainerClass">
            <div class="flex items-center">
              <div class="ml-3">
                <p class="text-sm font-medium" [ngClass]="textClass">
                  {{ displayFn()(item) }}
                </p>
              </div>
            </div>
            <button 
              (click)="selectItem(item)"
              class="px-4 py-2 text-sm font-medium rounded-lg"
              [ngClass]="getButtonClasses(item)"
            >
              {{ isSelected(item) ? 'Remove' : 'Add' }}
            </button>
          </div>
        }
        @empty {
          <div class="p-3 text-sm text-center rounded-lg" [ngClass]="emptyStateClass">
            No results found
          </div>
        }
      </div>
    </ng-template>
  `
})
export class SearchModalComponent<T extends { id: string }> {
  isOpen = input.required<boolean>();
  paginationOptions$ = input.required<PaginationOptions<T>>();
  state$ = input.required<State<T[], { endOfData: boolean }>>();

  title = input<string>("");
  searchPlaceholder = input<string>("");
  selectedItems = input<T[]>([]);
  displayFn = input<(item: T) => string>((item: T) => item.toString());
  trackBy = input<(item: T) => any>();
  compareFn = input<(a: T, b: T) => boolean>((a: T, b: T) => a.id === b.id);

  close = output<void>();
  search = output<string>();
  selectionChange = output<T>();
  loadMore = output<void>();

  // Add computed classes
  protected readonly itemContainerClass = 'bg-gray-50 dark:bg-gray-700 rounded-lg';
  protected readonly textClass = 'text-gray-900 dark:text-white';
  protected readonly emptyStateClass = 'text-gray-600 dark:text-gray-400 bg-gray-50 dark:bg-gray-800';

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

  protected getButtonClasses(item: T): string {
    const isItemSelected = this.isSelected(item);
    return isItemSelected
      ? 'bg-blue-600 hover:bg-blue-700 text-white'
      : 'bg-gray-200 hover:bg-gray-300';
  }
} 