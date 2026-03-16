namespace SmartAppointment.Blazor.Models
{
    public class AppointmentModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string? CustomerName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }
        public string? SSN { get; set; }
        public string? CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    }
}
