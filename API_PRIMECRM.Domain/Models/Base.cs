using System.ComponentModel.DataAnnotations;

namespace API_PRIMECRM.Domain.Models
{
    public abstract class Base
    {

        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;

    }
}