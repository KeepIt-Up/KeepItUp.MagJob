

using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Odpowiedź z listą członków organizacji.
/// </summary>
public class GetOrganizationMembersResponse
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Lista członków organizacji.
    /// </summary>
    public List<MemberDto> MembersList { get; set; } = new();
}
