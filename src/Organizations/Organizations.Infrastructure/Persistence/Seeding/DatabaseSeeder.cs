using Organizations.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static void SeedRequiredData(ApplicationDbContext context)
    {
        PermissionSeeder.Seed(context);
        context.SaveChanges();
    }

    public static void SeedTestData(ApplicationDbContext context)
    {
        OrganizationSeeder.Seed(context);
        context.SaveChanges();
    }
}