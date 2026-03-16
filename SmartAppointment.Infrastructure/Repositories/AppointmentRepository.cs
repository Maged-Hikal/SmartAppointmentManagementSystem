using Microsoft.EntityFrameworkCore;
using SmartAppointment.Application.Interfaces;
using SmartAppointment.Domain.Entities;
using SmartAppointment.Infrastructure.Data;

namespace SmartAppointment.Infrastructure.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppDbContext _context;
        public AppointmentRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Appointment>> GetAllAsync()
        {
            return await _context.Appointments.ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAllByUserAsync(string userId)
        {
            return await _context.Appointments
                .Where(a => a.UserId == userId.ToString())
                .ToListAsync();
        }
        public async Task<Appointment?> GetByIdAsync(Guid id)
        {
            return await _context.Appointments.FirstOrDefaultAsync(a => a.Id == id);
        }
        public async Task AddAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
        }
        public async Task DeleteAsync(Appointment appointment)
        {
            if (appointment == null)
                return;

            _context.Appointments.Remove(appointment);
            await Task.CompletedTask;

        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }



}
