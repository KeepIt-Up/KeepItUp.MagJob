import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgIcon } from '@ng-icons/core';

@Component({
  selector: 'app-button',
  templateUrl: './button.component.html',
  styleUrls: ['./button.component.scss'],
  standalone: true,
  imports: [CommonModule, NgIcon],
})
export class ButtonComponent {
  @Input() variant: 'primary' | 'outline' | 'text' = 'primary';
  @Input() size: 'sm' | 'md' | 'lg' = 'md';
  @Input() disabled = false;
  @Input() type: 'button' | 'submit' | 'reset' = 'button';
  @Input() iconLeft: string | null = null;
  @Input() iconRight: string | null = null;
  @Input() fullWidth = false;
  @Input() ariaLabel: string | null = null;

  @Output() clicked = new EventEmitter<void>();

  get buttonClasses(): string {
    const baseClasses = 'app-button';
    const variantClass = `app-button--${this.variant}`;
    const sizeClass = `app-button--${this.size}`;
    const fullWidthClass = this.fullWidth ? 'app-button--full-width' : '';

    return [baseClasses, variantClass, sizeClass, fullWidthClass].filter(Boolean).join(' ');
  }

  onClick(event: Event): void {
    if (!this.disabled) {
      this.clicked.emit();
    }
  }
}
