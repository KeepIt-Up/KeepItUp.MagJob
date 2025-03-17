namespace KeepItUp.MagJob.Identity.UseCases.Contributors.Update;

public record UpdateContributorCommand(Guid ContributorId, string NewName) : ICommand<Result<ContributorDTO>>;
