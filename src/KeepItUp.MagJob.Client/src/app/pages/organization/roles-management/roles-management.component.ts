import { Component, computed, inject, OnDestroy, OnInit, Signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ScrollControlService } from '@shared/services/scroll-control.service';
import { TabsComponent } from '@shared/components/tabs/tabs.component';
import { ButtonComponent } from '@shared/components/button/button.component';
import { ActivatedRoute } from '@angular/router';
import { InfiniteListComponent } from '@shared/components/infinite-list/infinite-list.component';
import { RolesListComponent } from '../../../features/roles/components/roles-list/roles-list.component';
import { MemberSearchModalComponent } from '../../../features/members/components/member-search-modal/member-search-modal.component';
import { RoleService } from '../../../features/roles/services/role.service';
import { Permission, Role } from '../../../features/roles/models/role.model';
import { MemberService } from '../../../features/members/services/member.service';
import { Member } from '../../../features/members/models/member';
import { OrganizationService } from '../../../features/organizations/services/organization.service';
import { NotificationService } from '@shared/services/notification.service';
import { PermissionService } from '../../../features/roles/models/permission.service';

interface Tab {
  id: string;
  label: string;
}

interface PermissionWithValue {
  permission: Permission;
  value: boolean;
}

@Component({
  selector: 'app-roles-management',
  imports: [
    FormsModule,
    TabsComponent,
    RolesListComponent,
    MemberSearchModalComponent,
    InfiniteListComponent,
    ButtonComponent,
  ],
  templateUrl: './roles-management.component.html',
})
export class RolesManagementComponent implements OnInit, OnDestroy {
  //scroll
  private scrollControlService = inject(ScrollControlService);

  //route
  private route = inject(ActivatedRoute);
  organizationId!: string;

  //organization
  private organizationService = inject(OrganizationService);
  private notificationService = inject(NotificationService);

  //roles and permissions
  private roleService = inject(RoleService);
  private permissionService = inject(PermissionService);
  state$ = this.roleService.roles$;
  paginationOptions$ = this.roleService.paginationOptions$;
  selectedRole$ = this.roleService.selectedRole$;
  permissions$ = this.roleService.permissions$;
  selectedRolePermission$: Signal<PermissionWithValue[]> = computed(() => {
    const permissionsData = this.permissions$().data;

    // Check if permissions and selected role are available
    if (!permissionsData || !this.selectedRole$()) {
      return (
        permissionsData?.map(p => ({
          permission: p,
          value: false,
        })) ?? []
      );
    }

    // Map all permissions with their status (checked/unchecked)
    return permissionsData.map(p => ({
      permission: p,
      value:
        this.selectedRole$()?.permissions.some(p2 => p2.id === p.id || p2.name === p.name) ?? false,
    }));
  });

  // Metoda zwracająca listę kategorii uprawnień
  permissionCategories(): string[] {
    const permissionsData = this.permissions$().data;
    if (!permissionsData) return [];

    // Wyodrębnij unikalne kategorie
    const categories = new Set<string>();
    permissionsData.forEach(p => {
      if (p.category) {
        categories.add(p.category);
      } else {
        categories.add('Inne'); // Domyślna kategoria dla uprawnień bez kategorii
      }
    });

    return Array.from(categories);
  }

  // Metoda zwracająca uprawnienia dla danej kategorii
  getCategoryPermissions(category: string): PermissionWithValue[] {
    return this.selectedRolePermission$().filter(
      p => p.permission.category === category || (category === 'Inne' && !p.permission.category),
    );
  }

  //tabs
  tabs: Tab[] = [
    { id: 'appearance', label: 'Appearance' },
    { id: 'permissions', label: 'Permissions' },
    { id: 'assignments', label: 'Assignments' },
  ];
  // do wyciągania do komponentu
  activeTab = 'appearance';

  //members
  memberSearchQuery = '';
  members: Member[] = [];
  memberService = inject(MemberService);
  memberSearchState$ = this.memberService.memberSearchState$;
  showMemberSearchModal = false;

  ngOnInit(): void {
    this.scrollControlService.setScrollable(false);

    this.route.parent?.params.subscribe(params => {
      this.organizationId = params['organizationId'] as string;

      // Load organization data
      this.organizationService.getOrganization(this.organizationId).subscribe();

      // Load roles data
      this.loadMoreRoles();
    });
  }

  ngOnDestroy(): void {
    this.scrollControlService.setScrollable(true);
  }

  setActiveTab(tabId: string) {
    this.activeTab = tabId;
  }

  loadMoreRoles() {
    this.roleService.getRoles(this.organizationId).subscribe(() => {
      if (!this.selectedRole$() && this.state$().data?.length) {
        this.selectRole(this.state$().data![0]);
      }
    });
  }

  selectRole(role: Role) {
    this.roleService.selectRole(role);
    // Ensure permissions are loaded when changing role
    this.permissionService.getPermissions();
  }

  addNewRole() {
    this.roleService.createRole(this.organizationId, 'New Role').subscribe();
  }

  updateRole() {
    if (!this.selectedRole$()) return;

    const role = this.selectedRole$()!;
    this.roleService
      .updateRole(role.id, {
        name: role.name,
        color: role.color,
      })
      .subscribe();
  }

  deleteRole() {
    if (
      !this.selectedRole$() ||
      !confirm('Are you sure you want to delete this role? This action cannot be undone.')
    ) {
      return;
    }

    this.roleService.deleteRole().subscribe();
  }

  updateRolePermissions() {
    if (!this.selectedRole$()) return;

    // Check if organization data is available
    const organizationState = this.organizationService.state$();
    if (!organizationState.data) {
      // Try to load the organization data first
      this.notificationService.show('Loading organization data...', 'info');
      this.organizationService.getOrganization(this.organizationId).subscribe({
        next: () => {
          // Once organization data is loaded, proceed with updating permissions
          this.performUpdateRolePermissions();
        },
        error: () => {
          this.notificationService.show('Failed to load organization data', 'error');
        },
      });
    } else {
      // Organization data is already available, proceed with updating permissions
      this.performUpdateRolePermissions();
    }
  }

  private performUpdateRolePermissions() {
    if (!this.selectedRole$()) return;

    const selectedPermissionNames = this.selectedRolePermission$()
      .filter(p => p.value)
      .map(p => p.permission.name);

    this.roleService
      .updateRolePermissions(this.selectedRole$()!.id, selectedPermissionNames)
      .subscribe({
        next: () => {
          // Odśwież dane roli po aktualizacji uprawnień
          const role = this.selectedRole$();
          if (role) {
            // Odświeżenie listy ról, aby uzyskać zaktualizowane dane
            this.roleService.getRoles(this.organizationId).subscribe(() => {
              // Ponowne wybranie tej samej roli, aby odświeżyć jej widok
              this.selectRole(role);
            });
          }
        },
        error: error => {
          console.error('Error updating permissions:', error);
        },
      });
  }

  togglePermission(permissionId: string | undefined) {
    if (!this.selectedRole$() || permissionId === undefined) return;

    const permission = this.selectedRolePermission$().find(p => p.permission.id === permissionId);
    if (permission) {
      permission.value = !permission.value;
    }
  }

  searchMembers(query: string) {
    this.memberSearchQuery = query;
    this.memberService.searchMembers(query, this.organizationId).subscribe();
  }

  loadMoreMembers() {
    this.memberService.searchMembers(this.memberSearchQuery, this.organizationId).subscribe();
  }

  isMemberInRole(member: Member): boolean {
    return this.selectedRole$()!.members.some(m => m.id === member.id) ?? false;
  }

  toggleMemberRole(member: Member) {
    if (!this.selectedRole$()) return;

    if (this.isMemberInRole(member)) {
      // Remove member from role
      this.roleService.removeMembersFromRole(this.selectedRole$()!.id, [member.id]).subscribe();
    } else {
      // Add member to role
      this.roleService.addMembersToRole(this.selectedRole$()!.id, [member.id]).subscribe();
    }
  }
}
