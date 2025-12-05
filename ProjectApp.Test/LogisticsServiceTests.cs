using ProjectApp.DataAccess.Memory;
using ProjectApp.DataModel;
using ProjectApp.Services;
using System;
using Xunit;

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

        Assert.False(result);
        Assert.Equal(PackageStatus.Nadana, package.PackageStatus);
    }

    [Fact]
    public void AssignPackageToCourier_ShouldSuccess_IfWorkerHasVehicle()
    {
        var db = new MemoryDbContext();
        var service = new LogisticsService(db);

        var vehicle = new Vehicle { VehicleId = Guid.NewGuid(), VehicleStatus = "Available" };
        var worker = new Worker { WorkerId = Guid.NewGuid(), Position = "Kurier", AssignedVehicleId = vehicle.VehicleId };

        var package = new Package { TrackingNumber = Guid.NewGuid(), PackageStatus = PackageStatus.Nadana };

        db.Vehicles.Add(vehicle);
        db.Workers.Add(worker);
        db.Packages.Add(package);

        var result = service.AssignPackageToCourier(package.TrackingNumber, worker.WorkerId);

        Assert.True(result);

        Assert.Equal(PackageStatus.WTrasie, package.PackageStatus);
        Assert.Equal(worker.WorkerId, package.AssignedWorkerId);
    }
}