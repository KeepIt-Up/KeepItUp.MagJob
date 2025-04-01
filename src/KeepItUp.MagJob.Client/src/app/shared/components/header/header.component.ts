import { Component, input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="mb-6">
      <h1
        class="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 text-2xl sm:text-3xl font-bold text-gray-900 dark:text-white"
      >
        <span>{{ title() }}</span>
        <div class="flex-shrink-0">
          <ng-content></ng-content>
        </div>
      </h1>
    </div>
  `,
})
export class HeaderComponent {
  title = input<string>('');
}
