using KeepItUp.MagJob.Identity.Core.ContributorAggregate;

namespace KeepItUp.MagJob.Identity.UseCases.Contributors.Create;

public class CreateContributorHandler(IRepository<Contributor> _repository)
  : ICommandHandler<CreateContributorCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateContributorCommand request,
      CancellationToken cancellationToken)
    {
        var newContributor = new Contributor(request.Name);
        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            newContributor.SetPhoneNumber(request.PhoneNumber);
        }
        var createdItem = await _repository.AddAsync(newContributor, cancellationToken);

        return createdItem.Id;
    }
}
