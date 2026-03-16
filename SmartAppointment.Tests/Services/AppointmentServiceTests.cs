using FluentAssertions;
using Moq;
using SmartAppointment.Application.DTOs;
using SmartAppointment.Application.Interfaces;
using SmartAppointment.Application.Services;
using SmartAppointment.Domain.Entities;
using SmartAppointment.Domain.Enums;

namespace SmartAppointment.Tests.Services
{
    public class AppointmentServiceTests
    {
        private readonly Mock<IAppointmentRepository> _repositoryMock;
        private readonly AppointmentService _appointmentService;

        public AppointmentServiceTests()
        {
            _repositoryMock = new Mock<IAppointmentRepository>();
            _appointmentService = new AppointmentService(_repositoryMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_All_Appointments_As_Dtos()
        {
            // Arrange
            var appointments = new List<Appointment>
            {
                new Appointment(DateTime.UtcNow.AddDays(1), "John Doe", "user1", "john@example.com", "+1234567890", "123456789012", createdAt : "2025-01-01"),
                new Appointment(DateTime.UtcNow.AddDays(2), "Jane Smith", "user2", "jane@example.com", "+0987654321", "987654321098", createdAt : "2025-01-01")
            };

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(appointments);

            // Act
            var result = await _appointmentService.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.First().CustomerName.Should().Be("John Doe");
            result.Last().CustomerName.Should().Be("Jane Smith");
            result.All(a => a.Status == AppointmentStatus.Pending.ToString()).Should().BeTrue();
        }

        [Fact]
        public async Task GetByUserAsync_Should_Return_User_Appointments_As_Dtos()
        {
            // Arrange
            var userId = "user123";
            var appointments = new List<Appointment>
            {
                new Appointment(DateTime.UtcNow.AddDays(1), "John Doe", userId, "john@example.com", "+1234567890", "123456789012", createdAt : "2025-01-01"),
                new Appointment(DateTime.UtcNow.AddDays(2), "John Doe", userId, "john@example.com", "+1234567891", "123456789013", createdAt : "2025-01-01")
            };

            _repositoryMock.Setup(r => r.GetAllByUserAsync(userId)).ReturnsAsync(appointments);

            // Act
            var result = await _appointmentService.GetByUserAsync(userId);

            // Assert
            result.Should().HaveCount(2);
            result.All(a => a.UserId == userId).Should().BeTrue();
        }

        [Fact]
        public async Task CreateAsync_Should_Create_Appointment_And_Save()
        {
            // Arrange
            var createDto = new CreateAppointmentDto
            {
                Date = DateTime.UtcNow.AddDays(1),
                CustomerName = "Test User",
                UserId = "user123",
                Email = "test@example.com",
                PhoneNumber = "+1234567890",
                SSN = "123456789012"
            };

            // Act
            await _appointmentService.CreateAsync(createDto);

            // Assert
            _repositoryMock.Verify(r => r.AddAsync(It.Is<Appointment>(a =>
                a.CustomerName == createDto.CustomerName &&
                a.UserId == createDto.UserId &&
                a.Email == createDto.Email &&
                a.PhoneNumber == createDto.PhoneNumber &&
                a.SSN == createDto.SSN &&
                a.Status == AppointmentStatus.Pending
            )), Times.Once);

            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ApproveAsync_Should_Approve_Appointment_When_Exists()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var appointment = new Appointment(DateTime.UtcNow.AddDays(1), "Test User", "user123", "test@example.com", "+1234567890", "123456789012", createdAt: "2025-01-01");

            _repositoryMock.Setup(r => r.GetByIdAsync(appointmentId)).ReturnsAsync(appointment);

            // Act
            await _appointmentService.ApproveAsync(appointmentId);

            // Assert
            appointment.Status.Should().Be(AppointmentStatus.Approved);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ApproveAsync_Should_Throw_Exception_When_Appointment_Not_Found()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(appointmentId)).ReturnsAsync((Appointment?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _appointmentService.ApproveAsync(appointmentId));
            exception.Message.Should().Be("Appointment not found");
        }

        [Fact]
        public async Task RejectAsync_Should_Delete_Appointment_When_Exists()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var appointment = new Appointment(DateTime.UtcNow.AddDays(1), "Test User", "user123", "test@example.com", "+1234567890", "123456789012", createdAt: "2025-01-01");

            _repositoryMock.Setup(r => r.GetByIdAsync(appointmentId)).ReturnsAsync(appointment);

            // Act
            await _appointmentService.RejectAsync(appointmentId);

            // Assert
            _repositoryMock.Verify(r => r.DeleteAsync(appointment), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RejectAsync_Should_Throw_Exception_When_Appointment_Not_Found()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(appointmentId)).ReturnsAsync((Appointment?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _appointmentService.RejectAsync(appointmentId));
            exception.Message.Should().Be("Appointment not found");
        }

        [Fact]
        public async Task RescheduleAsync_Should_Reschedule_Appointment_When_Exists()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var newDate = DateTime.UtcNow.AddDays(7);
            var appointment = new Appointment(DateTime.UtcNow.AddDays(1), "Test User", "user123", "test@example.com", "+1234567890", "123456789012", createdAt: "2025-01-01");

            _repositoryMock.Setup(r => r.GetByIdAsync(appointmentId)).ReturnsAsync(appointment);

            // Act
            await _appointmentService.RescheduleAsync(appointmentId, newDate);

            // Assert
            appointment.Date.Should().Be(newDate);
            appointment.Status.Should().Be(AppointmentStatus.Rescheduled);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RescheduleAsync_Should_Throw_Exception_When_Appointment_Not_Found()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var newDate = DateTime.UtcNow.AddDays(7);
            _repositoryMock.Setup(r => r.GetByIdAsync(appointmentId)).ReturnsAsync((Appointment?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _appointmentService.RescheduleAsync(appointmentId, newDate));
            exception.Message.Should().Be("Appointment not found");
        }

        [Fact]
        public async Task CancelAsync_Should_Cancel_Appointment_When_Exists()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var appointment = new Appointment(DateTime.UtcNow.AddDays(1), "Test User", "user123", "test@example.com", "+1234567890", "123456789012", createdAt: "2025-01-01");

            _repositoryMock.Setup(r => r.GetByIdAsync(appointmentId)).ReturnsAsync(appointment);

            // Act
            await _appointmentService.CancelAsync(appointmentId);

            // Assert
            appointment.Status.Should().Be(AppointmentStatus.Cancelled);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CancelAsync_Should_Throw_Exception_When_Appointment_Not_Found()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetByIdAsync(appointmentId)).ReturnsAsync((Appointment?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _appointmentService.CancelAsync(appointmentId));
            exception.Message.Should().Be("Appointment not found");
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Empty_List_When_No_Appointments()
        {
            // Arrange
            var emptyAppointments = new List<Appointment>();
            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(emptyAppointments);

            // Act
            var result = await _appointmentService.GetAllAsync();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByUserAsync_Should_Return_Empty_List_When_User_Has_No_Appointments()
        {
            // Arrange
            var userId = "nonexistentuser";
            var emptyAppointments = new List<Appointment>();
            _repositoryMock.Setup(r => r.GetAllByUserAsync(userId)).ReturnsAsync(emptyAppointments);

            // Act
            var result = await _appointmentService.GetByUserAsync(userId);

            // Assert
            result.Should().BeEmpty();
        }
    }
}
