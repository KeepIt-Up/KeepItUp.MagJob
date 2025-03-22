import { Component, computed, inject, OnDestroy, OnInit, Signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ScrollControlService } from '@shared/services/scroll-control.service';
import { TabsComponent } from '@shared/components/tabs/tabs.component';
import { ActivatedRoute } from '@angular/router';
import { InfiniteListComponent } from '@shared/components/infinite-list/infinite-list.component';
import { RolesListComponent } from '../../../features/roles/components/roles-list/roles-list.component';
import { MemberSearchModalComponent } from '../../../features/members/components/member-search-modal/member-search-modal.component';
import { RoleService } from '../../../features/roles/services/role.service';
import { Permission, Role } from '../../../features/roles/models/role.model';
import { MemberService } from '../../../features/members/services/member.service';
import { Member } from '../../../features/members/models/member';

interface Tab {
  id: string;
  label: string;
}

@Component({
  selector: 'app-roles-management',
  imports: [
    FormsModule,
    TabsComponent,
    RolesListComponent,
    MemberSearchModalComponent,
    InfiniteListComponent,
  ],
  templateUrl: './roles-management.component.html',
})
export class RolesManagementComponent implements OnInit, OnDestroy {
  //scroll
  private scrollControlService = inject(ScrollControlService);

  //route
  private route = inject(ActivatedRoute);
  organizationId!: string;

  //roles and permissions
  private roleService = inject(RoleService);
  state$ = this.roleService.roles$;
  paginationOptions$ = this.roleService.paginationOptions$;
  selectedRole$ = this.roleService.selectedRole$;
  permissions$ = this.roleService.permissions$;
  selectedRolePermission$: Signal<{ permission: Permission; value: boolean }[]> = computed(
    () =>
      this.permissions$().data?.map(p => ({
        permission: p,
        value: this.selectedRole$()?.permissions.some(p2 => p2.id === p.id) ?? false,
      })) ?? [],
  );

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
    this.roleService
      .updateRolePermissions(
        this.selectedRole$()!.id,
        this.selectedRolePermission$()
          .filter(p => p.value)
          .map(p => p.permission.id),
      )
      .subscribe();
  }

  togglePermission(permissionId: number) {
    if (!this.selectedRole$()) return;
    this.selectedRolePermission$().find(p => p.permission.id === permissionId)!.value =
      !this.selectedRolePermission$().find(p => p.permission.id === permissionId)!.value;
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
    if (this.isMemberInRole(member)) {
      // Remove member from role
      this.roleService.removeMembersFromRole(this.selectedRole$()!.id, [member.id]).subscribe();
    } else {
      // Add member to role
      this.roleService.addMembersToRole(this.selectedRole$()!.id, [member.id]).subscribe();
    }
  }
}
