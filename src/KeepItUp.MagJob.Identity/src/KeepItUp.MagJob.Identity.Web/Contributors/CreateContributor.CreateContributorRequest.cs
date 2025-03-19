using System.ComponentModel.DataAnnotations;

namespace KeepItUp.MagJob.Identity.Web.Contributors;

public class CreateContributorRequest
{
    public const string Route = "/Contributors";

    [Required]
    public string? Name { get; set; }
    public string? PhoneNumber { get; set; }
}
