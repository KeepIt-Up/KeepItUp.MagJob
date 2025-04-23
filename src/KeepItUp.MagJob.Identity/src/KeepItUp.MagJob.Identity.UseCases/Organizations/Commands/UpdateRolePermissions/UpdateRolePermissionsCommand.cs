using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateRolePermissions;

/// <summary>
/// Komenda do aktualizacji uprawnień roli w organizacji.
/// </summary>
public record UpdateRolePermissionsCommand : IRequest<Result>
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
    /// Lista nazw uprawnień do przypisania do roli.
    /// </summary>
    public List<string> Permissions { get; init; } = new();

    /// <summary>
    /// Identyfikator użytkownika wykonującego operację.
    /// </summary>
    public Guid UserId { get; init; }
}
