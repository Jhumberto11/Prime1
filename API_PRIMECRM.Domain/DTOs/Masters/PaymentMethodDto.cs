using API_PRIMECRM.Domain.Models.Masters.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace API_PRIMECRM.Domain.DTOs.Masters
{
    public class PaymentMethodDto
    {
        [Required(ErrorMessage = "El nombre del método de pago es requerido.")]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Name { get; set; } = string.Empty;

        public PaymentMethodType Type { get; set; }

        public BankName? BankName { get; set; }

        [MaxLength(4, ErrorMessage = "Last4 no puede contener más de 4 caracteres.")]
        public string? Last4 { get; set; }
    }
}
