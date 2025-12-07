using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System;
using System.Linq;
using Xunit;

namespace ProjectApp.Test
{
    public class PackageServiceTest : IClassFixture<InMemoryServicesFixture>
    {
        private readonly IPackageService _packageService;
        public PackageServiceTest(InMemoryServicesFixture fx)
        {
            _packageService = fx.PackageService;
        }

        [Fact]
        public void Search_ShouldFindPackageFromSeeder()
        {

            var id = Guid.Parse("21372137-2137-2137-2137-213721372137");

            var results = _packageService.Search(id);

            Assert.Single(results);
            Assert.Equal(id, results.First().TrackingNumber);
        }

        [Fact]
        public void CreatePackage_ShouldSetCorrectInitialStatusAndPayment()
        {
            var trackingId = Guid.NewGuid();
            var senderId = Guid.NewGuid();

            _packageService.CreatePackage(
                trackingId,
                senderId,
                DateTime.Now,
                10.0f,
                "Duża",
                PaymentStatus.PlatnoscPrzyOdbiorze
            );

            var createdPackage = _packageService.Search(trackingId).First();

            Assert.Equal(PackageStatus.Nadana, createdPackage.PackageStatus);
            Assert.Equal(PaymentStatus.PlatnoscPrzyOdbiorze, createdPackage.PaymentStatus);
            Assert.Equal(10.0f, createdPackage.Weight);
        }

        [Fact]
        public void GetPackagesByClient_ShouldReturnOnlyOwnPackages()
        {
            var client1 = Guid.NewGuid();
            var client2 = Guid.NewGuid();

            _packageService.CreatePackage(Guid.NewGuid(), client1, DateTime.Now, 1f, "Mała", PaymentStatus.Oplacona);
            _packageService.CreatePackage(Guid.NewGuid(), client1, DateTime.Now, 2f, "Średnia", PaymentStatus.Oplacona);
            _packageService.CreatePackage(Guid.NewGuid(), client2, DateTime.Now, 5f, "Duża", PaymentStatus.Oplacona);

            var client1Packages = _packageService.GetPackagesByClient(client1);

            Assert.Equal(2, client1Packages.Count());
            Assert.All(client1Packages, p => Assert.Equal(client1, p.SenderId));
        }
    }
}