import {
  Component,
  ElementRef,
  OnInit,
  TemplateRef,
  ViewChild,
  afterNextRender,
  input,
  model,
  output,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { SkeletonModule } from 'primeng/skeleton';
import { ScrollPanelModule } from 'primeng/scrollpanel';
import { PaginationParameters } from '../../models/pagination-parameters.model';
import { InfinityListConfig } from './models/infinity-list-config.model';

@Component({
  selector: 'app-infinity-list',
  standalone: true,
  imports: [CommonModule, ButtonModule, SkeletonModule, ScrollPanelModule],
  templateUrl: './infinity-list.component.html',
  styleUrls: ['./infinity-list.component.scss'],
})
export class InfinityListComponent<T, P extends PaginationParameters = PaginationParameters>
  implements OnInit
{
  @ViewChild('scrollContainer') scrollContainer!: ElementRef<HTMLDivElement>;
  @ViewChild('scrollPanel') scrollPanel: any;

  config = model.required<InfinityListConfig<T, P>>();
  columnTemplates = input<{ [key: string]: TemplateRef<any> }>({});
  itemSelect = output<T>();

  // Scroll handling
  private scrollThreshold = 200; // pixels from bottom to trigger loading more

  constructor() {
    afterNextRender(() => {
      this.setupScrollListener();
    });
  }

  ngOnInit(): void {
    if (!this.config().emptyMessage) {
      this.config().emptyMessage = 'No items available';
    }
    if (this.config().showLoadingIndicator === undefined) {
      this.config().showLoadingIndicator = true;
    }
  }

  /**
   * Set up scroll event listener
   */
  private setupScrollListener(): void {
    // For PrimeNG ScrollPanel, listen to the scroll event of its internal container
    if (this.scrollPanel && this.scrollPanel.el) {
      const scrollableContainer =
        this.scrollPanel.el.nativeElement.querySelector('.p-scrollpanel-content');
      if (scrollableContainer) {
        scrollableContainer.addEventListener('scroll', () => {
          if (this.shouldLoadMore(scrollableContainer)) {
            this.loadMoreItems();
          }
        });
      }
    }
  }

  /**
   * Check if more items should be loaded
   */
  private shouldLoadMore(scrollElement: HTMLElement): boolean {
    if (this.config().state().isLoading || !this.config().state().data?.hasNext) {
      return false;
    }

    const { scrollTop, scrollHeight, clientHeight } = scrollElement;
    const scrollPosition = scrollTop + clientHeight;
    const scrollThresholdPosition = scrollHeight - this.scrollThreshold;

    return scrollPosition >= scrollThresholdPosition;
  }

  /**
   * Load more items
   */
  loadMoreItems(): void {
    if (this.config().state().isLoading || !this.config().state().data?.hasNext) {
      return;
    }

    // Increment page for next batch
    this.config().params.pageNumber = (this.config().params.pageNumber || 1) + 1;

    // Get more data from the data source
    this.config().dataSource(this.config().params).subscribe();
  }

  /**
   * Handle item selection
   */
  handleItemSelect(item: T): void {
    this.itemSelect.emit(item);
  }

  /**
   * Force reload data
   */
  refresh(): void {
    this.config().params.pageNumber = 1;
    this.config().dataSource(this.config().params).subscribe();
  }

  /**
   * Track items by identifier function for ngFor optimization
   */
  trackByFn(index: number, item: any): any {
    return item.id || index;
  }
}
