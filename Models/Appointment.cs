using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RandevuSistemi.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = "Beklemede";
        public int ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public Service? Service { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }
    }
}
