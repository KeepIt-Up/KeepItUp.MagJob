

namespace KeepItUp.MagJob.Identity.Infrastructure.Data;

public static class SeedData
{

    public static async Task InitializeAsync(AppDbContext dbContext)
    {
        await dbContext.SaveChangesAsync();
    }

}
