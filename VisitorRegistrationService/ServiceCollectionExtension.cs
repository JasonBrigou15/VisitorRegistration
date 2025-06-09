using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using VisitorRegistrationApi.Dtos.Company;
using VisitorRegistrationApi.Validators.Company;
using VisitorRegistrationService.Dtos.Employee;
using VisitorRegistrationService.Validators.Employee;

namespace VisitorRegistrationService
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddVisitorRegistrationService(this IServiceCollection services)
        {
            services.AddTransient<CompanyService>();
            services.AddTransient<EmployeeService>();

            // Validators
            services.AddScoped<IValidator<CreateCompanyDto>, CreateCompanyValidator>();
            services.AddScoped<IValidator<UpdateCompanyDto>, UpdateCompanyValidator>();

            services.AddScoped<IValidator<CreateEmployeeDto>, CreateEmployeeValidator>();
            services.AddScoped<IValidator<UpdateEmployeeDto>, UpdateEmployeeValidator>();

            return services;
        }
    }
}
