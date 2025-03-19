namespace KeepItUp.MagJob.Identity.UseCases.Contributors.Commands.UpdateContributor;

public record UpdateContributorCommand(Guid ContributorId, string NewName) : ICommand<Result<ContributorDTO>>;
