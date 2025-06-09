using FluentValidation;
using VisitorRegistrationApi.Dtos.Company;
using VisitorRegistrationData.Interfaces;
using VisitorRegistrationShared.Extensions;

namespace VisitorRegistrationApi.Validators.Company
{
    public class UpdateCompanyValidator : AbstractValidator<UpdateCompanyDto>
    {
        private readonly ICompanyRepository companyRepository;

        public UpdateCompanyValidator(ICompanyRepository companyRepository)
        {
            this.companyRepository = companyRepository;

            RuleFor(c => c).NotNull().WithMessage("Company cannot be null");

            RuleFor(c => c.Id)
                .GreaterThan(0).WithMessage("Company ID must be greater than 0");


            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Company name is required")
                .MaximumLength(50).WithMessage("Company name cannot be longer than 50 characters")
                .MinimumLength(2).WithMessage("Company name has to be 2 or more characters long")
                .Matches(@"^[A-Za-z0-9\s&@!'’\-\.]+$").WithMessage("Company name contains invalid characters")
                .MustAsync(async (dto, name, cancellation) =>
                {
                    var nameToCheck = name.NormalizeForComparison();

                    var companies = await companyRepository.GetAllCompanies();

                    return !companies
                        .Where(c => !c.IsDeleted)
                        .Any(c => c.Name.NormalizeForComparison() == nameToCheck && c.Id != dto.Id);
                }).WithMessage("A company with this name already exists");

        }
    }
}
