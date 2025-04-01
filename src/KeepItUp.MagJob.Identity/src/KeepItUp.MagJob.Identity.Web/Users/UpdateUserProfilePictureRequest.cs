namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Żądanie aktualizacji zdjęcia profilowego użytkownika.
/// </summary>
public class UpdateUserProfilePictureRequest
{
    /// <summary>
    /// Szablon ścieżki URL dla endpointu aktualizacji zdjęcia profilowego użytkownika.
    /// </summary>
    public const string Route = "/Users/{UserId:guid}/profile-picture";

    /// <summary>
    /// Buduje ścieżkę URL dla określonego identyfikatora użytkownika.
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika.</param>
    /// <returns>Ścieżka URL z uwzględnionym identyfikatorem.</returns>
    public static string BuildRoute(Guid userId) => Route.Replace("{UserId:guid}", userId.ToString());

    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Plik ze zdjęciem profilowym.
    /// </summary>
    public IFormFile? ProfilePictureFile { get; set; }
}
