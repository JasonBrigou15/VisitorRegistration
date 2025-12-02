namespace VisitorRegistrationData.Entities
{
    public class Appointment
    {
        public int Id { get; set; }
        
        public required DateTime AppointmentStartDate { get; set; } = DateTime.Now;
        public required DateTime AppointmentEndDate { get; set; } = DateTime.Now.AddHours(1);

        public Visitor Visitor { get; set; } = null!;
        public int VisitorId { get; set; } = 0;

        public Employee Employee { get; set; } = null!;
        public int EmployeeId { get; set; } = 0;

        public required Company Company { get; set; } = null!;
        public int CompanyId { get; set; } = 0;

        public bool IsCancelled { get; set; } = false;

    }
}
