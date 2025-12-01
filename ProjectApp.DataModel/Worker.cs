namespace ProjectApp.DataModel
{
    public class Worker : PersonalData
    {
        public Guid WorkerId { get; set; } = Guid.NewGuid();
        public string? Position { get; set; }
        public Guid? AssignedVehicleId { get; set; }
    }
}