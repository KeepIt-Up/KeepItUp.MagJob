import { Component, Input, Output, EventEmitter, forwardRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ControlValueAccessor,
  NG_VALUE_ACCESSOR,
  ReactiveFormsModule,
  FormsModule,
} from '@angular/forms';

@Component({
  selector: 'app-textarea',
  templateUrl: './textarea.component.html',
  styleUrls: ['./textarea.component.scss'],
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => TextareaComponent),
      multi: true,
    },
  ],
})
export class TextareaComponent implements ControlValueAccessor {
  @Input() label: string = '';
  @Input() placeholder: string = '';
  @Input() rows: string = '3';
  @Input() required: boolean = false;
  @Input() disabled: boolean = false;
  @Input() helperText: string = '';
  @Input() fullWidth: boolean = true;
  @Input() size: 'sm' | 'md' | 'lg' = 'md';
  @Input() errorText: string = '';
  @Input() hasError: boolean = false;
  @Input() set error(value: string) {
    this.errorText = value;
    this.hasError = !!value;
  }
  @Output() valueChanged = new EventEmitter<string>();
  @Output() blurred = new EventEmitter<FocusEvent>();

  value: string = '';
  touched: boolean = false;
  id: string = `textarea-${Math.random().toString(36).substring(2, 9)}`;

  onChange: (value: string) => void = (_: string) => {
    /* This is intentionally empty */
  };
  onTouched: () => void = () => {
    /* This is intentionally empty */
  };

  get errorMessage(): string {
    return this.error || this.errorText;
  }

  get hasErrorState(): boolean {
    return !!this.error || this.hasError;
  }

  get containerClasses(): string {
    const baseClasses = 'app-textarea';
    const fullWidthClass = this.fullWidth ? 'app-textarea--full-width' : '';
    return [baseClasses, fullWidthClass].filter(Boolean).join(' ');
  }

  get textareaClasses(): string {
    const baseClasses = 'app-textarea__field';
    const sizeClass = `app-textarea__field--${this.size}`;
    const stateClass = this.hasErrorState ? 'app-textarea__field--error' : '';
    const fullWidthClass = this.fullWidth ? 'app-textarea__field--full-width' : '';

    return [baseClasses, sizeClass, stateClass, fullWidthClass].filter(Boolean).join(' ');
  }

  writeValue(value: string): void {
    this.value = value;
  }

  registerOnChange(fn: (value: string) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

  onInput(event: Event): void {
    const target = event.target as HTMLTextAreaElement;
    this.value = target.value;
    this.onChange(this.value);
    this.valueChanged.emit(this.value);
  }

  onBlur(event?: FocusEvent): void {
    if (!this.touched) {
      this.touched = true;
      this.onTouched();
    }
    if (event) {
      this.blurred.emit(event);
    }
  }
}
