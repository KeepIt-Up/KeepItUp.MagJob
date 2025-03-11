namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Odpowiedź dla endpointu GetOrganizationById.
/// </summary>
public class GetOrganizationByIdResponse
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nazwa organizacji.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Opis organizacji.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Identyfikator właściciela organizacji.
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// Czy użytkownik jest właścicielem organizacji.
    /// </summary>
    public bool IsOwner { get; set; }

    /// <summary>
    /// Liczba członków organizacji.
    /// </summary>
    public int MemberCount { get; set; }
} 
