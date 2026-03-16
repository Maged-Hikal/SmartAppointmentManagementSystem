using System.ComponentModel.DataAnnotations;

namespace SmartAppointment.Application.DTOs
{
    public class CreateAppointmentDto
    {
        [Required]
        public DateTime Date { get; set; }
        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string? Email { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        public string? PhoneNumber { get; set; }
        [Required]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "SSN must be exactly 12 digits.")]
        [StringLength(12, MinimumLength = 12)]
        public string? SSN { get; set; }
        public string CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
