using System.ComponentModel.DataAnnotations;

public class RegisterModel
{
    [Required(ErrorMessage = "Full Name is required.")]
    [StringLength(50, ErrorMessage = "Name is too long.")]
    [RegularExpression(@"^[a-zA-Z\s\-']+$", ErrorMessage = "Only letters and name symbols (- ') are allowed.")]
    public string UserName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "That doesn't look like a valid email (e.g., name@example.com).")]
    public string? Email { get; set; }

    [Phone(ErrorMessage = "Invalid phone number format.")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "SSN is required for identity verification.")]
    [RegularExpression(@"^\d{12}$", ErrorMessage = "SSN must be exactly 12 digits.")]
    [StringLength(12, MinimumLength = 12)]
    public string SSN { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

}