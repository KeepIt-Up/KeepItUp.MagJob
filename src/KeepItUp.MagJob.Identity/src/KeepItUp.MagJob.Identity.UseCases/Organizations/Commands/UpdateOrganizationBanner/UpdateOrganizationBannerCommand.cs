using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganizationBanner;

/// <summary>
/// Komenda do aktualizacji bannera organizacji.
/// </summary>
public record UpdateOrganizationBannerCommand : IRequest<Result>
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// URL do bannera organizacji.
    /// </summary>
    public string BannerUrl { get; init; } = string.Empty;

    /// <summary>
    /// Identyfikator użytkownika wykonującego operację.
    /// </summary>
    public Guid UserId { get; init; }
}
