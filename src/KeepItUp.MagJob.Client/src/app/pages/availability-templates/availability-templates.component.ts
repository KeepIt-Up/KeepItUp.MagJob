import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, AsyncPipe } from '@angular/common';
import { HeaderComponent } from '@shared/components/header/header.component';
import { AvailabilityTemplatesService } from '../../features/calendar/services/availability-templates.service';
import { AvailabilityTemplateCardComponent } from '../../features/calendar/components/availability-template-card/availability-template-card.component';
import { SpinnerComponent } from '@shared/components/spinner/spinner.component';
import { ErrorAlertComponent } from '@shared/components/error-alert/error-alert.component';
import { ButtonComponent } from '@shared/components/button/button.component';

@Component({
  selector: 'app-availability-templates',
  standalone: true,
  imports: [
    CommonModule,
    AsyncPipe,
    HeaderComponent,
    AvailabilityTemplateCardComponent,
    SpinnerComponent,
    ErrorAlertComponent,
    ButtonComponent,
  ],
  templateUrl: './availability-templates.component.html',
  styleUrls: ['./availability-templates.component.scss'],
})
export class AvailabilityTemplatesComponent implements OnInit {
  private readonly availabilityTemplatesService = inject(AvailabilityTemplatesService);

  templatesState$ = this.availabilityTemplatesService.templatesState$;
  paginationState$ = this.availabilityTemplatesService.paginationState$;

  ngOnInit(): void {
    this.loadTemplates();
  }

  loadTemplates(): void {
    this.availabilityTemplatesService.loadAvailabilityTemplates().subscribe();
  }

  loadMore(): void {
    this.availabilityTemplatesService.loadMore();
  }

  reload(): void {
    this.availabilityTemplatesService.reload();
  }
}
