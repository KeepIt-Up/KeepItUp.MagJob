import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { GraphicsService } from '../../../features/calendar/services/graphics.service';
import { GraphicResponse } from '../../../features/calendar/models/graphic.model';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { TagComponent } from '../../../shared/components/tag/tag.component';

@Component({
  selector: 'app-graphic-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, ButtonComponent, TagComponent],
  templateUrl: './graphic-detail.component.html',
  styleUrls: ['./graphic-detail.component.scss'],
})
export class GraphicDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly graphicsService = inject(GraphicsService);

  graphic: GraphicResponse | null = null;
  isLoading = true;
  error: string | null = null;

  ngOnInit(): void {
    const graphicId = this.route.snapshot.params['id'];
    if (graphicId) {
      this.loadGraphic(String(graphicId));
    } else {
      this.error = 'No graphic ID provided';
      this.isLoading = false;
    }
  }

  // Helper getter for template
  get hasValidGraphic(): boolean {
    return !this.isLoading && !this.error && this.graphic !== null;
  }

  // Type-safe getter for the graphic
  get currentGraphic(): GraphicResponse {
    if (!this.graphic) {
      throw new Error('Graphic not loaded');
    }
    return this.graphic;
  }

  private loadGraphic(id: string): void {
    this.isLoading = true;
    this.error = null;

    this.graphicsService.getGraphic(id).subscribe({
      next: graphic => {
        this.graphic = graphic;
        this.isLoading = false;
      },
      error: error => {
        this.error = error.message || 'Failed to load graphic details';
        this.isLoading = false;
      },
    });
  }

  getTimeEntryDate(dateTime: string): string {
    return new Date(dateTime).toLocaleDateString();
  }

  getTimeEntryTime(dateTime: string): string {
    return new Date(dateTime).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }

  getTotalHours(graphic: GraphicResponse): number {
    if (!graphic.timeEntries) return 0;

    return graphic.timeEntries.reduce((total, entry) => {
      const start = new Date(entry.startDateTime);
      const end = new Date(entry.endDateTime);
      const duration = (end.getTime() - start.getTime()) / (1000 * 60 * 60);
      return total + duration;
    }, 0);
  }

}
