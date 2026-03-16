using System.ComponentModel.DataAnnotations;
namespace SmartAppointment.Blazor.Models
{
    public class CreateAppointmentModel
    {
        [Required]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "That doesn't look like a valid email (e.g., name@example.com).")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name is too long.")]
        [RegularExpression(@"^[a-zA-Z\s\-']+$", ErrorMessage = "Only letters and name symbols (- ') are allowed.")]
        public string? CustomerName { get; set; }
        [Required]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "SSN must be exactly 12 digits.")]
        [StringLength(12, MinimumLength = 12)]
        public string? SSN { get; set; }
    }
}
