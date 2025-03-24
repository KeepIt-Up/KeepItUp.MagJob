using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Odpowiedź dla endpointu GetUserOrganizationsEndpoint.
/// </summary>
public class GetUserOrganizationsResponse
{
    /// <summary>
    /// Lista organizacji użytkownika.
    /// </summary>
    public List<UserOrganizationRecord> Organizations { get; set; } = new List<UserOrganizationRecord>();
} 
