export interface Role {
  id: string;
  name: string;
  description?: string;
  color?: string;
  organizationId: string;
  permissions: Permission[];
}

export interface Permission {
  id: string;
  name: string;
  description?: string;
  category?: string;
}

export interface CreateRolePayload {
  name: string;
  organizationId: string;
}

export interface AssignMembersPayload {
  roleId: string;
  roleMembers: { memberId: string }[];
}
