namespace SmartAppointment.Blazor.Models
{
    public class NotificationRequest
    {
        public string? UserEmail { get; set; }
        public string? UserPhone { get; set; }
        public string? AdminEmail { get; set; } = "admin@smartapp.com"; // Your Admin Email
        public string? Subject { get; set; }
        public string? Message { get; set; }
    }
}
