namespace ProjectApp.DataModel
{
    public class Client : PersonalData
    {
        public Guid ClientId { get; set; } = Guid.NewGuid();
    }
}