using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateOrganization;

/// <summary>
/// Komenda do tworzenia nowej organizacji.
/// </summary>
public record CreateOrganizationCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// Nazwa organizacji.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Opis organizacji.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Identyfikator właściciela organizacji.
    /// </summary>
    public Guid OwnerId { get; init; }
}
