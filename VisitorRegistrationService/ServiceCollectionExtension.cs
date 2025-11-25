using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using VisitorRegistrationApi.Dtos.Company;
using VisitorRegistrationApi.Validators.Company;
using VisitorRegistrationService.Dtos.Employee;
using VisitorRegistrationService.Dtos.Visitor;
using VisitorRegistrationService.Validators.Employee;
using VisitorRegistrationService.Validators.Visitor;

namespace VisitorRegistrationService
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddVisitorRegistrationService(this IServiceCollection services)
        {
            services.AddTransient<CompanyService>();
            services.AddTransient<EmployeeService>();
            services.AddTransient<VisitorService>();

            // Validators
            services.AddScoped<IValidator<CreateCompanyDto>, CreateCompanyValidator>();
            services.AddScoped<IValidator<UpdateCompanyDto>, UpdateCompanyValidator>();

            services.AddScoped<IValidator<CreateEmployeeDto>, CreateEmployeeValidator>();
            services.AddScoped<IValidator<UpdateEmployeeDto>, UpdateEmployeeValidator>();

            services.AddScoped<IValidator<CreateVisitorDto>, CreateVisitorValidator>();
            services.AddScoped<IValidator<UpdateVisitorDto>, UpdateVisitorValidator>();

            return services;
        }
    }
}
