using SmartAppointment.Application.DTOs;

namespace SmartAppointment.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDto>> GetAllAsync();
        Task<IEnumerable<AppointmentDto>> GetByUserAsync(string userId);
        //Task<AppointmentDto> GetAsync (Guid id);
        Task CreateAsync(CreateAppointmentDto dto);
        Task ApproveAsync(Guid id);
        Task RejectAsync(Guid id);
        Task RescheduleAsync(Guid id, DateTime newDate);
        Task CancelAsync(Guid id);
    }
}
