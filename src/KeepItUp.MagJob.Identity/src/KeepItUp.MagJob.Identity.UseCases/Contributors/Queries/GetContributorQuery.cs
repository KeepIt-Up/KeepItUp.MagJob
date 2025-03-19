namespace KeepItUp.MagJob.Identity.UseCases.Contributors.Queries.GetContributor;

public record GetContributorQuery(Guid ContributorId) : IQuery<Result<ContributorDTO>>;
