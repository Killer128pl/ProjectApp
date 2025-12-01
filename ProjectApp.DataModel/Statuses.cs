namespace ProjectApp.DataModel
{
    public enum PackageStatus
    {
        Sent,
        InTransit,
        Delivered,
        Damaged
    }

    public enum PaymentStatus
    {
        Paid,
        NotPaid,
        OnDeliveryPayment
    }
}