namespace SmartAppointment.Blazor.Models.Admin
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? SSN { get; set; }
        public string[]? Roles { get; set; }
    }
}
