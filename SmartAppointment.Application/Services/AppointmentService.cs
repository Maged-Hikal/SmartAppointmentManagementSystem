using SmartAppointment.Application.DTOs;
using SmartAppointment.Application.Interfaces;
using SmartAppointment.Domain.Entities;

namespace SmartAppointment.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _repository;
        public AppointmentService(IAppointmentRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<AppointmentDto>> GetAllAsync()
        {
            var appointments = await _repository.GetAllAsync();

            return appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                Date = a.Date,
                CustomerName = a.CustomerName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                SSN = a.SSN,
                Status = a.Status.ToString(),
                UserId = a.UserId,
            });
        }
        public async Task<IEnumerable<AppointmentDto>> GetByUserAsync(string userId)
        {
            var appointments = await _repository.GetAllByUserAsync(userId);

            return appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                Date = a.Date,
                CustomerName = a.CustomerName,
                Email = a.Email,
                PhoneNumber = a.PhoneNumber,
                SSN = a.SSN,
                Status = a.Status.ToString(),
                UserId = a.UserId,
            });
        }
        public async Task CreateAsync(CreateAppointmentDto dto)
        {
            var appointment = new Appointment(dto.Date, dto.CustomerName, dto.UserId, dto.Email, dto.PhoneNumber, dto.SSN, dto.CreatedAt);
            await _repository.AddAsync(appointment);
            await _repository.SaveChangesAsync();
        }
        public async Task ApproveAsync(Guid id)
        {
            var appointment = await _repository.GetByIdAsync(id);
            if (appointment == null)
                throw new Exception("Appointment not found");

            appointment.Approve();
            await _repository.SaveChangesAsync();
        }
        public async Task RejectAsync(Guid id)
        {
            var appointment = await _repository.GetByIdAsync(id);

            if (appointment == null)
                throw new Exception("Appointment not found");
            await _repository.DeleteAsync(appointment);
            await _repository.SaveChangesAsync();
        }
        public async Task RescheduleAsync(Guid id, DateTime newDate)
        {
            var appointment = await _repository.GetByIdAsync(id);
            if (appointment == null)
                throw new Exception("Appointment not found");
            appointment.Reschedule(newDate);
            await _repository.SaveChangesAsync();
        }
        public async Task CancelAsync(Guid id)
        {
            var appointment = await _repository.GetByIdAsync(id);
            if (appointment == null)
                throw new Exception("Appointment not found");
            appointment.Cancel();
            await _repository.SaveChangesAsync();
        }
    }
}
