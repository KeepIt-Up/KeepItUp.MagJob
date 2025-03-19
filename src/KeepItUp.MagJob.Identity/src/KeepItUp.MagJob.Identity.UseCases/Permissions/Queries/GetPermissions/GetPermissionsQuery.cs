using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Permissions.Queries.GetPermissions;

/// <summary>
/// Zapytanie o wszystkie dostępne uprawnienia w systemie.
/// </summary>
public record GetPermissionsQuery : IRequest<Result<List<PermissionDto>>>
{
    /// <summary>
    /// Identyfikator użytkownika wykonującego zapytanie.
    /// </summary>
    public Guid UserId { get; init; }
}
