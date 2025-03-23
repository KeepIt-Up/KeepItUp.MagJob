using KeepItUp.MagJob.Identity.Core.ContributorAggregate;


namespace KeepItUp.MagJob.Identity.IntegrationTests.Data;

public class EfRepositoryUpdate : BaseEfRepoTestFixture
{
    [Fact]
    public async Task UpdatesItemAfterAddingIt()
    {
        // add a Contributor
        var repository = GetRepository();
        var initialName = Guid.NewGuid().ToString();
        var contributor = Contributor.Create(initialName);

        await repository.AddAsync(contributor);

        // detach the item so we get a different instance
        _dbContext.Entry(contributor).State = EntityState.Detached;

        // fetch the item and update its title
        var newContributor = (await repository.ListAsync())
            .FirstOrDefault(Contributor => Contributor.Name == initialName);
        if (newContributor == null)
        {
            Assert.NotNull(newContributor);
            return;
        }
        Assert.NotSame(contributor, newContributor);
        var newName = Guid.NewGuid().ToString();
        newContributor.UpdateName(newName);

        // Update the item
        await repository.UpdateAsync(newContributor);

        // Fetch the updated item
        var updatedItem = (await repository.ListAsync())
            .FirstOrDefault(Contributor => Contributor.Name == newName);

        Assert.NotNull(updatedItem);
        Assert.NotEqual(contributor.Name, updatedItem?.Name);
        Assert.Equal(contributor.Status, updatedItem?.Status);
        Assert.Equal(newContributor.Id, updatedItem?.Id);
    }
}
