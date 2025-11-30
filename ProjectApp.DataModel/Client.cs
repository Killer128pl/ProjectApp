namespace ProjectApp.DataModel
{
    public class Client : PersonalData
    {
        private int clientId {  get; set; }
        private List<Package>? sentPackages { get; set; }
        public Package sendPackage() {}
    }
}
