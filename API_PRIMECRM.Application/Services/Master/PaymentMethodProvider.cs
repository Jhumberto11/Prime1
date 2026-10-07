using API_PRIMECRM.Domain.Interfaces;
using API_PRIMECRM.Domain.Models.Masters;
using API_PRIMECRM.Domain.Models.Masters.Enums;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace API_PRIMECRM.Application.Services.Master
{
    public class PaymentMethodProvider
    {
        private readonly IBaseRepository<PaymentMethod> _paymentMethodRepository;

        public PaymentMethodProvider(
            IBaseRepository<PaymentMethod> paymentMethodRepository)
        {
            _paymentMethodRepository = paymentMethodRepository;
        }


        public async Task<IEnumerable<PaymentMethod>>
            GetAllPaymentMethodsAsync()
        {
            return await _paymentMethodRepository.GetAllAsync();
        }


        public async Task<PaymentMethod>
            GetPaymentMethodByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "El Id del método de pago debe ser mayor que cero.",
                    nameof(id));
            }

            var paymentMethod =
                await _paymentMethodRepository.GetByIdAsync(id);

            if (paymentMethod == null)
            {
                throw new KeyNotFoundException(
                    $"No existe un método de pago con Id {id}.");
            }

            return paymentMethod;
        }


        public async Task<PaymentMethod>
            AddPaymentMethodAsync(PaymentMethod paymentMethod)
        {
            if (paymentMethod == null)
            {
                throw new ArgumentNullException(nameof(paymentMethod));
            }

            ValidatePaymentMethod(paymentMethod);

            paymentMethod.Name = paymentMethod.Name.Trim();

            var exists =
                await _paymentMethodRepository.ExistsAsync(
                    x => x.Name.ToLower() ==
                         paymentMethod.Name.ToLower());

            if (exists)
            {
                throw new InvalidOperationException(
                    $"El método de pago '{paymentMethod.Name}' ya existe.");
            }

            await _paymentMethodRepository.AddAsync(paymentMethod);

            return paymentMethod;
        }


        public async Task<PaymentMethod>
            UpdatePaymentMethodAsync(PaymentMethod paymentMethod)
        {
            if (paymentMethod == null)
            {
                throw new ArgumentNullException(nameof(paymentMethod));
            }

            if (paymentMethod.Id <= 0)
            {
                throw new ArgumentException(
                    "El Id del método de pago debe ser mayor que cero.",
                    nameof(paymentMethod));
            }

            ValidatePaymentMethod(paymentMethod);

            var existingPaymentMethod =
                await _paymentMethodRepository
                    .GetByIdAsync(paymentMethod.Id);

            if (existingPaymentMethod == null)
            {
                throw new KeyNotFoundException(
                    $"No existe un método de pago con Id {paymentMethod.Id}.");
            }

            var normalizedName = paymentMethod.Name.Trim();

            var duplicateExists =
                await _paymentMethodRepository.ExistsAsync(
                    x => x.Id != paymentMethod.Id &&
                         x.Name.ToLower() ==
                         normalizedName.ToLower());

            if (duplicateExists)
            {
                throw new InvalidOperationException(
                    $"Ya existe un método de pago llamado '{normalizedName}'.");
            }

            existingPaymentMethod.Name = normalizedName;
            existingPaymentMethod.Type = paymentMethod.Type;
            existingPaymentMethod.BankName = paymentMethod.BankName;
            existingPaymentMethod.Last4 =
                NormalizeLast4(paymentMethod.Last4);

            await _paymentMethodRepository
                .Update(existingPaymentMethod);

            return existingPaymentMethod;
        }


        public async Task DeactivatePaymentMethodAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "El Id del método de pago debe ser mayor que cero.",
                    nameof(id));
            }

            var paymentMethod =
                await _paymentMethodRepository.GetByIdAsync(id);

            if (paymentMethod == null)
            {
                throw new KeyNotFoundException(
                    $"No existe un método de pago con Id {id}.");
            }

            if (!paymentMethod.IsActive)
            {
                return;
            }

            paymentMethod.IsActive = false;

            await _paymentMethodRepository.Update(paymentMethod);
        }


        public async Task ActivatePaymentMethodAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "El Id del método de pago debe ser mayor que cero.",
                    nameof(id));
            }

            var paymentMethod =
                await _paymentMethodRepository.GetByIdAsync(id);

            if (paymentMethod == null)
            {
                throw new KeyNotFoundException(
                    $"No existe un método de pago con Id {id}.");
            }

            if (paymentMethod.IsActive)
            {
                return;
            }

            paymentMethod.IsActive = true;

            await _paymentMethodRepository.Update(paymentMethod);
        }


        private static void ValidatePaymentMethod(
            PaymentMethod paymentMethod)
        {
            if (string.IsNullOrWhiteSpace(paymentMethod.Name))
            {
                throw new ArgumentException(
                    "El nombre del método de pago es requerido.",
                    nameof(paymentMethod));
            }

            if (!Enum.IsDefined(typeof(PaymentMethodType),
                                paymentMethod.Type))
            {
                throw new ArgumentException(
                    "El tipo de método de pago no es válido.",
                    nameof(paymentMethod));
            }

            ValidatePaymentMethodDetails(paymentMethod);
        }


        private static void ValidatePaymentMethodDetails(
            PaymentMethod paymentMethod)
        {
            switch (paymentMethod.Type)
            {
                case PaymentMethodType.CreditCard:
                case PaymentMethodType.DebitCard:

                    ValidateBank(paymentMethod.BankName);

                    if (string.IsNullOrWhiteSpace(paymentMethod.Last4))
                    {
                        throw new ArgumentException(
                            "Los últimos 4 dígitos son requeridos para una tarjeta.");
                    }

                    if (paymentMethod.Last4.Length != 4 ||
                        !paymentMethod.Last4.All(char.IsDigit))
                    {
                        throw new ArgumentException(
                            "Last4 debe contener exactamente 4 dígitos.");
                    }

                    break;


                case PaymentMethodType.BankAccount:

                    ValidateBank(paymentMethod.BankName);

                    if (!string.IsNullOrWhiteSpace(paymentMethod.Last4) &&
                        (paymentMethod.Last4.Length != 4 ||
                         !paymentMethod.Last4.All(char.IsDigit)))
                    {
                        throw new ArgumentException(
                            "Last4 debe contener exactamente 4 dígitos.");
                    }

                    break;


                case PaymentMethodType.Cash:
                case PaymentMethodType.Other:

                    if (!string.IsNullOrWhiteSpace(paymentMethod.Last4))
                    {
                        throw new ArgumentException(
                            $"El tipo de pago '{paymentMethod.Type}' " +
                            "no debe contener últimos 4 dígitos.");
                    }

                    break;


                default:
                    throw new ArgumentException(
                        "El tipo de método de pago no es válido.");
            }
        }


        private static void ValidateBank(BankName bankName)
        {
            if (!Enum.IsDefined(typeof(BankName), bankName))
            {
                throw new ArgumentException(
                    "El banco seleccionado no es válido.");
            }
        }


        private static string? NormalizeLast4(string? last4)
        {
            return string.IsNullOrWhiteSpace(last4)
                ? null
                : last4.Trim();
        }
    }
}
