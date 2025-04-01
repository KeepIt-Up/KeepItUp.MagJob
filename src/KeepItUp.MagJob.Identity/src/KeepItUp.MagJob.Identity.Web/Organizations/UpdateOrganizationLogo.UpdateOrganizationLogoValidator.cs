namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Walidator dla żądania aktualizacji logo organizacji.
/// </summary>
public class UpdateOrganizationLogoValidator : Validator<UpdateOrganizationLogoRequest>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UpdateOrganizationLogoValidator"/>.
    /// </summary>
    public UpdateOrganizationLogoValidator()
    {
        // Walidacja identyfikatora organizacji
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");

        // Walidacja pliku logo
        RuleFor(x => x.LogoFile)
            .NotNull().WithMessage("Plik logo jest wymagany.");

        RuleFor(x => x.LogoFile)
            .Must(file => file != null && file.Length > 0)
            .WithMessage("Plik logo nie może być pusty.")
            .When(x => x.LogoFile != null);

        RuleFor(x => x.LogoFile)
            .Must(file => file != null && file.Length <= 2 * 1024 * 1024)
            .WithMessage("Plik logo nie może być większy niż 2MB.")
            .When(x => x.LogoFile != null && x.LogoFile.Length > 0);

        RuleFor(x => x.LogoFile)
            .Must(file => file != null &&
                 (file.ContentType == "image/jpeg" ||
                  file.ContentType == "image/png" ||
                  file.ContentType == "image/gif"))
            .WithMessage("Niedozwolony typ pliku. Dozwolone typy: JPEG, PNG, GIF.")
            .When(x => x.LogoFile != null && x.LogoFile.Length > 0);
    }
}
