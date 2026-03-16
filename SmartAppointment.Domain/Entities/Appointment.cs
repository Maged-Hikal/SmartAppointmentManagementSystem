using SmartAppointment.Domain.Enums;

namespace SmartAppointment.Domain.Entities
{
    public class Appointment
    {
        public Guid Id { get; private set; }
        public DateTime Date { get; private set; }
        public string CustomerName { get; private set; }
        public string? Email { get; private set; }
        public string? PhoneNumber { get; set; }
        public AppointmentStatus Status { get; private set; }
        public string? SSN { get; set; }

        public string? UserId { get; private set; }
        public string? CreatedAt { get; set; } = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        public Appointment() { }
        public Appointment(DateTime date, string customerName, string userId, string? email, string? phoneNumber, string? SSn, string? createdAt)
        {
            Id = Guid.NewGuid();
            Date = date;
            CustomerName = customerName;
            Status = AppointmentStatus.Pending;
            UserId = userId;
            Email = email;
            PhoneNumber = phoneNumber;
            SSN = SSn;
            CreatedAt = createdAt;
        }
        public void AssignToUser(string userId) => UserId = userId;
        public void Approve()
        {
            Status = AppointmentStatus.Approved;
        }
        public void Reject()
        {
            Status = AppointmentStatus.Rejected;
        }
        public void Cancel()
        {
            Status = AppointmentStatus.Cancelled;
        }
        public void Reschedule(DateTime newDate)
        {
            Date = newDate;
            Status = AppointmentStatus.Rescheduled;
        }
    }
}
