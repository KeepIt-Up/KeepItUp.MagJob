import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { GraphicsService } from '../../../features/calendar/services/graphics.service';
import { GraphicCardComponent } from '../../../features/calendar/components/graphic-card/graphic-card.component';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { GraphicResponse } from '../../../features/calendar/models/graphic.model';

@Component({
  selector: 'app-graphics',
  standalone: true,
  imports: [CommonModule, RouterModule, GraphicCardComponent, ButtonComponent],
  templateUrl: './graphics.component.html',
  styleUrls: ['./graphics.component.scss'],
})
export class GraphicsComponent implements OnInit {
  private readonly graphicsService = inject(GraphicsService);

  graphics: GraphicResponse[] = [];
  isLoading = false;
  error: string | null = null;
  currentPage = 0;
  pageSize = 10;
  hasMorePages = false;

  ngOnInit(): void {
    this.loadGraphics();
  }

  loadGraphics(): void {
    this.isLoading = true;
    this.error = null;

    this.graphicsService.loadGraphics(this.currentPage, this.pageSize).subscribe({
      next: response => {
        console.log('Graphics response:', response); // Debug log

        // Use the correct property name from backend response
        const graphicsList = response.graphicsResponse || response.graphicResponseList || [];
        this.graphics = graphicsList;
        this.hasMorePages = graphicsList.length === this.pageSize;
        this.isLoading = false;
      },
      error: error => {
        console.error('Error loading graphics:', error);
        this.error = error.message || 'Failed to load graphics';
        this.isLoading = false;
      },
    });
  }

  loadMore(): void {
    if (this.hasMorePages && !this.isLoading) {
      this.isLoading = true;
      this.currentPage++;

      this.graphicsService.loadGraphics(this.currentPage, this.pageSize).subscribe({
        next: response => {
          const graphicsList = response.graphicsResponse || response.graphicResponseList || [];
          this.graphics = [...this.graphics, ...graphicsList];
          this.hasMorePages = graphicsList.length === this.pageSize;
          this.isLoading = false;
        },
        error: error => {
          this.error = error.message || 'Failed to load more graphics';
          this.isLoading = false;
          this.currentPage--; // Revert page increment on error
        },
      });
    }
  }

  reload(): void {
    this.currentPage = 0;
    this.graphics = [];
    this.loadGraphics();
  }
}
