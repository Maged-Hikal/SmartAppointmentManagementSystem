using SmartAppointment.Domain.Entities;

namespace SmartAppointment.Application.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<IEnumerable<Appointment>> GetAllByUserAsync(string userId);
        Task<Appointment?> GetByIdAsync(Guid id);
        Task AddAsync(Appointment appointment);
        Task DeleteAsync(Appointment appointment);
        Task SaveChangesAsync();
    }
}
