import { Component, Input, Output, EventEmitter, forwardRef, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ControlValueAccessor,
  NG_VALUE_ACCESSOR,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { NgIcon } from '@ng-icons/core';

@Component({
  selector: 'app-input',
  templateUrl: './input.component.html',
  styleUrls: ['./input.component.scss'],
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, NgIcon],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => InputComponent),
      multi: true,
    },
  ],
})
export class InputComponent implements ControlValueAccessor, OnInit {
  @Input() type: 'text' | 'password' | 'email' | 'number' | 'tel' | 'url' | 'search' = 'text';
  @Input() placeholder = '';
  @Input() label = '';
  @Input() name = '';
  @Input() id = '';
  @Input() required = false;
  @Input() disabled = false;
  @Input() readonly = false;
  @Input() helperText = '';
  @Input() errorText = '';
  @Input() hasError = false;
  @Input() iconLeft = '';
  @Input() iconRight = '';
  @Input() size: 'sm' | 'md' | 'lg' = 'md';
  @Input() fullWidth = false;
  @Input() set error(value: string) {
    this.errorText = value;
    this.hasError = !!value;
  }

  @Output() valueChanged = new EventEmitter<string>();
  @Output() blurred = new EventEmitter<FocusEvent>();
  @Output() focused = new EventEmitter<FocusEvent>();

  value = '';
  isFocused = false;

  private onChange: (value: string) => void = (_: string) => {
    /* This will be overridden */
  };
  private onTouched: () => void = () => {
    /* This will be overridden */
  };

  ngOnInit(): void {
    if (!this.id) {
      this.id = `input-${Math.random().toString(36).substring(2, 9)}`;
    }
  }

  get inputClasses(): string {
    const baseClasses = 'app-input__field';
    const sizeClass = `app-input__field--${this.size}`;
    const stateClass = this.hasError ? 'app-input__field--error' : '';
    const iconLeftClass = this.iconLeft ? 'app-input__field--has-icon-left' : '';
    const iconRightClass = this.iconRight ? 'app-input__field--has-icon-right' : '';
    const fullWidthClass = this.fullWidth ? 'app-input__field--full-width' : '';

    return [baseClasses, sizeClass, stateClass, iconLeftClass, iconRightClass, fullWidthClass]
      .filter(Boolean)
      .join(' ');
  }

  get containerClasses(): string {
    const baseClasses = 'app-input';
    const fullWidthClass = this.fullWidth ? 'app-input--full-width' : '';

    return [baseClasses, fullWidthClass].filter(Boolean).join(' ');
  }

  onInputChange(event: Event): void {
    const inputValue = (event.target as HTMLInputElement).value;
    this.value = inputValue;
    this.onChange(inputValue);
    this.valueChanged.emit(inputValue);
  }

  onInputBlur(event: FocusEvent): void {
    this.isFocused = false;
    this.onTouched();
    this.blurred.emit(event);
  }

  onInputFocus(event: FocusEvent): void {
    this.isFocused = true;
    this.focused.emit(event);
  }

  // ControlValueAccessor implementation
  writeValue(value: string): void {
    this.value = value || '';
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
}
