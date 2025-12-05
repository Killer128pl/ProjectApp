namespace ProjectApp.DataModel
{
    public enum PackageStatus
    {
        Nadana,         // Dawniej Sent
        WTrasie,        // Dawniej InTransit
        Dostarczona,    // Dawniej Delivered
        Uszkodzona,     // Dawniej Damaged
        Zwrot           // Nowy status opcjonalny
    }

    public enum PaymentStatus
    {
        Oplacona,           // Paid
        Nieoplacona,        // NotPaid
        PlatnoscPrzyOdbiorze // OnDelivery
    }
}