using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisitorRegistrationService.Dtos.Appointments
{
    public class GetAppointmentDto
    {
        public DateTime AppointmentStartDate { get; set; } = DateTime.Now;

        public DateTime AppointmentEndDate { get; set; } = DateTime.Now.AddHours(1);

        public string VisitorFirstname { get; set; } = string.Empty;

        public string VisitorLastname { get; set; } = string.Empty;

        public string EmployeeFirstname { get; set; } = string.Empty;

        public string EmployeeLastname { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;
    }
}
