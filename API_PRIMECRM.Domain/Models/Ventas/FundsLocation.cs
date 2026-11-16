namespace API_PRIMECRM.Domain.Models.Ventas
{
    public enum FundsLocation
    {
     

        ToCapital = 1, // ya los tengo disponibles en Cuenta BA, pero en cuenta de la empresa, no en mi cuenta personal
        Cash = 2, // Efectivo
        NEQUI = 3, // fondos en Plataforma link de pago
        NoSettle = 4, // Fondos No Liquidados por la empresa de envios
        BAC = 10, // Fondos en Banco Personal America Central
        BA = 11 // fondos en Banco Agricola, Cuenta Principal


    }
}