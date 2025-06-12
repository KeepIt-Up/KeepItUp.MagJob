using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate;

/// <summary>
/// Represents a member of an organization.
/// </summary>
public class Member : BaseEntity
{
    /// <summary>
    /// User ID.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Organization ID.
    /// </summary>
    public Guid OrganizationId { get; private set; }

    /// <summary>
    /// List of IDs of roles assigned to the member.
    /// </summary>
    private readonly List<Guid> _roleIds = new();

    /// <summary>
    /// List of IDs of roles assigned to the member (read-only).
    /// </summary>
    public IReadOnlyCollection<Guid> RoleIds => _roleIds.AsReadOnly();

    /// <summary>
    /// List of roles assigned to the member (navigation property for EF Core).
    /// </summary>
    public virtual ICollection<Role> Roles { get; private set; } = new List<Role>();

    /// <summary>
    /// Date of joining the organization.
    /// </summary>
    public DateTime JoinedAt { get; private set; } = DateTime.UtcNow;

    /// <summary>
    /// Private constructor for EF Core and factory creation.
    /// </summary>
    private Member() { }

    /// <summary>
    /// Creates a new member of an organization.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="organizationId">Organization ID.</param>
    /// <param name="roleId">ID of the initial role.</param>
    /// <returns>New member of an organization.</returns>
    public static Member Create(Guid userId, Guid organizationId, Guid roleId)
    {
        Guard.Against.Default(userId, nameof(userId));
        Guard.Against.Default(organizationId, nameof(organizationId));
        Guard.Against.Default(roleId, nameof(roleId));

        var member = new Member
        {
            UserId = userId,
            OrganizationId = organizationId
        };

        member._roleIds.Add(roleId);
        member.RegisterDomainEventAndUpdate(new MemberCreatedEvent(member.Id, organizationId, userId, roleId));

        return member;
    }

    /// <summary>
    /// Helper method to synchronize roles with other members of an organization.
    /// Should be called by Organization after loading all roles.
    /// </summary>
    /// <param name="organizationRoles">All roles available in the organization.</param>
    public void SyncRoles(IEnumerable<Role> organizationRoles)
    {
        Roles.Clear();

        foreach (var roleId in _roleIds)
        {
            var role = organizationRoles.FirstOrDefault(r => r.Id == roleId);
            if (role != null)
            {
                Roles.Add(role);
            }
        }
    }

    /// <summary>
    /// Assigns a new role to a member of an organization.
    /// </summary>
    /// <param name="roleId">ID of the role to assign.</param>
    /// <param name="role">Optional instance of the role, if available (for efficiency).</param>
    public void AssignRole(Guid roleId, Role? role = null)
    {
        Guard.Against.Default(roleId, nameof(roleId));

        if (!_roleIds.Contains(roleId))
        {
            _roleIds.Add(roleId);

            // Add also to the Roles navigation property, if the instance was provided
            if (role != null && !Roles.Any(r => r.Id == roleId))
            {
                Roles.Add(role);
            }

            RegisterDomainEventAndUpdate(new RoleAssignedToMemberEvent(Id, OrganizationId, UserId, roleId));
        }
    }

    /// <summary>
    /// Removes a role assigned to a member of an organization.
    /// </summary>
    /// <param name="roleId">ID of the role to remove.</param>
    /// <returns>True, if the role was removed; otherwise false.</returns>
    public bool RemoveRole(Guid roleId)
    {
        Guard.Against.Default(roleId, nameof(roleId));

        // Don't allow removing the last role
        if (_roleIds.Count <= 1)
        {
            return false;
        }

        bool removed = _roleIds.Remove(roleId);

        if (removed)
        {
            // Remove also from the Roles navigation property
            var roleToRemove = Roles.FirstOrDefault(r => r.Id == roleId);
            if (roleToRemove != null)
            {
                Roles.Remove(roleToRemove);
            }

            RegisterDomainEventAndUpdate(new RoleRevokedFromMemberEvent(Id, OrganizationId, UserId, roleId));
        }

        return removed;
    }

    /// <summary>
    /// Checks if a member has a specific role.
    /// </summary>
    /// <param name="roleId">ID of the role.</param>
    /// <returns>True, if the member has the role; otherwise false.</returns>
    public bool HasRole(Guid roleId)
    {
        return _roleIds.Contains(roleId);
    }
}
