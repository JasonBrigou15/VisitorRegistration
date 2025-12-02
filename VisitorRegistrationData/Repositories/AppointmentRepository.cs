using Microsoft.EntityFrameworkCore;
using VisitorRegistrationData.Entities;
using VisitorRegistrationData.Interfaces;

namespace VisitorRegistrationData.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly VisitorRegistrationDbContext context;

        public AppointmentRepository(VisitorRegistrationDbContext context)
        {
            this.context = context;
        }

        public async Task<Appointment> CreateAppointment(Appointment appointment)
        {
            await context.Appointments.AddAsync(appointment);
            await context.SaveChangesAsync();
            return appointment;
        }

        public async Task DeleteAppointment(int id)
        {
            var appointment = await context.Appointments.FindAsync(id);
            appointment!.IsCancelled = true;
            await context.SaveChangesAsync();
        }

        public async Task<List<Appointment>> GetAllAppointments() => await context.Appointments
            .Include(a => a.Visitor)
            .Include(a => a.Employee)
            .Include(a => a.Company)
            .Where(a => !a.Visitor.IsDeleted && !a.Employee.IsDeleted && !a.Company.IsDeleted && !a.IsCancelled)
            .ToListAsync();

        public async Task<Appointment?> GetAppointmentById(int id) => await context.Appointments
            .Include(a => a.Visitor)
            .Include(a => a.Employee)
            .Include(a => a.Company)
            .Where(a => !a.Visitor.IsDeleted && !a.Employee.IsDeleted && !a.Company.IsDeleted && !a.IsCancelled)
            .SingleOrDefaultAsync(a => a.Id == id);

        public async Task<List<Appointment>> GetAppointmentsByEmployeeId(int employeeId)
        {
            return await context.Appointments
                .Where(a => !a.IsCancelled && a.EmployeeId == employeeId)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetAppointmentsByVisitorId(int visitorId)
        {
            return await context.Appointments
                .Where(a => !a.IsCancelled && a.VisitorId == visitorId)
                .ToListAsync();
        }

        public async Task UpdateAppointment(Appointment appointment)
        {
            var existingAppointment = await context.Appointments.FindAsync(appointment.Id);

            context.Entry(existingAppointment!).CurrentValues.SetValues(appointment);

            await context.SaveChangesAsync();
        }
    }
}
