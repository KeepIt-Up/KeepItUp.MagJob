using KeepItUp.MagJob.SharedKernel;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Events;

namespace KeepItUp.MagJob.Identity.Core.UserAggregate;

/// <summary>
/// Reprezentuje użytkownika w systemie.
/// </summary>
public class User : BaseEntity, IAggregateRoot
{
    /// <summary>
    /// Identyfikator użytkownika w systemie zewnętrznym (Keycloak).
    /// </summary>
    public Guid ExternalId { get; private set; }

    /// <summary>
    /// Adres e-mail użytkownika.
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Imię użytkownika.
    /// </summary>
    public string FirstName { get; private set; } = string.Empty;

    /// <summary>
    /// Nazwisko użytkownika.
    /// </summary>
    public string LastName { get; private set; } = string.Empty;

    /// <summary>
    /// Profil użytkownika.
    /// </summary>
    public UserProfile? Profile { get; private set; }

    /// <summary>
    /// Czy użytkownik jest aktywny.
    /// </summary>
    public bool IsActive { get; private set; } = true;

    /// <summary>
    /// Nazwa użytkownika.
    /// </summary>
    public string Username { get; private set; } = string.Empty;

    /// <summary>
    /// Lista uprawnień użytkownika.
    /// </summary>
    private readonly List<string> _permissions = new();

    /// <summary>
    /// Lista uprawnień użytkownika (tylko do odczytu).
    /// </summary>
    public IReadOnlyCollection<string> Permissions => _permissions.AsReadOnly();

    /// <summary>
    /// Data ostatniego logowania użytkownika.
    /// </summary>
    public DateTime LastLoginDate { get; private set; } = DateTime.MinValue;

    /// <summary>
    /// Prywatny konstruktor dla EF Core oraz tworzenia przez fabrykę.
    /// </summary>
    private User() { }

    /// <summary>
    /// Tworzy nowego użytkownika.
    /// </summary>
    /// <param name="firstName">Imię</param>
    /// <param name="lastName">Nazwisko</param>
    /// <param name="email">Adres e-mail</param>
    /// <param name="username">Nazwa użytkownika</param>
    /// <param name="externalId">Identyfikator zewnętrzny</param>
    /// <param name="isActive">Czy użytkownik jest aktywny</param>
    /// <returns>Nowy użytkownik</returns>
    public static User Create(string firstName, string lastName, string email, string username, Guid externalId, bool isActive = true)
    {
        Guard.Against.NullOrEmpty(firstName, nameof(firstName));
        Guard.Against.NullOrEmpty(lastName, nameof(lastName));
        Guard.Against.NullOrEmpty(email, nameof(email));
        Guard.Against.NullOrEmpty(username, nameof(username));
        Guard.Against.Default(externalId, nameof(externalId));

        var user = new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Username = username,
            ExternalId = externalId,
            IsActive = isActive,
        };

        user.RegisterDomainEventAndUpdate(new UserCreatedEvent(user.Id, user.ExternalId, user.Email));

        return user;
    }

    /// <summary>
    /// Aktualizuje dane użytkownika.
    /// </summary>
    /// <param name="firstName">Imię użytkownika.</param>
    /// <param name="lastName">Nazwisko użytkownika.</param>
    public void Update(string firstName, string lastName)
    {
        Guard.Against.NullOrEmpty(firstName, nameof(firstName));
        Guard.Against.NullOrEmpty(lastName, nameof(lastName));

        FirstName = firstName;
        LastName = lastName;

        RegisterDomainEventAndUpdate(new UserUpdatedEvent(Id, ExternalId, Email));
    }

    /// <summary>
    /// Aktualizuje profil użytkownika.
    /// </summary>
    /// <param name="phoneNumber">Numer telefonu.</param>
    /// <param name="address">Adres.</param>
    /// <param name="profileImage">URL do zdjęcia profilowego.</param>
    public void UpdateProfile(string? phoneNumber, string? address, string? profileImage)
    {
        Profile = new UserProfile(phoneNumber, address, profileImage);

        RegisterDomainEventAndUpdate(new UserUpdatedEvent(Id, ExternalId, Email));
    }

    /// <summary>
    /// Aktualizuje wybrane właściwości profilu użytkownika.
    /// </summary>
    /// <param name="phoneNumber">Nowy numer telefonu lub null, aby zachować obecny.</param>
    /// <param name="address">Nowy adres lub null, aby zachować obecny.</param>
    /// <param name="profileImage">Nowy URL do zdjęcia profilowego lub null, aby zachować obecny.</param>
    public void UpdateProfileProperties(string? phoneNumber = null, string? address = null, string? profileImage = null)
    {
        // Jeśli profil nie istnieje, tworzymy nowy
        if (Profile is null)
        {
            Profile = new UserProfile(phoneNumber, address, profileImage);
        }
        else
        {
            // W przeciwnym razie tworzymy nowy obiekt z zaktualizowanymi właściwościami
            Profile = Profile.WithUpdates(phoneNumber, address, profileImage);
        }

        RegisterDomainEventAndUpdate(new UserUpdatedEvent(Id, ExternalId, Email));
    }

    /// <summary>
    /// Aktualizuje uprawnienia użytkownika.
    /// </summary>
    /// <param name="permissions">Lista uprawnień.</param>
    public void UpdatePermissions(List<string> permissions)
    {
        _permissions.Clear();
        if (permissions != null)
        {
            _permissions.AddRange(permissions);
        }

        RegisterDomainEventAndUpdate(new UserPermissionsUpdatedEvent(Id, ExternalId, Email));
    }

    /// <summary>
    /// Dodaje uprawnienie użytkownikowi.
    /// </summary>
    /// <param name="permission">Uprawnienie do dodania.</param>
    /// <returns>True, jeśli uprawnienie zostało dodane; false, jeśli już istnieje.</returns>
    public bool AddPermission(string permission)
    {
        Guard.Against.NullOrEmpty(permission, nameof(permission));

        if (_permissions.Contains(permission))
        {
            return false;
        }

        _permissions.Add(permission);

        RegisterDomainEventAndUpdate(new UserPermissionsUpdatedEvent(Id, ExternalId, Email));
        return true;
    }

    /// <summary>
    /// Usuwa uprawnienie użytkownikowi.
    /// </summary>
    /// <param name="permission">Uprawnienie do usunięcia.</param>
    /// <returns>True, jeśli uprawnienie zostało usunięte; false, jeśli nie istnieje.</returns>
    public bool RemovePermission(string permission)
    {
        Guard.Against.NullOrEmpty(permission, nameof(permission));

        if (!_permissions.Contains(permission))
        {
            return false;
        }

        _permissions.Remove(permission);

        RegisterDomainEventAndUpdate(new UserPermissionsUpdatedEvent(Id, ExternalId, Email));
        return true;
    }

    /// <summary>
    /// Sprawdza, czy użytkownik posiada określone uprawnienie.
    /// </summary>
    /// <param name="permission">Uprawnienie do sprawdzenia.</param>
    /// <returns>True, jeśli użytkownik posiada uprawnienie; w przeciwnym razie false.</returns>
    public bool HasPermission(string permission)
    {
        Guard.Against.NullOrEmpty(permission, nameof(permission));
        return _permissions.Contains(permission);
    }

    /// <summary>
    /// Aktualizuje datę ostatniego logowania
    /// </summary>
    /// <param name="lastLoginDate">Data ostatniego logowania</param>
    public void UpdateLastLoginDate(DateTime lastLoginDate)
    {
        LastLoginDate = lastLoginDate;
        RegisterDomainEventAndUpdate(new UserLastLoginUpdatedEvent(Id, ExternalId, Email, lastLoginDate));
    }

    /// <summary>
    /// Dezaktywuje użytkownika
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;


        RegisterDomainEventAndUpdate(new UserDeactivatedEvent(Id, ExternalId, Email));
    }

    /// <summary>
    /// Aktywuje użytkownika.
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;


        RegisterDomainEventAndUpdate(new UserActivatedEvent(Id, ExternalId, Email));
    }

    /// <summary>
    /// Aktualizuje adres email i status aktywności użytkownika.
    /// </summary>
    /// <param name="email">Nowy adres email użytkownika.</param>
    /// <param name="isActive">Nowy status aktywności użytkownika.</param>
    public void UpdateEmailAndStatus(string email, bool isActive)
    {
        Guard.Against.NullOrEmpty(email, nameof(email));

        Email = email;

        // Aktualizacja statusu aktywności
        if (IsActive != isActive)
        {
            IsActive = isActive;

            if (isActive)
            {
                RegisterDomainEventAndUpdate(new UserActivatedEvent(Id, ExternalId, Email));
            }
            else
            {
                RegisterDomainEventAndUpdate(new UserDeactivatedEvent(Id, ExternalId, Email));
            }
        }
        else
        {
            RegisterDomainEventAndUpdate(new UserUpdatedEvent(Id, ExternalId, Email));
        }
    }

    /// <summary>
    /// Aktualizuje wszystkie dane użytkownika
    /// </summary>
    /// <param name="firstName">Imię</param>
    /// <param name="lastName">Nazwisko</param>
    /// <param name="email">Adres e-mail</param>
    /// <param name="username">Nazwa użytkownika</param>
    /// <param name="isActive">Czy użytkownik jest aktywny</param>
    public void UpdateAllDetails(string firstName, string lastName, string email, string username, bool isActive)
    {
        Guard.Against.NullOrEmpty(firstName, nameof(firstName));
        Guard.Against.NullOrEmpty(lastName, nameof(lastName));
        Guard.Against.NullOrEmpty(email, nameof(email));
        Guard.Against.NullOrEmpty(username, nameof(username));

        FirstName = firstName;
        LastName = lastName;
        Email = email;
        Username = username;

        // Aktualizacja statusu aktywności
        if (IsActive != isActive)
        {
            IsActive = isActive;

            if (isActive)
            {
                RegisterDomainEventAndUpdate(new UserActivatedEvent(Id, ExternalId, Email));
            }
            else
            {
                RegisterDomainEventAndUpdate(new UserDeactivatedEvent(Id, ExternalId, Email));
            }
        }
        else
        {
            RegisterDomainEventAndUpdate(new UserUpdatedEvent(Id, ExternalId, Email));
        }
    }
}
