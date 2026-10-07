using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace API_PRIMECRM.Domain.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public abstract class Base
    {

        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [DisplayName("Nombre es requerido")]
      
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;

    }
}