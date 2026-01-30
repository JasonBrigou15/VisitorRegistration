using System.ComponentModel.DataAnnotations;

namespace VisitorRegistrationShared.Dtos.Visitor
{
    public class CreateVisitorDto
    {
        [Required(ErrorMessage = "Voornaam is verplicht")]
        [StringLength(50, ErrorMessage = "Voornaam mag niet langer zijn dan 50 karakters")]
        public string Firstname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Achternaam is verplicht")]
        [StringLength(50, ErrorMessage = "Achternaam mag niet langer zijn dan 50 karakters")]
        public string Lastname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is verplicht")]
        [EmailAddress(ErrorMessage = "ongeldig email adres")]
        public string Email { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Bedrijf naam mag niet langer zijn dan 50 karakters")]
        public string CompanyName { get; set; } = string.Empty;
    }
}
