using System.Text.Json.Serialization;


namespace Organizations.Domain.Entities;
public class Role : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public string Color { get; set; }
    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; }
    public List<Member> Members { get; set; } = new List<Member>();
    public List<Permission> Permissions { get; set; } = new List<Permission>();

    private Role() { }
    public static Role Create(string name, Guid organizationId, string? description, string? color)
    {
        return new Role
        {
            Name = name,
            OrganizationId = organizationId,
            Description = description,
            Color = color ?? "#000000"
        };
    }

    public void Update(string? name, string? color)
    {
        // @everyone is a reserved role name
        if (name == "@everyone" && Name != "@everyone")
            throw new InvalidOperationException("Name cannot be '@everyone'");
        // @everyone cannot be updated
        if (Name == "@everyone" && name != null && name != "@everyone")
            throw new InvalidOperationException("Name '@everyone' cannot be updated");

        Name = name ?? Name;
        Color = color ?? Color;
    }

    public static Role[] GetDefaultRoles(Guid organizationId)
    {
        return [
            Create("@everyone", organizationId, "All members of the organization", "#000000")
        ];
    }

    public void UpdatePermissions(List<Permission> permissions)
    {
        // Create a HashSet of the new permission IDs for efficient lookup
        var newPermissionIds = permissions.Select(p => p.Id).ToHashSet();

        // Remove permissions that are no longer needed
        Permissions.RemoveAll(p => !newPermissionIds.Contains(p.Id));

        // Add only new permissions that don't already exist
        var existingPermissionIds = Permissions.Select(p => p.Id).ToHashSet();
        var permissionsToAdd = permissions.Where(p => !existingPermissionIds.Contains(p.Id));

        Permissions.AddRange(permissionsToAdd);
    }

    public void AddMembers(List<Member> members)
    {
        try
        {
            Members.AddRange(members);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to add members to role", ex);
        }
    }

    public void RemoveMembers(List<Member> members)
    {
        try
        {
            Members.RemoveAll(m => members.Equals(m));
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to remove members from role", ex);
        }
    }
}

public class Permission : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    [JsonIgnore]
    public List<Role> Roles { get; set; } = new List<Role>();


}

public class CreateRolePayload
{
    public int OrganizationId { get; set; }
    public string Name { get; set; }
}
