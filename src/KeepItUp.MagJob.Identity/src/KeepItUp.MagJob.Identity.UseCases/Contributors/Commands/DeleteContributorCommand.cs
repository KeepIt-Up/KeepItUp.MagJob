namespace KeepItUp.MagJob.Identity.UseCases.Contributors.Commands.DeleteContributor;

public record DeleteContributorCommand(Guid ContributorId) : ICommand<Result>;
