import { Component, Input, Output, EventEmitter, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { AvailabilityTemplateResponse } from '../../models/availability-template-response.model';
import { GraphicApiService } from '../../services/graphic.api.service';
import { PostCreateAndPopulateGraphic } from '../../models/post-create-and-populate-graphic.model';
import { UserContextService } from '../../../../features/users/services/user-context.service';

@Component({
  selector: 'app-create-graphic-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule, ButtonComponent],
  templateUrl: './create-graphic-dialog.component.html',
  styleUrls: ['./create-graphic-dialog.component.scss'],
})
export class CreateGraphicDialogComponent implements OnInit {
  @Input() template!: AvailabilityTemplateResponse;
  @Input() isOpen = false;
  @Output() close = new EventEmitter<void>();
  @Output() graphicCreated = new EventEmitter<void>();

  private readonly graphicApiService = inject(GraphicApiService);
  private readonly userContextService = inject(UserContextService);

  isCreating = false;
  graphicName = '';
  startDate = '';

  ngOnInit() {
    // Set default values when dialog opens
    this.graphicName = `${this.template?.name || 'New'} Graphic`;
    this.startDate = new Date().toISOString().split('T')[0]; // Today's date in YYYY-MM-DD format
  }

  onClose() {
    this.close.emit();
    this.resetForm();
  }

  onSubmit() {
    if (!this.graphicName.trim() || !this.startDate || this.isCreating) {
      return;
    }

    const currentUser = this.userContextService.getCurrentUser();
    if (!currentUser) {
      console.error('User not authenticated');
      return;
    }

    this.isCreating = true;

    const request: PostCreateAndPopulateGraphic = {
      name: this.graphicName.trim(),
      managerId: currentUser.id,
      availabilityTemplateId: this.template.id,
      startDate: this.startDate,
    };

    this.graphicApiService.createAndPopulateGraphic(request).subscribe({
      next: response => {
        console.log('Graphic created successfully:', response);
        this.isCreating = false;
        this.graphicCreated.emit();
        this.onClose();
      },
      error: error => {
        console.error('Failed to create graphic:', error);
        this.isCreating = false;
        // You could show an error message here
      },
    });
  }

  private resetForm() {
    this.graphicName = '';
    this.startDate = '';
    this.isCreating = false;
  }

  getTodayDate(): string {
    return new Date().toISOString().split('T')[0];
  }

  // Add this method to your component class
  onOverlayKeydown(event: KeyboardEvent): void {
    if (event.key === 'Escape') {
      this.onClose();
    }
  }
}
