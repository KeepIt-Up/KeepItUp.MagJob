namespace KeepItUp.MagJob.Identity.Web.Endpoints.Users;

/// <summary>
/// Odpowiedź dla endpointu GetUserOrganizationsEndpoint.
/// </summary>
public class GetUserOrganizationsResponse
{
    /// <summary>
    /// Lista organizacji użytkownika.
    /// </summary>
    public List<OrganizationDto> Organizations { get; set; } = new List<OrganizationDto>();
} 
