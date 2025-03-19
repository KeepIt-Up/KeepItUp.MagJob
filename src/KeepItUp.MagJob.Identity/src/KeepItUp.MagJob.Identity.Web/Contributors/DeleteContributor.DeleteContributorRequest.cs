namespace KeepItUp.MagJob.Identity.Web.Contributors;

/// <summary>
/// Obiekt żądania do usunięcia współpracownika.
/// </summary>
public record DeleteContributorRequest
{
    /// <summary>
    /// Szablon ścieżki URL dla endpointu usuwania współpracownika.
    /// </summary>
    public const string Route = "/Contributors/{ContributorId:int}";

    /// <summary>
    /// Buduje ścieżkę URL dla określonego identyfikatora współpracownika.
    /// </summary>
    /// <param name="contributorId">Identyfikator współpracownika.</param>
    /// <returns>Ścieżka URL z uwzględnionym identyfikatorem.</returns>
    public static string BuildRoute(int contributorId) => Route.Replace("{ContributorId:int}", contributorId.ToString());

    /// <summary>
    /// Identyfikator współpracownika do usunięcia.
    /// </summary>
    public int ContributorId { get; set; }
}
