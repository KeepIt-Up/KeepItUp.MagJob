import { Component, input, Input } from '@angular/core';

@Component({
  selector: 'app-header',
  template: `
    <h1 class="flex flex-row justify-between items-center text-3xl font-bold mb-4 text-gray-900 dark:text-white">
      {{ title() }}
      <ng-content></ng-content>
    </h1>
  `
})
export class HeaderComponent {
  title = input<string>('');
}
