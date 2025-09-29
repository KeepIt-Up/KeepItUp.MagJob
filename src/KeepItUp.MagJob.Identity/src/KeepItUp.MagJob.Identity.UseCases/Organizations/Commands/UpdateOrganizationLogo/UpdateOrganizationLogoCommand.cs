using MediatR;
using Microsoft.AspNetCore.Http;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganizationLogo;

/// <summary>
/// Command to update the logo of an organization.
/// </summary>
public record UpdateOrganizationLogoCommand : IRequest<Result<string>>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Organization logo file.
    /// </summary>
    public IFormFile LogoFile { get; init; } = null!;

    /// <summary>
    /// User identifier performing the operation.
    /// </summary>
    public Guid UserId { get; init; }
}
