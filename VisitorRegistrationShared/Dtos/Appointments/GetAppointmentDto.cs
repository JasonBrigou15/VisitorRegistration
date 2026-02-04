namespace VisitorRegistrationShared.Dtos.Appointments
{
    public class GetAppointmentDto
    {
        public DateTime AppointmentStartDate { get; set; } = DateTime.Now;

        public DateTime AppointmentEndDate { get; set; } = DateTime.Now.AddHours(1);

        public int Id { get; set; }

        public string VisitorFirstname { get; set; } = string.Empty;

        public string VisitorLastname { get; set; } = string.Empty;

        public string EmployeeFirstname { get; set; } = string.Empty;

        public string EmployeeLastname { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        public int VisitorId { get; set; }
        public int EmployeeId { get; set; }
        public int CompanyId { get; set; }
    }
}
