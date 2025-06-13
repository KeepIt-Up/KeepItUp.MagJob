using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateOrganization;

/// <summary>
/// Command to create a new organization.
/// </summary>
public record CreateOrganizationCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// Organization name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Organization description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Owner identifier.
    /// </summary>
    public Guid OwnerId { get; init; }
}
