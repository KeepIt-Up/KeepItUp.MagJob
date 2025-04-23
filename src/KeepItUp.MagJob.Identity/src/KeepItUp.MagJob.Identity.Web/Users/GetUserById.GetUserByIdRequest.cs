namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Żądanie dla endpointu GetUserEndpoint.
/// </summary>
public class GetUserByIdRequest
{
    /// <summary>
    /// Szablon ścieżki URL dla endpointu pobierania użytkownika.
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
}
