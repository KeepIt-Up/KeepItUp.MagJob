using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganization;

/// <summary>
/// Command to update an existing organization.
/// </summary>
public record UpdateOrganizationCommand : IRequest<Result<EmptyResponse>>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Organization name.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Organization description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// User identifier performing the operation.
    /// </summary>
    public Guid UserId { get; init; }
}
