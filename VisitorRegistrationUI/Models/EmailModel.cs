using System.ComponentModel.DataAnnotations;

namespace VisitorRegistrationUI.Models
{
    public class EmailModel
    {
        [Required(ErrorMessage = "Email is verplicht.")]
        [EmailAddress(ErrorMessage = "Voer een geldig emailadres in.")]
        public string Email { get; set; } = string.Empty;
    }
}
