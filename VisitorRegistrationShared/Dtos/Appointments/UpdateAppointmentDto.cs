namespace VisitorRegistrationShared.Dtos.Appointments
{
    public class UpdateAppointmentDto
    {
        public required int Id { get; set; }
        public DateTime AppointmentStartDate { get; set; }
        public DateTime AppointmentEndDate { get; set; }

        public int EmployeeId { get; set; }
        public int CompanyId { get; set; }
    }
}
