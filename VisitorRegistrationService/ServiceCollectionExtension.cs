using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using VisitorRegistrationApi.Dtos.Company;
using VisitorRegistrationApi.Validators.Company;

namespace VisitorRegistrationService
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddVisitorRegistrationService(this IServiceCollection services)
        {
            services.AddTransient<CompanyService>();

            // Validators
            services.AddScoped<IValidator<CreateCompanyDto>, CreateCompanyValidator>();
            services.AddScoped<IValidator<UpdateCompanyDto>, UpdateCompanyValidator>();

            return services;
        }
    }
}
