using ProjectApp.DataModel;

namespace ProjectApp.DataAccess.Memory
{
    public class MemoryDbContext
    {
        public List<Package> Packages { get; } = new();
        public List<Worker> Workers { get; } = new();
        public List<Vehicle> Vehicles { get; } = new();
        public List<Client> Clients { get; } = new();
        public List<Storage> Storages { get; } = new();
    }
}