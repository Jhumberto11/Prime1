using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace API_PRIMECRM.Domain.DTOs.Masters
{
    public class BrandDto
    {
        [Required(ErrorMessage = "El nombre de la marca es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El nombre de la marca no puede superar los 100 caracteres.")]
        public string Name { get; set; } = string.Empty;
    }
}
