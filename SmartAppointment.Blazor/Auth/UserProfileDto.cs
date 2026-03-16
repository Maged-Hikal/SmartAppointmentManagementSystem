using System.ComponentModel.DataAnnotations;

namespace SmartAppointment.Application.DTOs.Auth
{
    public class UserProfileDto
    {
        public string? UserName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [RegularExpression(@"^\d{12}$", ErrorMessage = "SSN must be exactly 12 digits.")]
        [StringLength(12, MinimumLength = 12)]
        public string? SSN { get; set; }
    }
}