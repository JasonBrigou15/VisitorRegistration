using FluentValidation;
using VisitorRegistrationApi.Dtos.Company;
using VisitorRegistrationData.Interfaces;
using VisitorRegistrationShared.Extensions;

namespace VisitorRegistrationApi.Validators.Company
{
    public class CreateCompanyValidator : AbstractValidator<CreateCompanyDto>
    {
        private readonly ICompanyRepository companyRepository;

        public CreateCompanyValidator(ICompanyRepository companyRepository)
        {
            this.companyRepository = companyRepository;

            RuleFor(c => c).NotNull().WithMessage("Company cannot be null");

            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Company name is required")
                .MaximumLength(50).WithMessage("Company name cannot be longer than 50 characters")
                .MinimumLength(2).WithMessage("Company name has to be 2 or more characters long")
                .Matches(@"^[A-Za-z0-9\s&@!'’\-\.]+$").WithMessage("Company name contains invalid characters")
                .MustAsync((name, Cancellation) => CompanyNameDoesNotExistAsync(name)).WithMessage("A Company with this name already exists");

        }

        private async Task<bool> CompanyNameDoesNotExistAsync(string name)
        {
            var nameToCheck = name.NormalizeForComparison();

            var companies = await companyRepository.GetAllCompanies();

            return !companies.Where(c => !c.IsDeleted).Any(c => c.Name.NormalizeForComparison() == nameToCheck);
        }
    }
}
