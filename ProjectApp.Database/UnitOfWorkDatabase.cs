using ProjectApp.Abstractions;

namespace ProjectApp.DataAccess.Database
{
    public class UnitOfWorkDatabase : IUnitOfWork
    {
        private readonly DatabaseDbContext _context;

        public UnitOfWorkDatabase(DatabaseDbContext context)
        {
            _context = context;
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}