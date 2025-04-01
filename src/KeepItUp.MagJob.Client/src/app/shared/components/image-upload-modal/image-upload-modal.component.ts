import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ModalComponent } from '../modal/modal.component';
import { ButtonComponent } from '../button/button.component';

@Component({
  selector: 'app-image-upload-modal',
  imports: [CommonModule, ModalComponent, ButtonComponent],
  templateUrl: './image-upload-modal.component.html',
})
export class ImageUploadModalComponent {
  @Input() isOpen = false;
  @Input() title = '';
  @Input() maxFileSize = 10 * 1024 * 1024; // 10MB default max size
  @Input() acceptedFileTypes = ['image/jpeg', 'image/png', 'image/gif']; // Default accepted file types

  @Output() closeModal = new EventEmitter<void>();
  @Output() imageUpload = new EventEmitter<File>();

  selectedFile: File | null = null;
  previewUrl: string | null = null;
  isDragging = false;
  errorMessage = '';

  onClose(): void {
    this.resetState();
    this.closeModal.emit();
  }

  resetState(): void {
    this.selectedFile = null;
    this.previewUrl = null;
    this.errorMessage = '';
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files?.length) {
      const file = input.files[0];
      if (this.validateFile(file)) {
        this.selectedFile = file;
        this.createPreview();
        this.errorMessage = '';
      }
    }
  }

  onUpload(): void {
    if (this.selectedFile) {
      this.imageUpload.emit(this.selectedFile);
      this.onClose();
    }
  }

  private createPreview(): void {
    if (this.selectedFile) {
      const reader = new FileReader();
      reader.onload = () => {
        this.previewUrl = reader.result as string;
      };
      reader.readAsDataURL(this.selectedFile);
    }
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;

    const files = event.dataTransfer?.files;
    if (files?.length) {
      const file = files[0];
      if (this.validateFile(file)) {
        this.selectedFile = file;
        this.createPreview();
        this.errorMessage = '';
      }
    }
  }

  validateFile(file: File): boolean {
    // Check file type
    if (!this.acceptedFileTypes.includes(file.type)) {
      this.errorMessage = 'Invalid file type. Please upload a JPEG, PNG, or GIF file.';
      return false;
    }

    // Check file size
    if (file.size > this.maxFileSize) {
      this.errorMessage = `File is too large. Maximum file size is ${this.maxFileSize / (1024 * 1024)}MB.`;
      return false;
    }

    return true;
  }

  onRemoveFile(): void {
    this.resetState();
  }
}
