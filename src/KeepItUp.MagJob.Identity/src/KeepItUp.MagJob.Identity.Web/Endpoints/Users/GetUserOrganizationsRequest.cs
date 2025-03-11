namespace KeepItUp.MagJob.Identity.Web.Endpoints.Users;

/// <summary>
/// Żądanie dla endpointu GetUserOrganizationsEndpoint.
/// </summary>
public class GetUserOrganizationsRequest
{
    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid Id { get; set; }
} 
