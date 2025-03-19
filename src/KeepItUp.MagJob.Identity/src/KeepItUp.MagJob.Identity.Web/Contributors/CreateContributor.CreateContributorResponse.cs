namespace KeepItUp.MagJob.Identity.Web.Contributors;

public class CreateContributorResponse(Guid id, string name)
{
    public Guid Id { get; set; } = id;
    public string Name { get; set; } = name;
}
