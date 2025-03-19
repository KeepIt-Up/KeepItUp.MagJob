namespace KeepItUp.MagJob.Identity.UseCases.Contributors.Commands.CreateContributor;

/// <summary>
/// Create a new Contributor.
/// </summary>
/// <param name="Name"></param>
public record CreateContributorCommand(string Name, string? PhoneNumber) : ICommand<Result<Guid>>;
