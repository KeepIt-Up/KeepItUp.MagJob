
namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Odpowiedź zawierająca identyfikator utworzonego zaproszenia.
/// </summary>
public class CreateInvitationResponse
{
    /// <summary>
    /// Identyfikator utworzonego zaproszenia.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Adres email osoby zapraszanej.
    /// </summary>
    public string Email { get; set; } = string.Empty;
}