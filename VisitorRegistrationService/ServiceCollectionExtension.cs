using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using VisitorRegistrationApi.Validators.Company;
using VisitorRegistrationService.Validators.Appointment;
using VisitorRegistrationService.Validators.Employee;
using VisitorRegistrationService.Validators.Visitor;
using VisitorRegistrationShared.Dtos.Appointments;
using VisitorRegistrationShared.Dtos.Company;
using VisitorRegistrationShared.Dtos.Employee;
using VisitorRegistrationShared.Dtos.Visitor;

namespace VisitorRegistrationService
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddVisitorRegistrationService(this IServiceCollection services)
        {
            services.AddTransient<CompanyService>();
            services.AddTransient<EmployeeService>();
            services.AddTransient<VisitorService>();
            services.AddTransient<AppointmentService>();
            services.AddTransient<AdminService>();

            // Validators
            services.AddScoped<IValidator<CreateCompanyDto>, CreateCompanyValidator>();
            services.AddScoped<IValidator<UpdateCompanyDto>, UpdateCompanyValidator>();

            services.AddScoped<IValidator<CreateEmployeeDto>, CreateEmployeeValidator>();
            services.AddScoped<IValidator<UpdateEmployeeDto>, UpdateEmployeeValidator>();

            services.AddScoped<IValidator<CreateVisitorDto>, CreateVisitorValidator>();
            services.AddScoped<IValidator<UpdateVisitorDto>, UpdateVisitorValidator>();

            services.AddScoped<IValidator<CreateAppointmentDto>, CreateAppointmentValidator>();
            services.AddScoped<IValidator<UpdateAppointmentDto>, UpdateAppointmentValidator>();

            return services;
        }
    }
}
