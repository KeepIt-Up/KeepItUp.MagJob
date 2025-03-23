using KeepItUp.MagJob.Identity.Core.ContributorAggregate;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data;

public static class SeedData
{
    public static readonly Contributor Contributor1 = Contributor.Create("Ardalis");
    public static readonly Contributor Contributor2 = Contributor.Create("Snowfrog");

    public static async Task InitializeAsync(AppDbContext dbContext)
    {
        if (await dbContext.Contributors.AnyAsync()) return; // DB has been seeded

        await PopulateTestDataAsync(dbContext);
    }

    public static async Task PopulateTestDataAsync(AppDbContext dbContext)
    {
        dbContext.Contributors.AddRange([Contributor1, Contributor2]);
        await dbContext.SaveChangesAsync();
    }
}
