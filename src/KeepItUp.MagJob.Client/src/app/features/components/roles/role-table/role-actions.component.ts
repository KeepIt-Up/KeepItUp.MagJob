import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-role-actions',
  imports: [],
  template: `
    <div class="flex gap-2">
      <button (click)="onEditCallback()" class="h-8 w-8 rounded border border-gray-400 text-blue-500 hover:bg-gray-100 dark:border-blue-400 dark:text-blue-400 dark:hover:bg-blue-400">
        <i class="bi bi-pencil-square m-1"></i>
      </button>
    </div>
  `
})
export class RoleActionsComponent {
  @Input() roleId!: string;
  @Input() onDeleteCallback: () => void = () => { };
  @Input() onEditCallback: () => void = () => { };
} 