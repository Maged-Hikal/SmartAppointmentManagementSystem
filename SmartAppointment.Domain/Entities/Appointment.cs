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

        public string UserId { get; private set; } = string.Empty;
        public Appointment() { }
        public Appointment(DateTime date, string customerName, string userId, string? email, string? phoneNumber, string? SSn)
        {
            Id = Guid.NewGuid();
            Date = date;
            CustomerName = customerName;
            Status = AppointmentStatus.Pending;
            UserId = userId;
            Email = email;
            PhoneNumber = phoneNumber;
            SSN = SSn;
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
