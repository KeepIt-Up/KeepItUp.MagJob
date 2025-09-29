using MediatR;
using Microsoft.AspNetCore.Http;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganizationBanner;

/// <summary>
/// Command to update the banner of an organization.
/// </summary>
public record UpdateOrganizationBannerCommand : IRequest<Result<string>>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Organization banner file.
    /// </summary>
    public IFormFile BannerFile { get; init; } = null!;

    /// <summary>
    /// User identifier performing the operation.
    /// </summary>
    public Guid UserId { get; init; }
}
