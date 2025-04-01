using KeepItUp.MagJob.Identity.Infrastructure.Data;


namespace KeepItUp.MagJob.Identity.FunctionalTests.ApiEndpoints;

[Collection("Sequential")]
public class ContributorGetById(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task ReturnsSeedContributorGivenId1()
    {
        var result = await _client.GetAndDeserializeAsync<ContributorRecord>(GetContributorByIdRequest.BuildRoute(new Guid()));

        Assert.Equal(new Guid(), result.Id);
        Assert.Equal(SeedData.Contributor1.Name, result.Name);
    }

    [Fact]
    public async Task ReturnsNotFoundGivenId1000()
    {
        string route = GetContributorByIdRequest.BuildRoute(new Guid());
        _ = await _client.GetAndEnsureNotFoundAsync(route);
    }
}
