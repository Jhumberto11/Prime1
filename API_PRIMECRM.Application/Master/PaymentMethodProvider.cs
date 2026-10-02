using API_PRIMECRM.Domain.Interfaces;
using API_PRIMECRM.Domain.Models.Masters;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Application.Master
{
    public class PaymentMethodProvider
    {
        private readonly IBaseRepository<PaymentMethod> _paymentMethodRepository;
        public PaymentMethodProvider(IBaseRepository<PaymentMethod> paymentMethodRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
        }

        public async Task<IEnumerable<PaymentMethod>> GetAllPaymentMethodsAsync()
        {
            try
            {
                var listPaymentMethods = await _paymentMethodRepository.GetAllAsync();
                if (listPaymentMethods == null || !listPaymentMethods.Any())
                {
                    throw new Exception("No payment methods found.");
                }
                ;
                return listPaymentMethods;
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logging framework here)
                throw new Exception($"An error occurred while retrieving payment methods: {ex.Message}", ex);
            }

        }

        public async Task<PaymentMethod> GetPaymentMethodByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("The payment method ID must be greater than zero.");
            }
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(id);
            if (paymentMethod == null)
            {
                throw new KeyNotFoundException($"No payment method found with ID {id}.");
            }
            return paymentMethod;
        }

        


    }
}
