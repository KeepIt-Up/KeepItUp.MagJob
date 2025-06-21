namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Odpowiedź endpointu aktualizacji zdjęcia profilowego użytkownika.
/// </summary>
public class UpdateUserProfilePictureResponse
{
    /// <summary>
    /// URL do zaktualizowanego zdjęcia profilowego.
    /// </summary>
    public string? ProfileImageUrl { get; set; }
}
