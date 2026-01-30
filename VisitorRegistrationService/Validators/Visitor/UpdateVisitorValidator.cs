using FluentValidation;
using VisitorRegistrationData.Interfaces;
using VisitorRegistrationShared.Dtos.Visitor;

namespace VisitorRegistrationService.Validators.Visitor
{
    public class UpdateVisitorValidator : AbstractValidator<UpdateVisitorDto>
    {
        private readonly IVisitorRepository visitorRepository;

        public UpdateVisitorValidator(IVisitorRepository visitorRepository)
        {
            this.visitorRepository = visitorRepository;

           RuleFor(v => v.Id).NotNull().WithMessage("Visitor ID is required")
                .GreaterThan(0).WithMessage("Visitor ID must be greater than 0");

            RuleFor(v => v.Firstname)
                .NotEmpty().WithMessage("First name is required")
                .MaximumLength(50).WithMessage("First name cannot be longer than 50 characters")
                .MinimumLength(2).WithMessage("First name has to be 2 or more characters long")
                .Matches(@"^[\p{L}][\p{L} '\-]*[\p{L}]$").WithMessage("First name can only contain letters, spaces, apostrophes, and hyphens.");

            RuleFor(v => v.Lastname)
                .NotEmpty().WithMessage("Last name is required")
                .MaximumLength(50).WithMessage("Last name cannot be longer than 50 characters")
                .MinimumLength(2).WithMessage("Last name has to be 2 or more characters long")
                .Matches(@"^[\p{L}][\p{L} '\-]*[\p{L}]$").WithMessage("Last name can only contain letters, spaces, apostrophes, and hyphens.");

            RuleFor(v => v.Email)
                .NotEmpty().WithMessage("Email is required")
                .MaximumLength(100).WithMessage("Email cannot be longer than 100 characters")
                .Matches(@"^(?!.*\.\.)[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")
                .WithMessage("Invalid email format.");

            RuleFor(v => v.CompanyName)
                .MaximumLength(50).WithMessage("Company name cannot be longer than 50 characters")
                .Matches(@"^[\p{L}][\p{L} '\-]*[\p{L}]$").WithMessage("Company name can only contain letters, spaces, apostrophes, and hyphens.")
                .When(v => !string.IsNullOrWhiteSpace(v.CompanyName));
        }
    }
}
