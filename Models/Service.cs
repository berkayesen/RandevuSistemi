using System.ComponentModel.DataAnnotations;

namespace RandevuSistemi.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public List<Appointment>? Appointments { get; set; }
    }
}
