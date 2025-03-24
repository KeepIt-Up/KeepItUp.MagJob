using Organizations.Infrastructure.Persistence;

public static class PermissionSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Permissions.Any())
            return;

        var permissions = new List<Permission>
        {
           new Permission { Name = "Create Organization", Description = "Create a new organization" },
           new Permission { Name = "Edit Organization", Description = "Edit an existing organization" },
           new Permission { Name = "Delete Organization", Description = "Delete an existing organization" },
           new Permission { Name = "Create Role", Description = "Create a new role" },
           new Permission { Name = "Edit Role", Description = "Edit an existing role" },
           new Permission { Name = "Delete Role", Description = "Delete an existing role" },
           new Permission { Name = "Create Member", Description = "Create a new member" },
           new Permission { Name = "Edit Member", Description = "Edit an existing member" },
           new Permission { Name = "Delete Member", Description = "Delete an existing member" },
           new Permission { Name = "Create Invitation", Description = "Create a new invitation" },
           new Permission { Name = "Edit Invitation", Description = "Edit an existing invitation" },
           new Permission { Name = "Delete Invitation", Description = "Delete an existing invitation" }
        };

        context.Permissions.AddRange(permissions);
    }
}