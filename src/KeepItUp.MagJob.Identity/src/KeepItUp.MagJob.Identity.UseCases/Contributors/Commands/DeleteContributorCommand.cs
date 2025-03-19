namespace KeepItUp.MagJob.Identity.UseCases.Contributors.Commands.DeleteContributor;

public record DeleteContributorCommand(int ContributorId) : ICommand<Result>;
