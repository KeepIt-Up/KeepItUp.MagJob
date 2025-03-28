import { CommonModule, NgClass } from '@angular/common';
import { Component, input } from '@angular/core';

export type TagColor =
  | 'primary'
  | 'info'
  | 'success'
  | 'danger'
  | 'warning'
  | 'dark'
  | 'blue'
  | 'purple'
  | 'yellow'
  | 'green'
  | 'red'
  | 'gray';
export type TagSize = 'sm' | 'md' | 'lg';

@Component({
  selector: 'app-tag',
  standalone: true,
  imports: [CommonModule, NgClass],
  template: `
    <span
      class="app-tag"
      [ngClass]="[
        'app-tag--' + variant(),
        'app-tag--' + size(),
        rounded() ? 'app-tag--rounded' : '',
        customClass(),
      ]"
    >
      <ng-content></ng-content>
      {{ text() }}
    </span>
  `,
  styleUrl: './tag.component.scss',
})
export class TagComponent {
  /**
   * @description The text content of the tag
   */
  text = input<string>('');

  /**
   * @description The color variant of the tag
   * @default 'dark'
   */
  variant = input<TagColor>('dark');

  /**
   * @description The size of the tag
   */
  size = input<TagSize>('md');

  /**
   * @description Whether the tag should have full rounded corners
   */
  rounded = input<boolean>(true);

  /**
   * @description Custom CSS class for the tag
   */
  customClass = input<string>('');
}
