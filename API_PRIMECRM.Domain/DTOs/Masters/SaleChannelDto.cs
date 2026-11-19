using System;
using System.ComponentModel.DataAnnotations;

namespace API_PRIMECRM.Domain.DTOs.Masters;

public class SaleChannelDto
{
    [Required(ErrorMessage = "El nombre del canal de venta es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre del canal de venta no puede superar los 100 caracteres.")]
    public string Name { get; set; } = string.Empty;

}
