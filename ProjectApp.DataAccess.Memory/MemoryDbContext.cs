using ProjectApp.DataModel;

namespace ProjectApp.DataAccess.Memory
{
    public class MemoryDbContext
    {
        public List<Package> packages { get; } = new();

        public List<Package> sentPackages { get; } = new();
    }
}
