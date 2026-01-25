using ProjectApp.Abstractions;
using ProjectApp.DataModel;
using System;
using System.Linq;

namespace ProjectApp.DataAccess.Database.Repositories
{
    public class PackageRepositoryDatabase : RepositoryDatabaseBase<Package>, IPackageRepository
    {
        public PackageRepositoryDatabase(DatabaseDbContext context) : base(context)
        {
        }
        public Package? Get(Guid id)
        {
            return DbSet.FirstOrDefault(p => p.TrackingNumber == id);
        }
    }
}