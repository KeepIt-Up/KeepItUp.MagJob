namespace KeepItUp.MagJob.Identity.Web.Endpoints.Users;

/// <summary>
/// Żądanie dla endpointu UpdateUserEndpoint.
/// </summary>
public class UpdateUserRequest
{
    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid Id { get; set; }

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
} 
