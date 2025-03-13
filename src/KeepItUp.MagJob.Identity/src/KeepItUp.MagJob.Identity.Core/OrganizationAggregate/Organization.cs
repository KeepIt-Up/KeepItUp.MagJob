using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Events;
using KeepItUp.MagJob.Identity.Core.SharedKernel;

namespace KeepItUp.MagJob.Identity.Core.OrganizationAggregate;

/// <summary>
/// Reprezentuje organizację w systemie.
/// </summary>
public class Organization : BaseEntity, IAggregateRoot
{
  /// <summary>
  /// Nazwa organizacji.
  /// </summary>
  public string Name { get; private set; } = string.Empty;

  /// <summary>
  /// Opis organizacji.
  /// </summary>
  public string? Description { get; private set; }

  /// <summary>
  /// Identyfikator właściciela organizacji.
  /// </summary>
  public Guid OwnerId { get; private set; }

  /// <summary>
  /// Czy organizacja jest aktywna.
  /// </summary>
  public bool IsActive { get; private set; } = true;

  /// <summary>
  /// Lista członków organizacji.
  /// </summary>
  private readonly List<Member> _members = new();

  /// <summary>
  /// Lista członków organizacji (tylko do odczytu).
  /// </summary>
  public IReadOnlyCollection<Member> Members => _members.AsReadOnly();

  /// <summary>
  /// Lista ról w organizacji.
  /// </summary>
  private readonly List<Role> _roles = new();

  /// <summary>
  /// Lista ról w organizacji (tylko do odczytu).
  /// </summary>
  public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();

  /// <summary>
  /// Lista zaproszeń do organizacji.
  /// </summary>
  private readonly List<Invitation> _invitations = new();

  /// <summary>
  /// Lista zaproszeń do organizacji (tylko do odczytu).
  /// </summary>
  public IReadOnlyCollection<Invitation> Invitations => _invitations.AsReadOnly();

  // Prywatny konstruktor dla EF Core
  private Organization() { }

  /// <summary>
  /// Tworzy nową organizację.
  /// </summary>
  /// <param name="name">Nazwa organizacji.</param>
  /// <param name="ownerId">Identyfikator właściciela organizacji.</param>
  /// <param name="description">Opis organizacji.</param>
  /// <returns>Nowa organizacja.</returns>
  public static Organization Create(string name, Guid ownerId, string? description = null)
  {
    Guard.Against.NullOrEmpty(name, nameof(name));
    Guard.Against.Default(ownerId, nameof(ownerId));

    var organization = new Organization
    {
      Name = name,
      OwnerId = ownerId,
      Description = description
    };

    // Dodaj domyślne role
    var adminRole = Role.Create("Admin", organization.Id, "Administrator organizacji", "#FF0000");
    var memberRole = Role.Create("Member", organization.Id, "Członek organizacji", "#00FF00");
    var guestRole = Role.Create("Guest", organization.Id, "Gość organizacji", "#0000FF");

    organization._roles.Add(adminRole);
    organization._roles.Add(memberRole);
    organization._roles.Add(guestRole);

    // Dodaj właściciela jako administratora
    var ownerMember = Member.Create(ownerId, organization.Id, adminRole.Id);
    organization._members.Add(ownerMember);

    organization.RegisterDomainEvent(new OrganizationCreatedEvent(organization.Id, organization.Name, organization.OwnerId));

    return organization;
  }

  /// <summary>
  /// Aktualizuje dane organizacji.
  /// </summary>
  /// <param name="name">Nazwa organizacji.</param>
  /// <param name="description">Opis organizacji.</param>
  public void Update(string name, string? description)
  {
    Guard.Against.NullOrEmpty(name, nameof(name));

    Name = name;
    Description = description;

    // Wywołanie metody Update z klasy bazowej
    base.Update();

    RegisterDomainEvent(new OrganizationUpdatedEvent(Id, Name, OwnerId));
  }

  /// <summary>
  /// Dezaktywuje organizację.
  /// </summary>
  public void Deactivate()
  {
    if (!IsActive)
      return;

    IsActive = false;

    // Wywołanie metody Update z klasy bazowej
    base.Update();

    RegisterDomainEvent(new OrganizationDeactivatedEvent(Id, Name, OwnerId));
  }

  /// <summary>
  /// Aktywuje organizację.
  /// </summary>
  public void Activate()
  {
    if (IsActive)
      return;

    IsActive = true;

    // Wywołanie metody Update z klasy bazowej
    base.Update();

    RegisterDomainEvent(new OrganizationActivatedEvent(Id, Name, OwnerId));
  }

  /// <summary>
  /// Dodaje nowego członka do organizacji.
  /// </summary>
  /// <param name="userId">Identyfikator użytkownika.</param>
  /// <param name="roleId">Identyfikator roli.</param>
  /// <returns>Nowy członek organizacji.</returns>
  public Member AddMember(Guid userId, Guid roleId)
  {
    Guard.Against.Default(userId, nameof(userId));
    Guard.Against.Default(roleId, nameof(roleId));

    // Sprawdź, czy użytkownik już jest członkiem organizacji
    var existingMember = _members.FirstOrDefault(m => m.UserId == userId);
    if (existingMember != null)
    {
      // Jeśli użytkownik już jest członkiem, dodaj mu nową rolę
      existingMember.AssignRole(roleId);

      // Wywołanie metody Update z klasy bazowej
      base.Update();

      RegisterDomainEvent(new MemberRoleAssignedEvent(Id, userId, roleId));
      return existingMember;
    }

    // Sprawdź, czy rola istnieje w organizacji
    if (!_roles.Any(r => r.Id == roleId))
    {
      throw new InvalidOperationException($"Rola o ID {roleId} nie istnieje w organizacji.");
    }

    var member = Member.Create(userId, Id, roleId);
    _members.Add(member);

    // Wywołanie metody Update z klasy bazowej
    base.Update();

    RegisterDomainEvent(new MemberAddedEvent(Id, userId, roleId));

    return member;
  }

  /// <summary>
  /// Usuwa członka z organizacji.
  /// </summary>
  /// <param name="userId">Identyfikator użytkownika.</param>
  public void RemoveMember(Guid userId)
  {
    Guard.Against.Default(userId, nameof(userId));

    // Sprawdź, czy użytkownik jest właścicielem organizacji
    if (userId == OwnerId)
    {
      throw new InvalidOperationException("Nie można usunąć właściciela organizacji.");
    }

    // Znajdź członka organizacji
    var member = _members.FirstOrDefault(m => m.UserId == userId);
    if (member == null)
    {
      throw new InvalidOperationException($"Użytkownik o ID {userId} nie jest członkiem organizacji.");
    }

    _members.Remove(member);

    // Wywołanie metody Update z klasy bazowej
    base.Update();

    RegisterDomainEvent(new MemberRemovedEvent(Id, userId));
  }

  /// <summary>
  /// Przypisuje rolę członkowi organizacji.
  /// </summary>
  /// <param name="userId">Identyfikator użytkownika.</param>
  /// <param name="roleId">Identyfikator roli.</param>
  public void AssignRoleToMember(Guid userId, Guid roleId)
  {
    Guard.Against.Default(userId, nameof(userId));
    Guard.Against.Default(roleId, nameof(roleId));

    // Sprawdź, czy rola istnieje w organizacji
    if (!_roles.Any(r => r.Id == roleId))
    {
      throw new InvalidOperationException($"Rola o ID {roleId} nie istnieje w organizacji.");
    }

    // Znajdź członka organizacji
    var member = _members.FirstOrDefault(m => m.UserId == userId);
    if (member == null)
    {
      throw new InvalidOperationException($"Użytkownik o ID {userId} nie jest członkiem organizacji.");
    }

    member.AssignRole(roleId);

    // Wywołanie metody Update z klasy bazowej
    base.Update();

    RegisterDomainEvent(new MemberRoleAssignedEvent(Id, userId, roleId));
  }

  /// <summary>
  /// Usuwa rolę przypisaną do członka organizacji.
  /// </summary>
  /// <param name="userId">Identyfikator użytkownika.</param>
  /// <param name="roleId">Identyfikator roli.</param>
  public void RevokeRoleFromMember(Guid userId, Guid roleId)
  {
    Guard.Against.Default(userId, nameof(userId));
    Guard.Against.Default(roleId, nameof(roleId));

    // Znajdź członka organizacji
    var member = _members.FirstOrDefault(m => m.UserId == userId);
    if (member == null)
    {
      throw new InvalidOperationException($"Użytkownik o ID {userId} nie jest członkiem organizacji.");
    }

    // Sprawdź, czy członek posiada tę rolę
    if (!member.HasRole(roleId))
    {
      throw new InvalidOperationException($"Użytkownik o ID {userId} nie posiada roli o ID {roleId}.");
    }

    // Usuń rolę
    if (!member.RemoveRole(roleId))
    {
      throw new InvalidOperationException("Nie można usunąć ostatniej roli przypisanej do członka organizacji.");
    }

    // Wywołanie metody Update z klasy bazowej
    base.Update();

    RegisterDomainEvent(new MemberRoleRevokedEvent(Id, userId, roleId));
  }

  /// <summary>
  /// Dodaje nową rolę do organizacji.
  /// </summary>
  /// <param name="name">Nazwa roli.</param>
  /// <param name="description">Opis roli.</param>
  /// <param name="color">Kolor roli (w formacie HEX).</param>
  /// <returns>Nowa rola.</returns>
  public Role AddRole(string name, string? description = null, string? color = null)
  {
    Guard.Against.NullOrEmpty(name, nameof(name));

    // Sprawdź, czy rola o takiej nazwie już istnieje
    if (_roles.Any(r => r.Name == name))
    {
      throw new InvalidOperationException($"Rola o nazwie {name} już istnieje w organizacji.");
    }

    var role = Role.Create(name, Id, description, color);
    _roles.Add(role);

    // Wywołanie metody Update z klasy bazowej
    base.Update();

    RegisterDomainEvent(new RoleCreatedEvent(Id, role.Id, role.Name));

    return role;
  }

  /// <summary>
  /// Usuwa rolę z organizacji.
  /// </summary>
  /// <param name="roleId">Identyfikator roli.</param>
  public void RemoveRole(Guid roleId)
  {
    Guard.Against.Default(roleId, nameof(roleId));

    // Znajdź rolę
    var role = _roles.FirstOrDefault(r => r.Id == roleId);
    if (role == null)
    {
      throw new InvalidOperationException($"Rola o ID {roleId} nie istnieje w organizacji.");
    }

    // Sprawdź, czy rola jest używana przez członków organizacji
    if (_members.Any(m => m.HasRole(roleId)))
    {
      throw new InvalidOperationException("Nie można usunąć roli, która jest przypisana do członków organizacji.");
    }

    _roles.Remove(role);

    // Wywołanie metody Update z klasy bazowej
    base.Update();

    RegisterDomainEvent(new RoleDeletedEvent(Id, roleId, role.Name));
  }

  /// <summary>
  /// Tworzy nowe zaproszenie do organizacji.
  /// </summary>
  /// <param name="email">Adres e-mail osoby zapraszanej.</param>
  /// <param name="roleId">Identyfikator roli, która zostanie przypisana po akceptacji zaproszenia.</param>
  /// <param name="expiresAt">Data wygaśnięcia zaproszenia.</param>
  /// <returns>Nowe zaproszenie.</returns>
  public Invitation CreateInvitation(string email, Guid roleId, DateTime? expiresAt = null)
  {
    Guard.Against.NullOrEmpty(email, nameof(email));
    Guard.Against.Default(roleId, nameof(roleId));

    // Sprawdź, czy rola istnieje w organizacji
    if (!_roles.Any(r => r.Id == roleId))
    {
      throw new InvalidOperationException($"Rola o ID {roleId} nie istnieje w organizacji.");
    }

    // Sprawdź, czy zaproszenie dla tego adresu e-mail już istnieje
    if (_invitations.Any(i => i.Email == email && !i.IsExpired))
    {
      throw new InvalidOperationException($"Zaproszenie dla adresu e-mail {email} już istnieje.");
    }

    var invitation = Invitation.Create(Id, email, roleId, expiresAt);
    _invitations.Add(invitation);

    // Wywołanie metody Update z klasy bazowej
    base.Update();

    RegisterDomainEvent(new InvitationCreatedEvent(Id, invitation.Id, email));

    return invitation;
  }

  /// <summary>
  /// Akceptuje zaproszenie do organizacji.
  /// </summary>
  /// <param name="invitationId">Identyfikator zaproszenia.</param>
  /// <param name="userId">Identyfikator użytkownika akceptującego zaproszenie.</param>
  /// <returns>Nowy członek organizacji.</returns>
  public Member AcceptInvitation(Guid invitationId, Guid userId)
  {
    Guard.Against.Default(invitationId, nameof(invitationId));
    Guard.Against.Default(userId, nameof(userId));

    // Znajdź zaproszenie
    var invitation = _invitations.FirstOrDefault(i => i.Id == invitationId);
    if (invitation == null)
    {
      throw new InvalidOperationException($"Zaproszenie o ID {invitationId} nie istnieje.");
    }

    // Sprawdź, czy zaproszenie nie wygasło
    if (invitation.IsExpired)
    {
      throw new InvalidOperationException("Zaproszenie wygasło.");
    }

    // Sprawdź, czy użytkownik już jest członkiem organizacji
    var existingMember = _members.FirstOrDefault(m => m.UserId == userId);
    if (existingMember != null)
    {
      // Jeśli użytkownik już jest członkiem, dodaj mu nową rolę
      existingMember.AssignRole(invitation.RoleId);

      // Akceptuj zaproszenie
      invitation.Accept();

      // Wywołanie metody Update z klasy bazowej
      base.Update();

      RegisterDomainEvent(new InvitationAcceptedEvent(Id, invitationId, userId));
      RegisterDomainEvent(new MemberRoleAssignedEvent(Id, userId, invitation.RoleId));

      return existingMember;
    }

    // Akceptuj zaproszenie
    invitation.Accept();

    // Dodaj nowego członka
    var member = Member.Create(userId, Id, invitation.RoleId);
    _members.Add(member);

    // Wywołanie metody Update z klasy bazowej
    base.Update();

    RegisterDomainEvent(new InvitationAcceptedEvent(Id, invitationId, userId));
    RegisterDomainEvent(new MemberAddedEvent(Id, userId, invitation.RoleId));

    return member;
  }

  /// <summary>
  /// Odrzuca zaproszenie do organizacji.
  /// </summary>
  /// <param name="invitationId">Identyfikator zaproszenia.</param>
  public void RejectInvitation(Guid invitationId)
  {
    Guard.Against.Default(invitationId, nameof(invitationId));

    // Znajdź zaproszenie
    var invitation = _invitations.FirstOrDefault(i => i.Id == invitationId);
    if (invitation == null)
    {
      throw new InvalidOperationException($"Zaproszenie o ID {invitationId} nie istnieje.");
    }

    // Sprawdź, czy zaproszenie nie wygasło
    if (invitation.IsExpired)
    {
      throw new InvalidOperationException("Zaproszenie wygasło.");
    }

    // Odrzuć zaproszenie
    invitation.Reject();

    // Wywołanie metody Update z klasy bazowej
    base.Update();

    RegisterDomainEvent(new InvitationRejectedEvent(Id, invitationId));
  }

  /// <summary>
  /// Sprawdza, czy użytkownik ma dostęp do organizacji.
  /// </summary>
  /// <param name="userId">Identyfikator użytkownika.</param>
  /// <returns>True, jeśli użytkownik ma dostęp do organizacji; w przeciwnym razie false.</returns>
  public bool HasAccess(Guid userId)
  {
    // Właściciel organizacji zawsze ma dostęp
    if (OwnerId == userId)
    {
      return true;
    }

    // Sprawdź, czy użytkownik jest członkiem organizacji
    return _members.Any(m => m.UserId == userId);
  }
}
