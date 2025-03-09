import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
    selector: 'app-tag',
    imports: [CommonModule],
    template: `
    <span [ngClass]="colorClasses" class="px-3 py-1 rounded-full text-sm font-medium">
      {{ text }}
    </span>
  `
})
export class TagComponent {
    @Input() variant: 'yellow' | 'green' | 'red' | 'gray' = 'gray';
    @Input() text: string = '';

    get colorClasses(): string {
        const classes = {
            'yellow': 'bg-yellow-100 text-yellow-800 dark:bg-yellow-900 dark:text-yellow-300',
            'green': 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-300',
            'red': 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-300',
            'gray': 'bg-gray-100 text-gray-800 dark:bg-gray-900 dark:text-gray-300'
        };
        return classes[this.variant];
    }
} 