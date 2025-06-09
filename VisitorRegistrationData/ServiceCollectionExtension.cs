using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VisitorRegistrationData.Interfaces;
using VisitorRegistrationData.Repositories;

namespace VisitorRegistrationData
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddVisitorRegistrationData(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<VisitorRegistrationDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("VisitorRegistrationDb"),
                x => x.MigrationsAssembly("VisitorRegistrationData")));

            services.AddTransient<ICompanyRepository, CompanyRepository>();
            services.AddTransient<IEmployeeRepository, EmployeeRepository>();

            return services;
        }
    }
}
