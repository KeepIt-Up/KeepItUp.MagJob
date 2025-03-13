import { Component, input, OnInit, output } from '@angular/core';
import { StatefullContainerComponent } from '../statefull-container/statefull-container.component';
import { PaginationOptions } from '../pagination/pagination.component';
import { InfiniteScrollComponent } from '../infinite-scroll/infinite-scroll.component';
import { State } from '@shared/services/state.service';

@Component({
  selector: 'app-infinite-list',
  imports: [StatefullContainerComponent, InfiniteScrollComponent],
  templateUrl: './infinite-list.component.html'
})
export class InfiniteListComponent<T extends { id: string }> implements OnInit {
  paginationOptions$ = input.required<PaginationOptions<T>>();
  state$ = input.required<State<T[], { endOfData: boolean }>>();
  onLoad = output<void>();

  ngOnInit(): void {
    this.loadMore();
  }

  onScroll(): void {
    console.log('onScroll');
    this.paginationOptions$().pageNumber++;
    this.loadMore();
  }

  loadMore(): void {
    console.log('loadMore');
    if (!this.state$().metadata?.endOfData) {
      this.onLoad.emit();
    }
  }

}
