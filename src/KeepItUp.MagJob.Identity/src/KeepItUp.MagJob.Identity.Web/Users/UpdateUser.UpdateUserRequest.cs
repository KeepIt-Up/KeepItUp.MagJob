namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Żądanie dla endpointu UpdateUserEndpoint.
/// </summary>
public class UpdateUserRequest
{
    /// <summary>
    /// Szablon ścieżki URL dla endpointu aktualizacji użytkownika.
    /// </summary>
    public const string Route = "/Users/{Id:guid}";

    /// <summary>
    /// Buduje ścieżkę URL dla określonego identyfikatora użytkownika.
    /// </summary>
    /// <param name="id">Identyfikator użytkownika.</param>
    /// <returns>Ścieżka URL z uwzględnionym identyfikatorem.</returns>
    public static string BuildRoute(Guid id) => Route.Replace("{Id:guid}", id.ToString());

    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Imię użytkownika.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Nazwisko użytkownika.
    /// </summary>
    public string LastName { get; set; } = string.Empty;
}
