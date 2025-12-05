using ProjectApp.DataAccess.Memory;
using ProjectApp.DataModel;
using System;
using System.Linq;

namespace ProjectApp.Services
{
    public class LogisticsService
    {
        private readonly MemoryDbContext _db;
        public LogisticsService(MemoryDbContext db) => _db = db;

        public bool AssignVehicleToWorker(Guid workerId, Guid vehicleId)
        {
            var worker = _db.Workers.FirstOrDefault(w => w.WorkerId == workerId);
            var vehicle = _db.Vehicles.FirstOrDefault(v => v.VehicleId == vehicleId);

            if (worker == null || vehicle == null) return false;
            if (vehicle.VehicleStatus != "Available") return false;

            worker.AssignedVehicleId = vehicle.VehicleId;
            vehicle.VehicleStatus = "InUse";
            return true;
        }

        public bool AssignPackageToCourier(Guid packageId, Guid workerId)
        {
            var package = _db.Packages.FirstOrDefault(p => p.TrackingNumber == packageId);
            var worker = _db.Workers.FirstOrDefault(w => w.WorkerId == workerId);

            if (package == null || worker == null) return false;
            if (worker.Position != "Kurier" || worker.AssignedVehicleId == null) return false;

            package.AssignedWorkerId = worker.WorkerId;
            package.PackageStatus = PackageStatus.WTrasie; // Zmiana na PL

            return true;
        }
    }
}