namespace KeepItUp.MagJob.Identity.Web.Endpoints.Users;

/// <summary>
/// Odpowiedź dla endpointu GetUserEndpoint.
/// </summary>
public class GetUserResponse
{
    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Zewnętrzny identyfikator użytkownika (np. z Keycloak).
    /// </summary>
    public string ExternalId { get; set; } = string.Empty;

    /// <summary>
    /// Adres email użytkownika.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Imię użytkownika.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Nazwisko użytkownika.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Czy użytkownik jest aktywny.
    /// </summary>
    public bool IsActive { get; set; }
} 
