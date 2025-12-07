using System;

namespace ProjectApp.DataModel
{
    public class Package
    {
        public Guid TrackingNumber { get; set; }
        public Guid SenderId { get; set; }
        public DateTime SentDate { get; set; }
        public float Weight { get; set; }
        public string? Size { get; set; }
        public PackageStatus PackageStatus { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Nieoplacona;
        public Guid? AssignedWorkerId { get; set; }

        public override string ToString()
        {
            string statusInfo = PackageStatus.ToString().ToUpper();

            if (PackageStatus == PackageStatus.WTrasie) statusInfo = "W TRASIE";

            return $"Nr: {TrackingNumber.ToString().Substring(0, 8)}... | Status: {statusInfo} | Waga: {Weight}kg";
        }
    }
}