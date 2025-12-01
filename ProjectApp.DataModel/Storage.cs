namespace ProjectApp.DataModel
{
    public class Storage
    {
        public Guid StorageId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "Main Storage";
        public List<Package> StoredPackages { get; set; } = new();
    }
}