using FluentValidation;
using VisitorRegistrationService.Dtos.Appointments;

namespace VisitorRegistrationService.Validators.Appointment
{
    public class UpdateAppointmentValidator : AbstractValidator<UpdateAppointmentDto>
    {
        public UpdateAppointmentValidator()
        {
            RuleFor(a => a.Id)
                .GreaterThan(0).WithMessage("Appointment ID must be greater than 0.")
                .NotEmpty().WithMessage("Appointment ID is required.");

            RuleFor(a => a.AppointmentStartDate)
                .NotEmpty().WithMessage("Appointment start date is required.")
                .GreaterThan(DateTime.Now).WithMessage("Appointment start date cannot be in the past.")
                .LessThan(a => a.AppointmentEndDate)
                .WithMessage("Appointment start date must be earlier than end date.");

            RuleFor(a => a.AppointmentEndDate)
                .NotEmpty().WithMessage("Appointment end date is required.")
                .GreaterThan(a => a.AppointmentStartDate)
                .WithMessage("Appointment end date must be later than start date.");

            RuleFor(a => a.EmployeeId)
                .NotEmpty().WithMessage("Employee ID is required.")
                .GreaterThan(0).WithMessage("Employee ID must be greater than zero.");

            RuleFor(a => a.CompanyId)
                .NotEmpty().WithMessage("Company ID is required.")
                .GreaterThan(0).WithMessage("Company ID must be greater than zero.");
        }
    }
}
