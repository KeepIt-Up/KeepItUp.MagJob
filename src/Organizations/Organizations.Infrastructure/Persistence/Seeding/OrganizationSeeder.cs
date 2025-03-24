using Organizations.Infrastructure.Persistence;

public static class OrganizationSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        if (context.Organizations.Any())
        {
            return;
        }

        var sampleProfileImage = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+ip1sAAAAASUVORK5CYII=");
        var sampleBannerImage = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+ip1sAAAAASUVORK5CYII=");

        var organizations = new List<Organization>
        {
            Organization.Create("Organization 1", Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Description 1", sampleProfileImage, sampleBannerImage),
            Organization.Create("Organization 2", Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Description 2", sampleProfileImage, sampleBannerImage),
            Organization.Create("Organization 3", Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Description 3", sampleProfileImage, sampleBannerImage),
            Organization.Create("Organization 4", Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Description 4", sampleProfileImage, sampleBannerImage),
            Organization.Create("Organization 5", Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Description 5", sampleProfileImage, sampleBannerImage),
            Organization.Create("Organization 6", Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Description 6", sampleProfileImage, sampleBannerImage),
            Organization.Create("Organization 7", Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Description 7", sampleProfileImage, sampleBannerImage),
            Organization.Create("Organization 8", Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Description 8", sampleProfileImage, sampleBannerImage),
            Organization.Create("Organization 9", Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Description 9", sampleProfileImage, sampleBannerImage),
            Organization.Create("Organization 10", Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Description 10", sampleProfileImage, sampleBannerImage),
            Organization.Create("Organization 11", Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Description 11", sampleProfileImage, sampleBannerImage),
            Organization.Create("Organization 12", Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Description 12", sampleProfileImage, sampleBannerImage),
            Organization.Create("Organization 13", Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Description 13", sampleProfileImage, sampleBannerImage),
            Organization.Create("Organization 14", Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Description 14", sampleProfileImage, sampleBannerImage),
            Organization.Create("Organization 15", Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Description 15", sampleProfileImage, sampleBannerImage),
            Organization.Create("Organization 16", Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Description 16", sampleProfileImage, sampleBannerImage),
            Organization.Create("Organization 17", Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Description 17", sampleProfileImage, sampleBannerImage),
            Organization.Create("Organization 18", Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Description 18", sampleProfileImage, sampleBannerImage),
            Organization.Create("Organization 19", Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Description 19", sampleProfileImage, sampleBannerImage),

        };

        SeedMembersAndRoles(organizations);
        SeedInvitations(organizations.First());
        context.Organizations.AddRange(organizations);
    }

    private static void SeedMembersAndRoles(List<Organization> organizations)
    {
        var org1 = organizations[0];
        org1.AddMember(Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "James", "Dean");
        org1.AddMember(Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "John", "Doe");
        org1.AddMember(Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Jane", "Smith");
        org1.AddMember(Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Alice", "Johnson");
        org1.AddMember(Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Bob", "Brown");
        org1.AddMember(Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Charlie", "Davis");
        org1.AddMember(Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Diana", "Lee");
        org1.AddMember(Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Ethan", "Garcia");
        org1.AddMember(Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "Fiona", "Martinez");
        org1.AddMember(Guid.Parse("bb5d1b04-6f73-4ac0-acd2-e746ecf835f6"), "George", "Hernandez");
    }

    private static void SeedInvitations(Organization organization)
    {
        organization.Invitations.Add(Invitation.Create(organization.Id, Guid.NewGuid()));
        organization.Invitations.Add(Invitation.Create(organization.Id, Guid.NewGuid()));
        organization.Invitations.Add(Invitation.Create(organization.Id, Guid.NewGuid()));
        organization.Invitations.Add(Invitation.Create(organization.Id, Guid.NewGuid()));
        organization.Invitations.Add(Invitation.Create(organization.Id, Guid.NewGuid()));
        organization.Invitations.Add(Invitation.Create(organization.Id, Guid.NewGuid()));
        organization.Invitations.Add(Invitation.Create(organization.Id, Guid.NewGuid()));
        organization.Invitations.Add(Invitation.Create(organization.Id, Guid.NewGuid()));
        organization.Invitations.Add(Invitation.Create(organization.Id, Guid.NewGuid()));
        organization.Invitations.Add(Invitation.Create(organization.Id, Guid.NewGuid()));
        organization.Invitations.Add(Invitation.Create(organization.Id, Guid.NewGuid()));
    }
}