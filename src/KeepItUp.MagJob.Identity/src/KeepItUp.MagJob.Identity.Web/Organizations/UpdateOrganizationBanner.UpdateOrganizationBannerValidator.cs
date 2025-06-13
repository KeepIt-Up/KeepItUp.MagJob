namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Validator for the UpdateOrganizationBannerRequest.
/// </summary>
public class UpdateOrganizationBannerValidator : Validator<UpdateOrganizationBannerRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateOrganizationBannerValidator"/> class.
    /// </summary>
    public UpdateOrganizationBannerValidator()
    {
        RuleFor(x => x.OrganizationId)
            .NotEmpty().WithMessage("Identyfikator organizacji jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator organizacji nie może być pusty (Guid.Empty).");

        RuleFor(x => x.BannerFile)
            .NotNull().WithMessage("Plik bannera jest wymagany.");

        RuleFor(x => x.BannerFile)
            .Must(file => file != null && file.Length > 0)
            .WithMessage("Plik bannera nie może być pusty.")
            .When(x => x.BannerFile != null);

        RuleFor(x => x.BannerFile)
            .Must(file => file != null && file.Length <= 5 * 1024 * 1024)
            .WithMessage("Plik bannera nie może być większy niż 5MB.")
            .When(x => x.BannerFile != null && x.BannerFile.Length > 0);

        RuleFor(x => x.BannerFile)
            .Must(file => file != null &&
                 (file.ContentType == "image/jpeg" ||
                  file.ContentType == "image/png" ||
                  file.ContentType == "image/gif"))
            .WithMessage("Niedozwolony typ pliku. Dozwolone typy: JPEG, PNG, GIF.")
            .When(x => x.BannerFile != null && x.BannerFile.Length > 0);
    }
}
