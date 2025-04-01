
namespace KeepItUp.MagJob.Identity.UseCases.Permissions.Queries;

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