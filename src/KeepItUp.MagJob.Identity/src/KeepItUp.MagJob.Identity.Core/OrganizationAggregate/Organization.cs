using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;

namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate;

/// <summary>
/// Represents an organization in the system.
/// </summary>
public class Organization : BaseEntity, IAggregateRoot
{
    /// <summary>
    /// Name of the organization.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Description of the organization.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// URL of the organization's logo.
    /// </summary>
    public string? LogoUrl { get; private set; }

    /// <summary>
    /// URL of the organization's banner.
    /// </summary>
    public string? BannerUrl { get; private set; }

    /// <summary>
    /// ID of the organization's owner.
    /// </summary>
    public Guid OwnerId { get; private set; }

    /// <summary>
    /// Whether the organization is active.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// List of organization members.
    /// </summary>
    private readonly List<Member> _members = new();

    /// <summary>
    /// List of organization members (read-only).
    /// </summary>
    public IReadOnlyCollection<Member> Members => _members.AsReadOnly();

    /// <summary>
    /// List of roles in the organization.
    /// </summary>
    private readonly List<Role> _roles = new();

    /// <summary>
    /// List of roles in the organization (read-only).
    /// </summary>
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();


    // Private constructor for EF Core
    private Organization() { }

    /// <summary>
    /// Creates a new organization.
    /// </summary>
    /// <param name="name">Name of the organization.</param>
    /// <param name="ownerId">ID of the organization's owner.</param>
    /// <param name="description">Description of the organization.</param>
    /// <param name="logoUrl">URL of the organization's logo.</param>
    /// <param name="bannerUrl">URL of the organization's banner.</param>
    /// <returns>New organization.</returns>
    public static Organization Create(string name, Guid ownerId, string? description = null, string? logoUrl = null, string? bannerUrl = null)
    {
        Guard.Against.NullOrEmpty(name, nameof(name));
        Guard.Against.Default(ownerId, nameof(ownerId));

        var organization = new Organization
        {
            Name = name,
            OwnerId = ownerId,
            Description = description,
            LogoUrl = logoUrl,
            BannerUrl = bannerUrl
        };

        organization.RegisterDomainEvent(new OrganizationCreatedEvent(organization.Id, organization.Name, organization.OwnerId));

        return organization;
    }

    /// <summary>
    /// Initializes default roles and membership for the owner.
    /// This method should be called after saving the organization to the database,
    /// when the organization already has valid IDs.
    /// </summary>
    /// <returns>Organization with initialized roles and membership for the owner.</returns>
    public Organization InitializeRoles()
    {
        // Ensure the organization has valid ID
        Guard.Against.Default(Id, nameof(Id));

        var adminRole = Role.Create("Admin", Id, "Administrator organizacji", "#FF0000");
        var memberRole = Role.Create("Member", Id, "Członek organizacji", "#00FF00");
        var guestRole = Role.Create("Guest", Id, "Gość organizacji", "#0000FF");

        _roles.Add(adminRole);
        _roles.Add(memberRole);
        _roles.Add(guestRole);

        return this;
    }

    /// <summary>
    /// Initializes default roles and membership for the owner.
    /// This method should be called after saving the organization to the database,
    /// when the organization already has valid IDs.
    /// </summary>
    /// <returns>Organization with initialized roles and membership for the owner.</returns>
    public Organization InitializeOwner()
    {
        // Ensure the organization has valid ID
        Guard.Against.Default(Id, nameof(Id));

        // Find the Admin role
        var adminRole = _roles.FirstOrDefault(r => r.Name == "Admin");
        if (adminRole == null)
        {
            throw new InvalidOperationException("Nie znaleziono roli 'Admin' w organizacji. Uruchom najpierw InitializeRoles().");
        }

        // Add the owner as an admin
        var ownerMember = Member.Create(OwnerId, Id, adminRole);

        _members.Add(ownerMember);

        return this;
    }

    /// <summary>
    /// Updates the organization's data.
    /// </summary>
    /// <param name="name">Name of the organization.</param>
    /// <param name="description">Description of the organization.</param>
    /// <param name="logoUrl">URL of the organization's logo.</param>
    /// <param name="bannerUrl">URL of the organization's banner.</param>
    public void Update(string name, string? description, string? logoUrl = null, string? bannerUrl = null)
    {
        Guard.Against.NullOrEmpty(name, nameof(name));

        Name = name;
        Description = description;

        if (logoUrl != null)
        {
            LogoUrl = logoUrl;
        }

        if (bannerUrl != null)
        {
            BannerUrl = bannerUrl;
        }

        RegisterDomainEventAndUpdate(new OrganizationUpdatedEvent(Id, Name, OwnerId));
    }

    /// <summary>
    /// Updates the organization's logo.
    /// </summary>
    /// <param name="logoUrl">URL of the organization's logo.</param>
    public void UpdateLogo(string? logoUrl)
    {
        LogoUrl = logoUrl;

        RegisterDomainEventAndUpdate(new OrganizationLogoUpdatedEvent(Id, LogoUrl));
    }

    /// <summary>
    /// Updates the organization's banner.
    /// </summary>
    /// <param name="bannerUrl">URL of the organization's banner.</param>
    public void UpdateBanner(string? bannerUrl)
    {
        BannerUrl = bannerUrl;

        RegisterDomainEventAndUpdate(new OrganizationBannerUpdatedEvent(Id, BannerUrl));
    }

    /// <summary>
    /// Deactivates the organization.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;

        RegisterDomainEventAndUpdate(new OrganizationDeactivatedEvent(Id, Name, OwnerId));
    }

    /// <summary>
    /// Activates the organization.
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;

        RegisterDomainEventAndUpdate(new OrganizationActivatedEvent(Id, Name, OwnerId));
    }

    /// <summary>
    /// Updates the organization's data including activity status.
    /// This method properly handles status changes through domain methods.
    /// </summary>
    /// <param name="name">Name of the organization.</param>
    /// <param name="description">Description of the organization.</param>
    /// <param name="isActive">Whether the organization should be active.</param>
    /// <param name="logoUrl">URL of the organization's logo.</param>
    /// <param name="bannerUrl">URL of the organization's banner.</param>
    public void UpdateWithStatus(string name, string? description, bool isActive, string? logoUrl = null, string? bannerUrl = null)
    {
        // Update basic properties first
        Update(name, description, logoUrl, bannerUrl);

        // Handle status changes through domain methods
        if (isActive && !IsActive)
        {
            Activate();
        }
        else if (!isActive && IsActive)
        {
            Deactivate();
        }
    }

    /// <summary>
    /// Adds a new member to the organization.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="roleId">Role ID.</param>
    /// <returns>New member.</returns>
    public Member AddMember(Guid userId, Guid roleId)
    {
        Guard.Against.Default(userId, nameof(userId));
        Guard.Against.Default(roleId, nameof(roleId));

        var role = _roles.FirstOrDefault(r => r.Id == roleId);
        if (role == null)
        {
            throw new InvalidOperationException($"Rola o ID {roleId} nie istnieje w organizacji.");
        }

        var existingMember = _members.FirstOrDefault(m => m.UserId == userId);
        if (existingMember != null)
        {
            existingMember.AssignRole(role);

            RegisterDomainEventAndUpdate(new MemberRoleAssignedEvent(Id, userId, roleId));
            return existingMember;
        }

        var member = Member.Create(userId, Id, role);

        _members.Add(member);

        RegisterDomainEventAndUpdate(new MemberAddedEvent(Id, userId, roleId));

        return member;
    }

    /// <summary>
    /// Removes a member from the organization.
    /// </summary>
    /// <param name="userId">User ID.</param>
    public void RemoveMember(Guid userId)
    {
        Guard.Against.Default(userId, nameof(userId));

        if (userId == OwnerId)
        {
            throw new InvalidOperationException("Nie można usunąć właściciela organizacji.");
        }

        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member == null)
        {
            throw new InvalidOperationException($"Użytkownik o ID {userId} nie jest członkiem organizacji.");
        }

        _members.Remove(member);

        RegisterDomainEventAndUpdate(new MemberRemovedEvent(Id, userId));
    }

    /// <summary>
    /// Assigns a role to a member of the organization.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="roleId">Role ID.</param>
    public void AssignRoleToMember(Guid userId, Guid roleId)
    {
        Guard.Against.Default(userId, nameof(userId));
        Guard.Against.Default(roleId, nameof(roleId));

        var role = _roles.FirstOrDefault(r => r.Id == roleId);
        if (role == null)
        {
            throw new InvalidOperationException($"Rola o ID {roleId} nie istnieje w organizacji.");
        }

        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member == null)
        {
            throw new InvalidOperationException($"Użytkownik o ID {userId} nie jest członkiem organizacji.");
        }

        member.AssignRole(role);

        RegisterDomainEventAndUpdate(new MemberRoleAssignedEvent(Id, userId, roleId));
    }

    /// <summary>
    /// Removes a role assigned to a member of the organization.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <param name="roleId">Role ID.</param>
    public void RevokeRoleFromMember(Guid userId, Guid roleId)
    {
        Guard.Against.Default(userId, nameof(userId));
        Guard.Against.Default(roleId, nameof(roleId));

        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member == null)
        {
            throw new InvalidOperationException($"Użytkownik o ID {userId} nie jest członkiem organizacji.");
        }

        if (!member.HasRole(roleId))
        {
            throw new InvalidOperationException($"Użytkownik o ID {userId} nie posiada roli o ID {roleId}.");
        }

        var role = _roles.FirstOrDefault(r => r.Id == roleId);
        if (role == null)
        {
            throw new InvalidOperationException($"Rola o ID {roleId} nie istnieje w organizacji.");
        }

        if (!member.RemoveRole(role))
        {
            throw new InvalidOperationException("Nie można usunąć ostatniej roli przypisanej do członka organizacji.");
        }

        RegisterDomainEventAndUpdate(new MemberRoleRevokedEvent(Id, userId, roleId));
    }

    /// <summary>
    /// Adds a new role to the organization.
    /// </summary>
    /// <param name="name">Name of the role.</param>
    /// <param name="description">Description of the role.</param>
    /// <param name="color">Color of the role (in HEX format).</param>
    /// <returns>New role.</returns>
    public Role AddRole(string name, string? description = null, string? color = null)
    {
        Guard.Against.NullOrEmpty(name, nameof(name));


        var role = Role.Create(name, Id, description, color);
        _roles.Add(role);

        RegisterDomainEventAndUpdate(new RoleCreatedEvent(Id, role.Id, role.Name));

        return role;
    }

    /// <summary>
    /// Removes a role from the organization.
    /// </summary>
    /// <param name="roleId">Role ID.</param>
    public void RemoveRole(Guid roleId)
    {
        Guard.Against.Default(roleId, nameof(roleId));

        var role = _roles.FirstOrDefault(r => r.Id == roleId);
        if (role == null)
        {
            throw new InvalidOperationException($"Rola o ID {roleId} nie istnieje w organizacji.");
        }

        if (_members.Any(m => m.HasRole(roleId)))
        {
            throw new InvalidOperationException("Nie można usunąć roli, która jest przypisana do członków organizacji.");
        }

        _roles.Remove(role);

        RegisterDomainEventAndUpdate(new RoleDeletedEvent(Id, roleId, role.Name));
    }

    /// <summary>
    /// Adds a member to the organization from an accepted invitation.
    /// </summary>
    /// <param name="userId">User ID of the person accepting the invitation.</param>
    /// <param name="roleId">Role ID to assign to the member.</param>
    public Member AddMemberFromInvitation(Guid userId, Guid roleId)
    {
        Guard.Against.Default(userId, nameof(userId));
        Guard.Against.Default(roleId, nameof(roleId));

        var role = _roles.FirstOrDefault(r => r.Id == roleId);
        if (role == null)
        {
            throw new InvalidOperationException($"Rola o ID {roleId} nie istnieje w organizacji.");
        }

        var existingMember = _members.FirstOrDefault(m => m.UserId == userId);
        if (existingMember != null)
        {
            existingMember.AssignRole(role);

            RegisterDomainEventAndUpdate(new MemberRoleAssignedEvent(Id, userId, roleId));

            return existingMember;
        }

        var member = Member.Create(userId, Id, role);
        _members.Add(member);

        RegisterDomainEventAndUpdate(new MemberAddedEvent(Id, userId, roleId));

        return member;
    }

    /// <summary>
    /// Checks if the user has access to the organization.
    /// </summary>
    /// <param name="userId">User ID.</param>
    /// <returns>True, if the user has access to the organization; otherwise false.</returns>
    public bool HasAccess(Guid userId)
    {
        // The owner of the organization always has access
        if (OwnerId == userId)
        {
            return true;
        }

        // Check if the user is a member of the organization
        return _members.Any(m => m.UserId == userId);
    }

    /// <summary>
    /// Checks if a role with the given ID exists in the organization.
    /// </summary>
    /// <param name="roleId">Role ID.</param>
    /// <returns>True if the role exists; otherwise false.</returns>
    public bool HasRole(Guid roleId)
    {
        return _roles.Any(r => r.Id == roleId);
    }
}
