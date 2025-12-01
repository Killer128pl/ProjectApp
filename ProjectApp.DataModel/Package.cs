namespace ProjectApp.DataModel
{
    public class Package
    {
        public Guid TrackingNumber { get; set; }
        public DateTime SentDate { get; set; }
        public float Weight { get; set; }
        public string? Size { get; set; }
        public PackageStatus PackageStatus { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public Guid? AssignedWorkerId { get; set; }

        public override string ToString()
        {
            return $"Numer: {TrackingNumber} | Status: {PackageStatus} | Waga: {Weight}kg | KurierID: {(AssignedWorkerId.HasValue ? AssignedWorkerId.ToString() : "Brak")}";
        }
    }
}