namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Walidator dla żądania aktualizacji zdjęcia profilowego użytkownika.
/// </summary>
public class UpdateUserProfilePictureValidator : Validator<UpdateUserProfilePictureRequest>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UpdateUserProfilePictureValidator"/>.
    /// </summary>
    public UpdateUserProfilePictureValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Identyfikator użytkownika jest wymagany.");

        RuleFor(x => x.ProfilePictureFile)
            .NotNull()
            .WithMessage("Plik ze zdjęciem profilowym jest wymagany.");

        RuleFor(x => x.ProfilePictureFile)
            .Must(file => file == null || file.Length <= 5 * 1024 * 1024) // 5MB
            .WithMessage("Rozmiar pliku nie może przekraczać 5MB.");

        RuleFor(x => x.ProfilePictureFile)
            .Must(file => file == null || IsValidImageContentType(file.ContentType))
            .WithMessage("Dozwolone są tylko obrazy w formatach JPEG, PNG lub GIF.");
    }

    private bool IsValidImageContentType(string contentType)
    {
        return contentType.StartsWith("image/jpeg") ||
               contentType.StartsWith("image/png") ||
               contentType.StartsWith("image/gif");
    }
}
