namespace KeepItUp.MagJob.Identity.UseCases.Contributors.Get;

public record GetContributorQuery(Guid ContributorId) : IQuery<Result<ContributorDTO>>;
