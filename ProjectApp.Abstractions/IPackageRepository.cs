using ProjectApp.DataModel;

namespace ProjectApp.Abstractions
{
    public interface IPackageRepository
    {
        IQueryable<Package> Query();
        Package? Get(Guid id);
        void Add(Package entity);
        void Remove(Package entity);
        IEnumerable<Package> GetAll();
    }
}
