namespace ProjectApp.DataModel
{
    public class Vehicle
    {
        public Guid VehicleId { get; set; } = Guid.NewGuid();
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? RegNumber { get; set; }
        public string VehicleStatus { get; set; } = "Available";
    }
}