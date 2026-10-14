namespace API_PRIMECRM.Domain.Models.Ventas
{
    public enum ShippingPaymentMode
    {
        Cash = 1,
        TransferToBAC = 2, //America Central
        TransferToBA = 3, //Agricola

        PaymentLink = 4
    }
}