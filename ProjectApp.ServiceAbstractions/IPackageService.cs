using ProjectApp.DataModel;

namespace ProjectApp.ServiceAbstractions
{
    public interface IPackageService
    {
        bool UpdatePackageStatus(Guid trackingNumber, PackageStatus status);

        bool UpdatePaymentStatus(Guid trackingNumber, PaymentStatus status);

        public Guid CreatePackage(Guid trackingNumber, DateTime sentDate, float weight, string size);

        IReadOnlyList<Package> GetAll();

        IEnumerable<Package> Search(Guid trackingNumber);
    }
}
