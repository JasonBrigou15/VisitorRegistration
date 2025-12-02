using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisitorRegistrationData.Entities;

namespace VisitorRegistrationData.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.Property(a => a.AppointmentStartDate)
                .IsRequired();

            builder.Property(a => a.AppointmentEndDate)
                .IsRequired();

            builder.Property(a => a.IsCancelled)
                .HasDefaultValue(false);

            builder.HasOne(a => a.Employee)
               .WithMany()
               .HasForeignKey(a => a.EmployeeId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Company)
                .WithMany()
                .HasForeignKey(a => a.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Visitor)
                .WithMany()
                .HasForeignKey(a => a.VisitorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
