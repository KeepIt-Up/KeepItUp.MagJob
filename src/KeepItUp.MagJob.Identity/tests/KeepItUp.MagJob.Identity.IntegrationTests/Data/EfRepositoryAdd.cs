using KeepItUp.MagJob.Identity.Core.ContributorAggregate;

namespace KeepItUp.MagJob.Identity.IntegrationTests.Data;

public class EfRepositoryAdd : BaseEfRepoTestFixture
{
    [Fact]
    public async Task AddsContributorAndSetsId()
    {
        var testContributorName = "testContributor";
        var testContributorStatus = ContributorStatus.NotSet;
        var repository = GetRepository();
        var contributor = Contributor.Create(testContributorName);

        await repository.AddAsync(contributor);

        var newContributor = (await repository.ListAsync())
                        .FirstOrDefault();

        Assert.Equal(testContributorName, newContributor?.Name);
        Assert.Equal(testContributorStatus, newContributor?.Status);
    }
}
