using ProjectApp.ServiceAbstractions;
using Xunit;

public class PackageServiceTest : IClassFixture<InMemoryServicesFixture>
{
    private readonly IPackageService _packageService;

    public PackageServiceTest(InMemoryServicesFixture fx)
    {
        _packageService = fx.PackageService;

    }
    [Fact]
    public void Search_ShouldFindPackage2137()
    {
        var id = Guid.Parse("2137");
        var results = _packageService.Search(id);
        Assert.Contains(results, p => p.TrackingNumber == id);
    }
}