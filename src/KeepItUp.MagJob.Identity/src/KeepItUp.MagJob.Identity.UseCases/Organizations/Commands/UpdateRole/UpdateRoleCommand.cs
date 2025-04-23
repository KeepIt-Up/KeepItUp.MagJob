using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateRole;

/// <summary>
/// Komenda do aktualizacji istniejącej roli w organizacji.
/// </summary>
public record UpdateRoleCommand : IRequest<Result>
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Identyfikator roli.
    /// </summary>
    public Guid RoleId { get; init; }

    /// <summary>
    /// Nazwa roli.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Opis roli.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Kolor roli (w formacie HEX).
    /// </summary>
    public string? Color { get; init; }

    /// <summary>
    /// Identyfikator użytkownika wykonującego operację.
    /// </summary>
    public Guid UserId { get; init; }
}
