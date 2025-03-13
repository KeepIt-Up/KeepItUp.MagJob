import { Component, Input, Output, EventEmitter, input, output } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-modal',
  imports: [CommonModule],
  templateUrl: './modal.component.html'
})
export class ModalComponent {
  isOpen = input.required<boolean>();
  title = input.required<string>();

  onCloseClick = output<void>();
}
