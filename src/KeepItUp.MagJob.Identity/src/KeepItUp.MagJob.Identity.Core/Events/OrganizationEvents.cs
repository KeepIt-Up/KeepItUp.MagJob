using Ardalis.SharedKernel;

namespace KeepItUp.MagJob.Identity.Core.Events;

/// <summary>
/// Zdarzenie informujące o utworzeniu nowej organizacji.
/// </summary>
public class OrganizationCreatedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Nazwa organizacji.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Identyfikator właściciela organizacji.
    /// </summary>
    public Guid OwnerId { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie informujące o utworzeniu organizacji.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="name">Nazwa organizacji.</param>
    /// <param name="ownerId">Identyfikator właściciela organizacji.</param>
    public OrganizationCreatedEvent(Guid organizationId, string name, Guid ownerId)
    {
        OrganizationId = organizationId;
        Name = name;
        OwnerId = ownerId;
    }
}

/// <summary>
/// Zdarzenie informujące o aktualizacji danych organizacji.
/// </summary>
public class OrganizationUpdatedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Nazwa organizacji.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Identyfikator właściciela organizacji.
    /// </summary>
    public Guid OwnerId { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie informujące o aktualizacji danych organizacji.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="name">Nazwa organizacji.</param>
    /// <param name="ownerId">Identyfikator właściciela organizacji.</param>
    public OrganizationUpdatedEvent(Guid organizationId, string name, Guid ownerId)
    {
        OrganizationId = organizationId;
        Name = name;
        OwnerId = ownerId;
    }
}

/// <summary>
/// Zdarzenie informujące o dezaktywacji organizacji.
/// </summary>
public class OrganizationDeactivatedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Nazwa organizacji.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Identyfikator właściciela organizacji.
    /// </summary>
    public Guid OwnerId { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie informujące o dezaktywacji organizacji.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="name">Nazwa organizacji.</param>
    /// <param name="ownerId">Identyfikator właściciela organizacji.</param>
    public OrganizationDeactivatedEvent(Guid organizationId, string name, Guid ownerId)
    {
        OrganizationId = organizationId;
        Name = name;
        OwnerId = ownerId;
    }
}

/// <summary>
/// Zdarzenie informujące o aktywacji organizacji.
/// </summary>
public class OrganizationActivatedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Nazwa organizacji.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Identyfikator właściciela organizacji.
    /// </summary>
    public Guid OwnerId { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie informujące o aktywacji organizacji.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="name">Nazwa organizacji.</param>
    /// <param name="ownerId">Identyfikator właściciela organizacji.</param>
    public OrganizationActivatedEvent(Guid organizationId, string name, Guid ownerId)
    {
        OrganizationId = organizationId;
        Name = name;
        OwnerId = ownerId;
    }
}

/// <summary>
/// Zdarzenie informujące o dodaniu nowego członka do organizacji.
/// </summary>
public class MemberAddedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Identyfikator roli.
    /// </summary>
    public Guid RoleId { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie informujące o dodaniu członka do organizacji.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="userId">Identyfikator użytkownika.</param>
    /// <param name="roleId">Identyfikator roli.</param>
    public MemberAddedEvent(Guid organizationId, Guid userId, Guid roleId)
    {
        OrganizationId = organizationId;
        UserId = userId;
        RoleId = roleId;
    }
}

/// <summary>
/// Zdarzenie informujące o usunięciu członka z organizacji.
/// </summary>
public class MemberRemovedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie informujące o usunięciu członka z organizacji.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="userId">Identyfikator użytkownika.</param>
    public MemberRemovedEvent(Guid organizationId, Guid userId)
    {
        OrganizationId = organizationId;
        UserId = userId;
    }
}

/// <summary>
/// Zdarzenie informujące o przypisaniu roli do członka organizacji.
/// </summary>
public class MemberRoleAssignedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Identyfikator roli.
    /// </summary>
    public Guid RoleId { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie informujące o przypisaniu roli do członka organizacji.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="userId">Identyfikator użytkownika.</param>
    /// <param name="roleId">Identyfikator roli.</param>
    public MemberRoleAssignedEvent(Guid organizationId, Guid userId, Guid roleId)
    {
        OrganizationId = organizationId;
        UserId = userId;
        RoleId = roleId;
    }
}

/// <summary>
/// Zdarzenie informujące o usunięciu roli przypisanej do członka organizacji.
/// </summary>
public class MemberRoleRevokedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Identyfikator roli.
    /// </summary>
    public Guid RoleId { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie informujące o usunięciu roli przypisanej do członka organizacji.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="userId">Identyfikator użytkownika.</param>
    /// <param name="roleId">Identyfikator roli.</param>
    public MemberRoleRevokedEvent(Guid organizationId, Guid userId, Guid roleId)
    {
        OrganizationId = organizationId;
        UserId = userId;
        RoleId = roleId;
    }
}

/// <summary>
/// Zdarzenie informujące o utworzeniu nowej roli w organizacji.
/// </summary>
public class RoleCreatedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Identyfikator roli.
    /// </summary>
    public Guid RoleId { get; }

    /// <summary>
    /// Nazwa roli.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie informujące o utworzeniu roli w organizacji.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="roleId">Identyfikator roli.</param>
    /// <param name="name">Nazwa roli.</param>
    public RoleCreatedEvent(Guid organizationId, Guid roleId, string name)
    {
        OrganizationId = organizationId;
        RoleId = roleId;
        Name = name;
    }
}

/// <summary>
/// Zdarzenie informujące o aktualizacji roli w organizacji.
/// </summary>
public class RoleUpdatedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Identyfikator roli.
    /// </summary>
    public Guid RoleId { get; }

    /// <summary>
    /// Nazwa roli.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie informujące o aktualizacji roli w organizacji.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="roleId">Identyfikator roli.</param>
    /// <param name="name">Nazwa roli.</param>
    public RoleUpdatedEvent(Guid organizationId, Guid roleId, string name)
    {
        OrganizationId = organizationId;
        RoleId = roleId;
        Name = name;
    }
}

/// <summary>
/// Zdarzenie informujące o usunięciu roli z organizacji.
/// </summary>
public class RoleDeletedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Identyfikator roli.
    /// </summary>
    public Guid RoleId { get; }

    /// <summary>
    /// Nazwa roli.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie informujące o usunięciu roli z organizacji.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="roleId">Identyfikator roli.</param>
    /// <param name="name">Nazwa roli.</param>
    public RoleDeletedEvent(Guid organizationId, Guid roleId, string name)
    {
        OrganizationId = organizationId;
        RoleId = roleId;
        Name = name;
    }
}

/// <summary>
/// Zdarzenie informujące o utworzeniu nowego zaproszenia do organizacji.
/// </summary>
public class InvitationCreatedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Identyfikator zaproszenia.
    /// </summary>
    public Guid InvitationId { get; }

    /// <summary>
    /// Adres e-mail osoby zapraszanej.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie informujące o utworzeniu zaproszenia do organizacji.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="invitationId">Identyfikator zaproszenia.</param>
    /// <param name="email">Adres e-mail osoby zapraszanej.</param>
    public InvitationCreatedEvent(Guid organizationId, Guid invitationId, string email)
    {
        OrganizationId = organizationId;
        InvitationId = invitationId;
        Email = email;
    }
}

/// <summary>
/// Zdarzenie informujące o akceptacji zaproszenia do organizacji.
/// </summary>
public class InvitationAcceptedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Identyfikator zaproszenia.
    /// </summary>
    public Guid InvitationId { get; }

    /// <summary>
    /// Identyfikator użytkownika, który zaakceptował zaproszenie.
    /// </summary>
    public Guid UserId { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie informujące o akceptacji zaproszenia do organizacji.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="invitationId">Identyfikator zaproszenia.</param>
    /// <param name="userId">Identyfikator użytkownika, który zaakceptował zaproszenie.</param>
    public InvitationAcceptedEvent(Guid organizationId, Guid invitationId, Guid userId)
    {
        OrganizationId = organizationId;
        InvitationId = invitationId;
        UserId = userId;
    }
}

/// <summary>
/// Zdarzenie informujące o odrzuceniu zaproszenia do organizacji.
/// </summary>
public class InvitationRejectedEvent : DomainEventBase
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; }

    /// <summary>
    /// Identyfikator zaproszenia.
    /// </summary>
    public Guid InvitationId { get; }

    /// <summary>
    /// Tworzy nowe zdarzenie informujące o odrzuceniu zaproszenia do organizacji.
    /// </summary>
    /// <param name="organizationId">Identyfikator organizacji.</param>
    /// <param name="invitationId">Identyfikator zaproszenia.</param>
    public InvitationRejectedEvent(Guid organizationId, Guid invitationId)
    {
        OrganizationId = organizationId;
        InvitationId = invitationId;
    }
} 
