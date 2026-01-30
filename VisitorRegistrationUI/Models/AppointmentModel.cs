using System.ComponentModel.DataAnnotations;

namespace VisitorRegistrationUI.Models
{
    public class AppointmentModel
    {
        [Required(ErrorMessage = "Kies een datum")]
        public DateTime? AppointmentDate { get; set; }

        [Required(ErrorMessage = "Kies een starttijd")]
        public DateTime? StartTime { get; set; }

        [Required(ErrorMessage = "Kies een eindtijd")]
        public DateTime? EndTime { get; set; }
        [Required(ErrorMessage = "Kies een bedrijf")]
        public string CompanyId { get; set; } = string.Empty;
        [Required(ErrorMessage = "Kies een werknemer")]
        public string EmployeeId { get; set; } = string.Empty;
    }

}
