using ProjectApp.Abstractions;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectApp.Services
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packages;

        public PackageService(IPackageRepository packages)
        {
            _packages = packages;
        }

        public Guid CreatePackage(Guid trackingNumber, Guid senderId, DateTime sentDate, float weight, string size, PaymentStatus initialPaymentStatus)
        {
            var package = new Package
            {
                TrackingNumber = trackingNumber,
                SenderId = senderId,
                SentDate = sentDate,
                Weight = weight,
                Size = size,
                PackageStatus = PackageStatus.Nadana,
                PaymentStatus = initialPaymentStatus
            };
            _packages.Add(package);
            return package.TrackingNumber;
        }

        public IReadOnlyList<Package> GetAll() => _packages.Query().OrderBy(m => m.SentDate).ToList();

        public IEnumerable<Package> Search(Guid trackingNumber) => _packages.GetAll().Where(p => p.TrackingNumber == trackingNumber);

        public IEnumerable<Package> GetPackagesByClient(Guid clientId) => _packages.GetAll().Where(p => p.SenderId == clientId);

        public IEnumerable<Package> GetPackagesByWorker(Guid workerId) => _packages.GetAll().Where(p => p.AssignedWorkerId == workerId);

        public bool UpdatePackageStatus(Guid trackingNumber, PackageStatus status)
        {
            var m = _packages.Get(trackingNumber);
            if (m is null) return false;
            m.PackageStatus = status;
            return true;
        }

        public bool UpdatePaymentStatus(Guid trackingNumber, PaymentStatus status)
        {
            var m = _packages.Get(trackingNumber);
            if (m is null) return false;
            m.PaymentStatus = status;
            return true;
        }
    }
}