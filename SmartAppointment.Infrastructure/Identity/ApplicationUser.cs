using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SmartAppointment.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "SSN must be exactly 12 digits.")]
        [StringLength(12, MinimumLength = 12)]
        public string? SSN { get; set; }
    }
}
