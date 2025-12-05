using System;

namespace ProjectApp.DataModel
{
    public class Package
    {
        public Guid TrackingNumber { get; set; }
        public Guid SenderId { get; set; } // ID Klienta
        public DateTime SentDate { get; set; }
        public float Weight { get; set; }
        public string? Size { get; set; }
        public PackageStatus PackageStatus { get; set; }
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.NotPaid;
        public Guid? AssignedWorkerId { get; set; }

        public override string ToString()
        {
            string payInfo = PaymentStatus == PaymentStatus.Paid ? "[OPŁACONA]" : "[NIEOPŁACONA]";
            return $"{payInfo} Nr: {TrackingNumber.ToString().Substring(0, 8)}... | Status: {PackageStatus} | Waga: {Weight}kg";
        }
    }
}