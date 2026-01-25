using ProjectApp.DataAccess.Database;
using ProjectApp.DataModel;
using System;
using System.Linq;

namespace ProjectApp.Services
{
    public class LogisticsService
    {
        private readonly DatabaseDbContext _db;

        public LogisticsService(DatabaseDbContext db)
        {
            _db = db;
        }

        public bool AssignVehicleToWorker(Guid workerId, Guid vehicleId)
        {
            var worker = _db.Workers.FirstOrDefault(w => w.WorkerId == workerId);
            var vehicle = _db.Vehicles.FirstOrDefault(v => v.VehicleId == vehicleId);

            if (worker == null || vehicle == null) return false;
            if (vehicle.VehicleStatus != "Dostępny") return false;

            worker.AssignedVehicleId = vehicle.VehicleId;
            vehicle.VehicleStatus = "Zajęty";

            _db.SaveChanges();
            return true;
        }

        public bool AssignPackageToCourier(Guid packageId, Guid workerId)
        {
            var package = _db.Packages.FirstOrDefault(p => p.TrackingNumber == packageId);
            var worker = _db.Workers.FirstOrDefault(w => w.WorkerId == workerId);

            if (package == null || worker == null) return false;
            if (worker.Position != "Kurier" || worker.AssignedVehicleId == null) return false;

            package.AssignedWorkerId = worker.WorkerId;
            package.PackageStatus = PackageStatus.WTrasie;

            _db.SaveChanges();
            return true;
        }
    }
}