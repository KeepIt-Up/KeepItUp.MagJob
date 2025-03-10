import { Component, inject, Input, OnInit } from '@angular/core';
import { ColumnDefinition } from '../../../../shared/components/table/table.component';
import { RoleApiService } from '../../../apis/role.api.service';
import { TableWithPaginationComponent } from '../../../../shared/components/table-with-pagination/table-with-pagination.component';
import { SearchInputComponent } from '../../../../shared/components/search-input/search-input.component';
import { PaginationOptions } from '../../../../shared/components/pagination/pagination.component';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { NotificationService } from '@shared/services/notification.service';
import { RoleActionsComponent } from './role-actions.component';
import { Role } from '../../../models/role/role';

@Component({
  selector: 'app-roles-table',
  imports: [TableWithPaginationComponent, SearchInputComponent, RouterLink],
  templateUrl: './role-table.component.html'
})
export class RolesTableComponent implements OnInit {
  roleService = inject(RoleApiService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  @Input() organizationId!: string;
  paginationOptions!: PaginationOptions<Role>;
  endpointURL!: string;
  queryParams!: Record<any, any>;
  notificationService = inject(NotificationService);

  ngOnInit(): void {
    this.route.parent?.params.subscribe(params => {
      this.organizationId = params['organizationId'];
      this.queryParams = {
        organizationId: this.organizationId
      };
      this.paginationOptions = {
        pageNumber: 1,
        pageSize: 10,
        sortField: "id",
        ascending: true
      };
      this.endpointURL = `/api/organizations/roles`;
    });
  }

  columnsConfig: ColumnDefinition<Role>[] = [
    {
      title: "Id",
      modelProp: "id",
    },
    {
      title: "Name",
      modelProp: 'name',
      isSortable: true
    },
    {
      title: "Description",
      modelProp: 'description',
      isSortable: true
    },
    {
      title: "Members Count",
      modelProp: 'members',
      computeValue: (row) => row.members.length.toString()
    },
    {
      title: "Actions",
      modelProp: 'id',
      component: {
        type: RoleActionsComponent,
        inputs: (row) => ({
          roleId: row.id,
          onEditCallback: () => this.navigateToRoleManagement(row.id),
        }),
      }
    }
  ];

  navigateToRoleManagement(roleId: string) {
    this.router.navigate(['/organization', this.organizationId, 'roles', roleId]);
  }

  deleteRole(roleId: string) {
    // Implement role deletion logic
  }
} 