namespace ProjectApp.DataModel
{
    public class Package : Client
    {
        public Guid TrackingNumber { get; set; }
        public DateTime SentDate { get; set; }
        public float Weight { get; set; }
        public string? Size { get; set; }
        public PackageStatus packageStatus { get; set; }
        public PaymentStatus paymentStatus { get; set; }
        public void updateStatus() { }
        public override string ToString()
        {
            return $"Numer paczki: {TrackingNumber},\nData nadania: {SentDate.ToString()},\nWaga: {Weight}kg,\nRozmiar: {Size};";
        }

    }

}
