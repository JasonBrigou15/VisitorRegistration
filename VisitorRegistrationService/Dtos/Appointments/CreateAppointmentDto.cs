namespace VisitorRegistrationService.Dtos.Appointments
{
    public class CreateAppointmentDto
    {
        public required DateTime AppointmentStartDate { get; set; } = DateTime.Now;
        public required DateTime AppointmentEndDate { get; set; } = DateTime.Now.AddHours(1);

        public required int VisitorId { get; set; } = 0;
        public required int EmployeeId { get; set; } = 0;
        public required int CompanyId { get; set; } = 0;
    }
}
