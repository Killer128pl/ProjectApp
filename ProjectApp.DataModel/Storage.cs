namespace ProjectApp.DataModel
{  public class Storage : Package
    {
        public Guid storageId { get; set; }
        public Package package { get; set; }
        public Worker assignedWorker { get; set; }
        public Vehicle assignedVehicle { get; set; }
        public DateTime assignDate { get; set; }
        public List<Package> packages { get; set; }

        public Storage(){}

        public void UpdateAssignment(){}

        public void DisplayStorageInfo(){}
    }

}
