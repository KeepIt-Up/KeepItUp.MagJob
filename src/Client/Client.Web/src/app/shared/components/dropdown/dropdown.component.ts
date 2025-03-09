import { CommonModule } from '@angular/common';
import { Component, Input, input } from '@angular/core';

@Component({
    selector: 'app-dropdown',
    imports: [CommonModule],
    templateUrl: './dropdown.component.html'
})
export class DropdownComponent {
    @Input() isOpen: boolean = false;
    userName = input<string>('');
    userEmail = input<string>('');
} 