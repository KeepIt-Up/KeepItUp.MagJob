namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// Żądanie dla endpointu DeleteOrganizationEndpoint.
/// </summary>
public class DeleteOrganizationRequest
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid Id { get; set; }
} 
