using System.ComponentModel.DataAnnotations;

namespace KeepItUp.MagJob.Identity.Web.Contributors;

public class UpdateContributorRequest
{
  public const string Route = "/Contributors/{ContributorId:guid}";
  public static string BuildRoute(Guid contributorId) => Route.Replace("{ContributorId:guid}", contributorId.ToString());

  public Guid ContributorId { get; set; }

  [Required]
  public Guid Id { get; set; }
  [Required]
  public string? Name { get; set; }
}
