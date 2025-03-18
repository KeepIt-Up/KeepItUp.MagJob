using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetPermissions;

/// <summary>
/// DTO dla uprawnienia.
/// </summary>
public class PermissionDto
{
    /// <summary>
    /// Nazwa uprawnienia.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Opis uprawnienia.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Kategoria uprawnienia.
    /// </summary>
    public string Category { get; set; } = string.Empty;
}

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
