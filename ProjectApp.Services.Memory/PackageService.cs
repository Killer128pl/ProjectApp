using ProjectApp.Abstractions;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System.Text.RegularExpressions;

namespace ProjectApp.Services;

public class PackageService : IPackageService
{
    private readonly IPackageRepository _packages;

    public PackageService(IPackageRepository packages)
    {
        _packages = packages;
    }
    public Guid CreatePackage(Guid trackingNumber, DateTime sentDate,
        float weight, string size)
    {
        var package = new Package
        {
            TrackingNumber = trackingNumber,
            SentDate = sentDate,
            Weight = weight,
            Size = size
        };

        _packages.Add(package);
        return package.TrackingNumber;
    }
    public Package? Get(Guid trackingNumber)
    {
        return _packages.Get(trackingNumber);
    }
    public IReadOnlyList<Package> GetAll()
    {
        return _packages.Query().OrderBy(m => m.SentDate).ToList();
    }
    public IEnumerable<Package> Search(Guid trackingNumber)
    {
        return _packages.GetAll().Where(p => p.TrackingNumber == trackingNumber);
    }
    public bool UpdatePackageStatus(Guid trackingNumber, PackageStatus status)
    {
        var m = _packages.Get(trackingNumber);
        if (m is null) return false;
        m.packageStatus = status;
        return true;
    }
    public bool UpdatePaymentStatus(Guid trackingNumber, PaymentStatus status)
    {
        var m = _packages.Get(trackingNumber);
        if (m is null) return false;
        m.paymentStatus = status;
        return true;
    }
    public bool Delete(Guid trackingNumber)
    {
        var m = _packages.Get(trackingNumber);
        if (m is null) return false;
        _packages.Remove(m);
        return true;
    }
}
