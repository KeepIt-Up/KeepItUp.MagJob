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

    /// <summary>
    /// List of invitations to the organization.
    /// </summary>
    private readonly List<Invitation> _invitations = new();

    /// <summary>
    /// List of invitations to the organization (read-only).
    /// </summary>
    public IReadOnlyCollection<Invitation> Invitations => _invitations.AsReadOnly();

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
        var ownerMember = Member.Create(OwnerId, Id, adminRole.Id);

        // Explicitly set references for navigation between Member and Role
        ownerMember.Roles.Add(adminRole);

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

        // Call the Update method from the base class
        base.Update();

        RegisterDomainEvent(new OrganizationUpdatedEvent(Id, Name, OwnerId));
    }

    /// <summary>
    /// Updates the organization's logo.
    /// </summary>
    /// <param name="logoUrl">URL of the organization's logo.</param>
    public void UpdateLogo(string? logoUrl)
    {
        LogoUrl = logoUrl;

        // Call the Update method from the base class
        base.Update();

        RegisterDomainEvent(new OrganizationLogoUpdatedEvent(Id, LogoUrl));
    }

    /// <summary>
    /// Updates the organization's banner.
    /// </summary>
    /// <param name="bannerUrl">URL of the organization's banner.</param>
    public void UpdateBanner(string? bannerUrl)
    {
        BannerUrl = bannerUrl;

        // Call the Update method from the base class
        base.Update();

        RegisterDomainEvent(new OrganizationBannerUpdatedEvent(Id, BannerUrl));
    }

    /// <summary>
    /// Deactivates the organization.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;

        // Call the Update method from the base class
        base.Update();

        RegisterDomainEvent(new OrganizationDeactivatedEvent(Id, Name, OwnerId));
    }

    /// <summary>
    /// Activates the organization.
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;

        // Call the Update method from the base class
        base.Update();

        RegisterDomainEvent(new OrganizationActivatedEvent(Id, Name, OwnerId));
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

        // Check if the role exists in the organization
        var role = _roles.FirstOrDefault(r => r.Id == roleId);
        if (role == null)
        {
            throw new InvalidOperationException($"Rola o ID {roleId} nie istnieje w organizacji.");
        }

        // Check if the user is already a member of the organization
        var existingMember = _members.FirstOrDefault(m => m.UserId == userId);
        if (existingMember != null)
        {
            // If the user is already a member, add a new role to them
            existingMember.AssignRole(roleId, role);

            // Call the Update method from the base class
            base.Update();

            RegisterDomainEvent(new MemberRoleAssignedEvent(Id, userId, roleId));
            return existingMember;
        }

        var member = Member.Create(userId, Id, roleId);

        // Add reference to the role object in the member
        member.SyncRoles(_roles);

        _members.Add(member);

        // Call the Update method from the base class
        base.Update();

        RegisterDomainEvent(new MemberAddedEvent(Id, userId, roleId));

        return member;
    }

    /// <summary>
    /// Removes a member from the organization.
    /// </summary>
    /// <param name="userId">User ID.</param>
    public void RemoveMember(Guid userId)
    {
        Guard.Against.Default(userId, nameof(userId));

        // Check if the user is the owner of the organization
        if (userId == OwnerId)
        {
            throw new InvalidOperationException("Nie można usunąć właściciela organizacji.");
        }

        // Find the member of the organization
        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member == null)
        {
            throw new InvalidOperationException($"Użytkownik o ID {userId} nie jest członkiem organizacji.");
        }

        _members.Remove(member);

        // Call the Update method from the base class
        base.Update();

        RegisterDomainEvent(new MemberRemovedEvent(Id, userId));
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

        // Check if the role exists in the organization
        if (!_roles.Any(r => r.Id == roleId))
        {
            throw new InvalidOperationException($"Rola o ID {roleId} nie istnieje w organizacji.");
        }

        // Find the member of the organization
        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member == null)
        {
            throw new InvalidOperationException($"Użytkownik o ID {userId} nie jest członkiem organizacji.");
        }

        member.AssignRole(roleId);

        // Call the Update method from the base class
        base.Update();

        RegisterDomainEvent(new MemberRoleAssignedEvent(Id, userId, roleId));
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

        // Find the member of the organization
        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member == null)
        {
            throw new InvalidOperationException($"Użytkownik o ID {userId} nie jest członkiem organizacji.");
        }

        // Check if the member has the role
        if (!member.HasRole(roleId))
        {
            throw new InvalidOperationException($"Użytkownik o ID {userId} nie posiada roli o ID {roleId}.");
        }

        // Remove the role
        if (!member.RemoveRole(roleId))
        {
            throw new InvalidOperationException("Nie można usunąć ostatniej roli przypisanej do członka organizacji.");
        }

        // Call the Update method from the base class
        base.Update();

        RegisterDomainEvent(new MemberRoleRevokedEvent(Id, userId, roleId));
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

        // Update the entity version
        Update();

        // Register the domain event
        RegisterDomainEvent(new RoleCreatedEvent(Id, role.Id, role.Name));

        return role;
    }

    /// <summary>
    /// Removes a role from the organization.
    /// </summary>
    /// <param name="roleId">Role ID.</param>
    public void RemoveRole(Guid roleId)
    {
        Guard.Against.Default(roleId, nameof(roleId));

        // Find the role
        var role = _roles.FirstOrDefault(r => r.Id == roleId);
        if (role == null)
        {
            throw new InvalidOperationException($"Rola o ID {roleId} nie istnieje w organizacji.");
        }

        // Check if the role is used by members of the organization
        if (_members.Any(m => m.HasRole(roleId)))
        {
            throw new InvalidOperationException("Nie można usunąć roli, która jest przypisana do członków organizacji.");
        }

        _roles.Remove(role);

        // Call the Update method from the base class
        base.Update();

        RegisterDomainEvent(new RoleDeletedEvent(Id, roleId, role.Name));
    }

    /// <summary>
    /// Creates a new invitation to the organization.
    /// </summary>
    /// <param name="email">Email address of the invited person.</param>
    /// <param name="roleId">Role ID that will be assigned after the invitation is accepted.</param>
    /// <param name="expiresAt">Expiration date of the invitation.</param>
    /// <returns>New invitation.</returns>
    public Invitation CreateInvitation(string email, Guid roleId, DateTime? expiresAt = null)
    {
        Guard.Against.NullOrEmpty(email, nameof(email));
        Guard.Against.Default(roleId, nameof(roleId));

        // Check if the role exists in the organization
        if (!_roles.Any(r => r.Id == roleId))
        {
            throw new InvalidOperationException($"Rola o ID {roleId} nie istnieje w organizacji.");
        }

        // Check if an invitation for this email address already exists
        if (_invitations.Any(i => i.Email == email && !i.IsExpired))
        {
            throw new InvalidOperationException($"Zaproszenie dla adresu e-mail {email} już istnieje.");
        }

        var invitation = Invitation.Create(Id, email, roleId, expiresAt);
        _invitations.Add(invitation);

        RegisterDomainEventAndUpdate(new InvitationCreatedEvent(Id, invitation.Id, email, roleId));

        return invitation;
    }

    /// <summary>
    /// Accepts an invitation to the organization.
    /// </summary>
    /// <param name="invitationId">Invitation ID.</param>
    /// <param name="userId">User ID of the person accepting the invitation.</param>
    /// <returns>New member.</returns>
    public Member AcceptInvitation(Guid invitationId, Guid userId)
    {
        Guard.Against.Default(invitationId, nameof(invitationId));
        Guard.Against.Default(userId, nameof(userId));

        // Find the invitation
        var invitation = _invitations.FirstOrDefault(i => i.Id == invitationId);
        if (invitation == null)
        {
            throw new InvalidOperationException($"Zaproszenie o ID {invitationId} nie istnieje.");
        }

        // Check if the invitation has expired
        if (invitation.IsExpired)
        {
            throw new InvalidOperationException("Zaproszenie wygasło.");
        }

        // Check if the user is already a member of the organization
        var existingMember = _members.FirstOrDefault(m => m.UserId == userId);
        if (existingMember != null)
        {
            // If the user is already a member, add a new role to them
            existingMember.AssignRole(invitation.RoleId);

            // Accept the invitation
            invitation.Accept();

            // Call the Update method from the base class
            base.Update();

            RegisterDomainEventAndUpdate(new InvitationAcceptedEvent(invitationId, Id, invitation.Email, invitation.RoleId));
            RegisterDomainEventAndUpdate(new MemberRoleAssignedEvent(Id, userId, invitation.RoleId));

            return existingMember;
        }

        // Accept the invitation
        invitation.Accept();

        // Add a new member
        var member = Member.Create(userId, Id, invitation.RoleId);
        _members.Add(member);

        // Call the Update method from the base class
        base.Update();

        RegisterDomainEventAndUpdate(new InvitationAcceptedEvent(invitationId, Id, invitation.Email, invitation.RoleId));
        RegisterDomainEventAndUpdate(new MemberAddedEvent(Id, userId, invitation.RoleId));

        return member;
    }

    /// <summary>
    /// Rejects an invitation to the organization.
    /// </summary>
    /// <param name="invitationId">Invitation ID.</param>
    public void RejectInvitation(Guid invitationId)
    {
        Guard.Against.Default(invitationId, nameof(invitationId));

        // Find the invitation
        var invitation = _invitations.FirstOrDefault(i => i.Id == invitationId);
        if (invitation == null)
        {
            throw new InvalidOperationException($"Zaproszenie o ID {invitationId} nie istnieje.");
        }

        // Check if the invitation has expired
        if (invitation.IsExpired)
        {
            throw new InvalidOperationException("Zaproszenie wygasło.");
        }

        // Reject the invitation
        invitation.Reject();

        RegisterDomainEventAndUpdate(new InvitationRejectedEvent(invitationId, Id, invitation.Email));
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
}
