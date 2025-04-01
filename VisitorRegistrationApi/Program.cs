
using FluentValidation;
using VisitorRegistrationApi.Dtos.Company;
using VisitorRegistrationApi.Validators.Company;
using VisitorRegistrationData;

namespace VisitorRegistrationApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ServiceCollectionExtender - Data
            builder.Services.AddVisitorRegistrationData(builder.Configuration);
            
            // Validators
            builder.Services.AddScoped<IValidator<CreateCompanyDto>, CreateCompanyValidator>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
