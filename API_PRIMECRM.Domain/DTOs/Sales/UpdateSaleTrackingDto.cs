using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace API_PRIMECRM.Domain.DTOs.Sales
{
    public class UpdateSaleTrackingDto
    {
        [Required(ErrorMessage = "El tracking es obligatorio")]
        public string TrackingNumber { get; set; } = string.Empty;
    }
}
