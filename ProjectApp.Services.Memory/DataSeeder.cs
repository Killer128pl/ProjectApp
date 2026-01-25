using ProjectApp.DataAccess.Database;
using ProjectApp.DataAccess.Memory;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System;

namespace ProjectApp.Services
{
    public class DataSeeder
    {
        private readonly IPackageService _packageSvc;

        private readonly MemoryDbContext? _memoryDb;
        private readonly DatabaseDbContext? _sqlDb;

        public DataSeeder(IPackageService packageSvc, MemoryDbContext? memoryDb, DatabaseDbContext? sqlDb)
        {
            _packageSvc = packageSvc;
            _memoryDb = memoryDb;
            _sqlDb = sqlDb;
        }

        public void Seed()
        {
            var client = new Client
            {
                ClientId = Guid.NewGuid(),
                FirstName = "Jan",
                LastName = "Kowalski",
                PhoneNumber = 123456789
            };

            var worker1 = new Worker { WorkerId = Guid.NewGuid(), FirstName = "Piotr", LastName = "Szybki", Position = "Kurier" };
            var worker2 = new Worker { WorkerId = Guid.NewGuid(), FirstName = "Adam", LastName = "Nowak", Position = "Magazynier" };

            var vehicle1 = new Vehicle { VehicleId = Guid.NewGuid(), Brand = "Ford", Model = "Transit", RegNumber = "WA 12345", VehicleStatus = "Dostępny" };
            var vehicle2 = new Vehicle { VehicleId = Guid.NewGuid(), Brand = "Iveco", Model = "Daily", RegNumber = "ID 99999", VehicleStatus = "Dostępny" };

            if (_sqlDb != null)
            {
                try
                {
                    _sqlDb.Clients.Add(client);
                    _sqlDb.Workers.AddRange(worker1, worker2);
                    _sqlDb.Vehicles.AddRange(vehicle1, vehicle2);
                    _sqlDb.SaveChanges();
                }
                catch
                {
                }
            }

            if (_memoryDb != null)
            {
                _memoryDb.Clients.Add(client);
                _memoryDb.Workers.Add(worker1);
                _memoryDb.Workers.Add(worker2);
                _memoryDb.Vehicles.Add(vehicle1);
                _memoryDb.Vehicles.Add(vehicle2);
            }

            try
            {
                _packageSvc.CreatePackage(Guid.NewGuid(), client.ClientId, DateTime.Now, 2.5f, "Mała", PaymentStatus.Oplacona);
                _packageSvc.CreatePackage(Guid.NewGuid(), client.ClientId, DateTime.Now.AddDays(-1), 15.0f, "Duża", PaymentStatus.Nieoplacona);
                _packageSvc.CreatePackage(Guid.NewGuid(), client.ClientId, DateTime.Now.AddDays(-2), 5.0f, "Średnia", PaymentStatus.PlatnoscPrzyOdbiorze);
            }
            catch
            {
            }
        }
    }
}