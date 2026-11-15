using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Domain.Models.Finances.Enums
{
    public enum CategoryTransaction
    {
        // Ingresos
        Transfer = 1,
        CashBack = 2,
        Gift = 3,
        Salary = 4,
        Others = 10


        ,

        // Gastos 

        CarParts = 11,
        Gym =12,
        PersonalExpenses = 13,
        OfficeExpenses = 14,
        BusinessExpenses = 15,
        Subscriptions = 16,
        Loan = 17, // Prestamos


        // Inversion
        TransferToBroker =  100









    }
}
