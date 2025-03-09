using KeepItUp.MagJob.Identity.Core.Events;
using KeepItUp.MagJob.Identity.Core.SharedKernel;

namespace KeepItUp.MagJob.Identity.Core.UserAggregate;

/// <summary>
/// Reprezentuje użytkownika w systemie.
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// Identyfikator użytkownika w systemie zewnętrznym (Keycloak).
    /// </summary>
    public string ExternalId { get; private set; } = string.Empty;

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

    // Prywatny konstruktor dla EF Core
    private User() { }

    /// <summary>
    /// Tworzy nowego użytkownika.
    /// </summary>
    /// <param name="externalId">Identyfikator użytkownika w systemie zewnętrznym (Keycloak).</param>
    /// <param name="email">Adres e-mail użytkownika.</param>
    /// <param name="firstName">Imię użytkownika.</param>
    /// <param name="lastName">Nazwisko użytkownika.</param>
    /// <returns>Nowy użytkownik.</returns>
    public static User Create(string externalId, string email, string firstName, string lastName)
    {
        Guard.Against.NullOrEmpty(externalId, nameof(externalId));
        Guard.Against.NullOrEmpty(email, nameof(email));
        Guard.Against.NullOrEmpty(firstName, nameof(firstName));
        Guard.Against.NullOrEmpty(lastName, nameof(lastName));

        var user = new User
        {
            ExternalId = externalId,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };

        user.RegisterDomainEvent(new UserCreatedEvent(user.Id, user.ExternalId, user.Email));

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
        
        // Wywołanie metody Update z klasy bazowej
        base.Update();

        RegisterDomainEvent(new UserUpdatedEvent(Id, ExternalId, Email));
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
        
        // Wywołanie metody Update z klasy bazowej
        base.Update();

        RegisterDomainEvent(new UserUpdatedEvent(Id, ExternalId, Email));
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
        
        // Wywołanie metody Update z klasy bazowej
        base.Update();

        RegisterDomainEvent(new UserUpdatedEvent(Id, ExternalId, Email));
    }

    /// <summary>
    /// Dezaktywuje użytkownika.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
        
        // Wywołanie metody Update z klasy bazowej
        base.Update();

        RegisterDomainEvent(new UserDeactivatedEvent(Id, ExternalId, Email));
    }

    /// <summary>
    /// Aktywuje użytkownika.
    /// </summary>
    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;
        
        // Wywołanie metody Update z klasy bazowej
        base.Update();

        RegisterDomainEvent(new UserActivatedEvent(Id, ExternalId, Email));
    }
} 
