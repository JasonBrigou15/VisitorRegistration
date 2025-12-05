using FluentValidation;
using VisitorRegistrationData.Interfaces;
using VisitorRegistrationShared.Dtos.Employee;

namespace VisitorRegistrationService.Validators.Employee
{
    public class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeDto>
    {
        private readonly IEmployeeRepository employeeRepository;

        public UpdateEmployeeValidator(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;

            RuleFor(e => e.Id).NotNull().WithMessage("Employee ID is required")
                .GreaterThan(0).WithMessage("Employee ID must be greater than 0");

            RuleFor(e => e.FirstName)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot be longer than 50 characters")
                .MinimumLength(2).WithMessage("First name has to be 2 or more characters long")
                .Matches(@"^[\p{L}][\p{L} '\-]*[\p{L}]$").WithMessage("First name can only contain letters, spaces, apostrophes, and hyphens.");

            RuleFor(e => e.LastName)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot be longer than 50 characters")
                .MinimumLength(2).WithMessage("Last name has to be 2 or more characters long")
                .Matches(@"^[\p{L}][\p{L} '\-]*[\p{L}]$").WithMessage("Last name can only contain letters, spaces, apostrophes, and hyphens.");

            RuleFor(e => e.Title)
                .NotEmpty().WithMessage("Title is required")
                .Matches(@"^[\p{L}][\p{L} '\-]*([\p{L}]|\b)$").WithMessage("Title can only contain letters, spaces, apostrophes, and hyphens.");

            RuleFor(e => e.CompanyId)
                .GreaterThan(0).WithMessage("A valid company must be selected.");
        }
    }
}
