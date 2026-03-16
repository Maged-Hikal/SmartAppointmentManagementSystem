using System.ComponentModel.DataAnnotations;

namespace SmartAppointment.Blazor.Models
{
    public class UserProfileDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? SSN { get; set; }
    }
    public class PasswordChangeRequest
    {
        [Required]
        public string CurrentPassword { get; set; } = "";

        [Required, MinLength(6)]
        public string NewPassword { get; set; } = "";
    }
}
