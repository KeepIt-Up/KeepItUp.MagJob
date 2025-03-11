namespace KeepItUp.MagJob.Identity.Web.Endpoints.Organizations;

/// <summary>
/// Żądanie dla endpointu GetOrganizationEndpoint.
/// </summary>
public class GetOrganizationRequest
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid Id { get; set; }
} 
