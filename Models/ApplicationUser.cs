using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RandevuSistemi.Models
{
    public class ApplicationUser:IdentityUser
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
    }
}
