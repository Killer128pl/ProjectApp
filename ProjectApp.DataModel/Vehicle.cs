namespace ProjectApp.DataModel
{
    public class Vehicle
    {
        public Guid vehicleId {  get; set; }
        public string? brand {  get; set; }
        public string? model { get; set; }
        public string? regNumber { get; set; }
        public string? vehicleStatus { get; set; }
        public void updateStatus() { }
    }
}
