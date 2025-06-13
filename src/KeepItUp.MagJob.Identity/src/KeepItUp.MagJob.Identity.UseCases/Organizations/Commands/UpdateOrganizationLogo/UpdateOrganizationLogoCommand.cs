using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganizationLogo;

/// <summary>
/// Command to update the logo of an organization.
/// </summary>
public record UpdateOrganizationLogoCommand : IRequest<Result>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Organization logo URL.
    /// </summary>
    public string LogoUrl { get; init; } = string.Empty;

    /// <summary>
    /// User identifier performing the operation.
    /// </summary>
    public Guid UserId { get; init; }
}
