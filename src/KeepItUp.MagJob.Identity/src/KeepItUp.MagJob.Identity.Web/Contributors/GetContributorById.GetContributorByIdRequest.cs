namespace KeepItUp.MagJob.Identity.Web.Contributors;

/// <summary>
/// Obiekt żądania do pobierania współpracownika po identyfikatorze.
/// </summary>
public class GetContributorByIdRequest
{
    /// <summary>
    /// Szablon ścieżki URL dla endpointu pobierania współpracownika.
    /// </summary>
    public const string Route = "/Contributors/{ContributorId:guid}";

    /// <summary>
    /// Buduje ścieżkę URL dla określonego identyfikatora współpracownika.
    /// </summary>
    /// <param name="contributorId">Identyfikator współpracownika.</param>
    /// <returns>Ścieżka URL z uwzględnionym identyfikatorem.</returns>
    public static string BuildRoute(Guid contributorId) => Route.Replace("{ContributorId:guid}", contributorId.ToString());

    /// <summary>
    /// Identyfikator współpracownika do pobrania.
    /// </summary>
    public Guid ContributorId { get; set; }
}
