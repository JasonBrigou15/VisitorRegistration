using VisitorRegistrationData.Entities;

namespace VisitorRegistrationData.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<List<Appointment>> GetAllAppointments();

        Task<Appointment?> GetAppointmentById(int id);

        Task<List<Appointment>> GetAppointmentsByVisitorId(int visitorId);

        Task<List<Appointment>> GetAppointmentsByEmployeeId(int employeeId);

        Task<Appointment> CreateAppointment(Appointment appointment);

        Task UpdateAppointment(Appointment appointment);

        Task DeleteAppointment(int id);
    }
}
