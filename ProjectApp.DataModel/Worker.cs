namespace ProjectApp.DataModel
{
    public class Worker : PersonalData
    {
        private int workerId {  get; set; }
        private string? position { get; set; }
        private Vehicle assignedVehicle { get; set; }
        public void assignVehicle(Vehicle) { }
        public void deliverPackage(Package) { }
    }
}
