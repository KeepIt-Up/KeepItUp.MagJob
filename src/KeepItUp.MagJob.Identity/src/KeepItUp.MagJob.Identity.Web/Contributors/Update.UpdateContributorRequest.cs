using System.ComponentModel.DataAnnotations;

namespace KeepItUp.MagJob.Identity.Web.Contributors;

/// <summary>
/// Obiekt żądania do aktualizacji współpracownika.
/// </summary>
public class UpdateContributorRequest
{
    /// <summary>
    /// Szablon ścieżki URL dla endpointu aktualizacji współpracownika.
    /// </summary>
    public const string Route = "/Contributors/{ContributorId:guid}";

    /// <summary>
    /// Buduje ścieżkę URL dla określonego identyfikatora współpracownika.
    /// </summary>
    /// <param name="contributorId">Identyfikator współpracownika.</param>
    /// <returns>Ścieżka URL z uwzględnionym identyfikatorem.</returns>
    public static string BuildRoute(Guid contributorId) => Route.Replace("{ContributorId:guid}", contributorId.ToString());

    /// <summary>
    /// Identyfikator współpracownika do aktualizacji.
    /// </summary>
    public Guid ContributorId { get; set; }

    /// <summary>
    /// Główny identyfikator współpracownika.
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// Nazwa współpracownika.
    /// </summary>
    [Required]
    public string? Name { get; set; }
}
