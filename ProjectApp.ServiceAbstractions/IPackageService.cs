using ProjectApp.DataModel;
using System;
using System.Collections.Generic;

namespace ProjectApp.ServiceAbstractions
{
    public interface IPackageService
    {
        bool UpdatePackageStatus(Guid trackingNumber, PackageStatus status);
        bool UpdatePaymentStatus(Guid trackingNumber, PaymentStatus status);

        Guid CreatePackage(Guid trackingNumber, Guid senderId, DateTime sentDate, float weight, string size, PaymentStatus initialPaymentStatus);

        IReadOnlyList<Package> GetAll();
        IEnumerable<Package> Search(Guid trackingNumber);

        IEnumerable<Package> GetPackagesByClient(Guid clientId);
        IEnumerable<Package> GetPackagesByWorker(Guid workerId);
    }
}