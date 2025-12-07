using ProjectApp.DataAccess.Memory;
using ProjectApp.DataModel;
using ProjectApp.Services;
using System;
using Xunit;

namespace ProjectApp.Test
{
    public class LogisticsServiceTests
    {
        [Fact]
        public void AssignPackageToCourier_ShouldFail_IfWorkerHasNoVehicle()
        {
            var db = new MemoryDbContext();
            var service = new LogisticsService(db);

            var worker = new Worker { WorkerId = Guid.NewGuid(), Position = "Kurier" };

            var package = new Package { TrackingNumber = Guid.NewGuid(), PackageStatus = PackageStatus.Nadana };

            db.Workers.Add(worker);
            db.Packages.Add(package);

            var result = service.AssignPackageToCourier(package.TrackingNumber, worker.WorkerId);

            Assert.False(result, "Nie powinno udać się przypisać paczki kurierowi bez auta.");
            Assert.Equal(PackageStatus.Nadana, package.PackageStatus);
        }

        [Fact]
        public void AssignPackageToCourier_ShouldSuccess_IfWorkerHasVehicle()
        {
            var db = new MemoryDbContext();
            var service = new LogisticsService(db);

            var vehicle = new Vehicle { VehicleId = Guid.NewGuid(), VehicleStatus = "Dostępny" };
            var worker = new Worker { WorkerId = Guid.NewGuid(), Position = "Kurier", AssignedVehicleId = vehicle.VehicleId };
            var package = new Package { TrackingNumber = Guid.NewGuid(), PackageStatus = PackageStatus.Nadana };

            db.Vehicles.Add(vehicle);
            db.Workers.Add(worker);
            db.Packages.Add(package);

            var result = service.AssignPackageToCourier(package.TrackingNumber, worker.WorkerId);

            Assert.True(result, "Powinno udać się przypisać paczkę.");
            Assert.Equal(PackageStatus.WTrasie, package.PackageStatus);
            Assert.Equal(worker.WorkerId, package.AssignedWorkerId);
        }

        [Fact]
        public void AssignVehicleToWorker_ShouldSetVehicleAsBusy()
        {
            var db = new MemoryDbContext();
            var service = new LogisticsService(db);

            var worker = new Worker { WorkerId = Guid.NewGuid(), Position = "Kurier" };
            var vehicle = new Vehicle { VehicleId = Guid.NewGuid(), VehicleStatus = "Dostępny" };

            db.Workers.Add(worker);
            db.Vehicles.Add(vehicle);

            var result = service.AssignVehicleToWorker(worker.WorkerId, vehicle.VehicleId);

            Assert.True(result);
            Assert.Equal(vehicle.VehicleId, worker.AssignedVehicleId);
            Assert.Equal("Zajęty", vehicle.VehicleStatus);
        }

        [Fact]
        public void AssignVehicleToWorker_ShouldFail_IfVehicleIsBusy()
        {
            var db = new MemoryDbContext();
            var service = new LogisticsService(db);

            var worker = new Worker { WorkerId = Guid.NewGuid() };
            var vehicle = new Vehicle { VehicleId = Guid.NewGuid(), VehicleStatus = "Zajęty" };

            db.Workers.Add(worker);
            db.Vehicles.Add(vehicle);

            var result = service.AssignVehicleToWorker(worker.WorkerId, vehicle.VehicleId);

            Assert.False(result, "Nie można przypisać zajętego auta.");
            Assert.Null(worker.AssignedVehicleId);
        }
    }
}