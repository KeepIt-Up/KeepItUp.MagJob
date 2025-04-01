using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganization;

/// <summary>
/// Komenda do aktualizacji istniejącej organizacji.
/// </summary>
public record UpdateOrganizationCommand : IRequest<Result>
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Nazwa organizacji.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Opis organizacji.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Identyfikator użytkownika wykonującego operację.
    /// </summary>
    public Guid UserId { get; init; }
}
