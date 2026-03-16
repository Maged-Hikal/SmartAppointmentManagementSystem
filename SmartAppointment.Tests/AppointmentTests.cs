using FluentAssertions;
using SmartAppointment.Domain.Entities;
using SmartAppointment.Domain.Enums;

namespace SmartAppointment.Tests
{
    public class AppointmentTests
    {
        private string? createdAt;

        [Fact]
        public void New_Appointment_Should_Be_pending()
        {
            var appointment = new Appointment(DateTime.UtcNow, "John Doe", "user123", "Jhonedoe@smartapp.com", "+4656565656", "121212121212", createdAt: "2025-01-01");
            Assert.Equal(AppointmentStatus.Pending, appointment.Status);
        }

        [Fact]
        public void Approve_Appointment_Should_Set_Status_To_Approved()
        {
            var appointment = new Appointment(DateTime.UtcNow, "John Doe", "user123", "Jhonedoe@smartapp.com", "+4656565656", "121212121212", createdAt: "2025-01-01");
            appointment.Approve();
            Assert.Equal(AppointmentStatus.Approved, appointment.Status);
        }

        [Fact]
        public void Reject_Appointment_Should_Set_Status_To_Rejected()
        {
            var appointment = new Appointment(DateTime.UtcNow, "Jane Smith", "user456", "jane@example.com", "+1234567890", "123456789012", createdAt: "2025-01-01");
            appointment.Reject();
            appointment.Status.Should().Be(AppointmentStatus.Rejected);
        }

        [Fact]
        public void Cancel_Appointment_Should_Set_Status_To_Cancelled()
        {
            var appointment = new Appointment(DateTime.UtcNow, "Bob Johnson", "user789", "bob@example.com", "+0987654321", "987654321098", createdAt: "2025-01-01");
            appointment.Cancel();
            appointment.Status.Should().Be(AppointmentStatus.Cancelled);
        }

        [Fact]
        public void Reschedule_Appointment_Should_Set_New_Date_And_Status()
        {
            var originalDate = DateTime.UtcNow;
            var newDate = originalDate.AddDays(7);
            var appointment = new Appointment(originalDate, "Alice Brown", "user101", "alice@example.com", "+1122334455", "112233445566", createdAt: "2025-01-01");

            appointment.Reschedule(newDate);

            appointment.Date.Should().Be(newDate);
            appointment.Status.Should().Be(AppointmentStatus.Rescheduled);
        }

        [Fact]
        public void AssignToUser_Should_Set_UserId()
        {
            var appointment = new Appointment(DateTime.UtcNow, "Charlie Wilson", "oldUser", "charlie@example.com", "+5544332211", "554433221199", createdAt: "2025-01-01");
            var newUserId = "newUser123";

            appointment.AssignToUser(newUserId);

            appointment.UserId.Should().Be(newUserId);
        }

        [Fact]
        public void Constructor_Should_Set_All_Properties_Correctly()
        {
            var date = DateTime.UtcNow;
            var customerName = "David Lee";
            var userId = "user202";
            var email = "david@example.com";
            var phoneNumber = "+9988776655";
            var ssn = "998877665544";

            var appointment = new Appointment(date, customerName, userId, email, phoneNumber, ssn, createdAt);

            appointment.Id.Should().NotBeEmpty();
            appointment.Date.Should().Be(date);
            appointment.CustomerName.Should().Be(customerName);
            appointment.UserId.Should().Be(userId);
            appointment.Email.Should().Be(email);
            appointment.PhoneNumber.Should().Be(phoneNumber);
            appointment.SSN.Should().Be(ssn);
            appointment.Status.Should().Be(AppointmentStatus.Pending);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Constructor_Should_Handle_Null_Or_Empty_Optional_Fields(string? optionalValue)
        {
            var appointment = new Appointment(DateTime.UtcNow, "Test User", "user123", optionalValue, optionalValue, optionalValue, createdAt);

            appointment.Email.Should().Be(optionalValue);
            appointment.PhoneNumber.Should().Be(optionalValue);
            appointment.SSN.Should().Be(optionalValue);
        }

        [Fact]
        public void Default_Constructor_Should_Create_Instance()
        {
            var appointment = new Appointment();

            appointment.Should().NotBeNull();
            appointment.Id.Should().BeEmpty();
            appointment.UserId.Should().BeEmpty();
        }

        [Fact]
        public void Multiple_Status_Changes_Should_Work_Correctly()
        {
            var appointment = new Appointment(DateTime.UtcNow, "Test User", "user123", "test@example.com", "+1234567890", "123456789012", createdAt: "2025-01-01");

            appointment.Approve();
            appointment.Status.Should().Be(AppointmentStatus.Approved);

            appointment.Cancel();
            appointment.Status.Should().Be(AppointmentStatus.Cancelled);

            appointment.Reschedule(DateTime.UtcNow.AddDays(1));
            appointment.Status.Should().Be(AppointmentStatus.Rescheduled);
        }
    }
}
