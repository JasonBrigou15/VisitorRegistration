using System.ComponentModel.DataAnnotations;

namespace VisitorRegistrationApi.Dtos.Company
{
    public class UpdateCompanyDto
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "ID must be higher then 0")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Company name is required")]
        [MinLength(2, ErrorMessage = "Company name must be atleast 2 letters long")]
        [StringLength(50, ErrorMessage = "Must be 50 characters or fewer")]
        [RegularExpression(@"^[A-Za-z0-9\s&@!'’\-\.]+$", ErrorMessage = "Company name contains invalid characters")]
        public string Name { get; set; } = string.Empty;
    }
}
