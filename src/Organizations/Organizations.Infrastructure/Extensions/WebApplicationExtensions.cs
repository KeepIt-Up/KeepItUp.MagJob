using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Organizations.Infrastructure.Persistence;

public static class WebApplicationExtensions
{
    public static void ApplyMigrationsAndSeedData(this WebApplication app, IWebHostEnvironment environment)
    {
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<ApplicationDbContext>();


            // Apply pending migrations
            context.Database.Migrate();

            // Seed the database
            DatabaseSeeder.SeedRequiredData(context);

            if (environment.IsDevelopment())
            {
                DatabaseSeeder.SeedTestData(context);
            }
        }
    }
}