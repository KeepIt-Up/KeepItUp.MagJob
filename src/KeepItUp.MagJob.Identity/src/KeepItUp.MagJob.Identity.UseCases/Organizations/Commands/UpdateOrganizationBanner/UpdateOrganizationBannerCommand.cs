using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganizationBanner;

/// <summary>
/// Command to update the banner of an organization.
/// </summary>
public record UpdateOrganizationBannerCommand : IRequest<Result>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Organization banner URL.
    /// </summary>
    public string BannerUrl { get; init; } = string.Empty;

    /// <summary>
    /// User identifier performing the operation.
    /// </summary>
    public Guid UserId { get; init; }
}
