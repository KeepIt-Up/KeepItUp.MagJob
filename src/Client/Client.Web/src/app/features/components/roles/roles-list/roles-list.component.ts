import { Component, Input, Output, EventEmitter, input, output } from '@angular/core';
import { Role } from '../../../models/role/role';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'app-roles-list',
    imports: [CommonModule],
    templateUrl: './roles-list.component.html'
})
export class RolesListComponent {
    roles$ = input.required<Role[]>();
    selectedRole$ = input.required<Role | undefined>();
    onRoleSelect = output<Role>();
} 