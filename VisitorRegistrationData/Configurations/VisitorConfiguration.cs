using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VisitorRegistrationData.Entities;

namespace VisitorRegistrationData.Configurations
{
    public class VisitorConfiguration : IEntityTypeConfiguration<Visitor>
    {
        public void Configure(EntityTypeBuilder<Visitor> builder)
        {
            builder.Property(v => v.Firstname)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(v => v.Lastname)
               .IsRequired()
               .HasMaxLength(50);

            builder.Property(v => v.Email)
               .IsRequired()
               .HasMaxLength(100);

            builder.HasOne(v => v.Company)
                .WithMany()
                .HasForeignKey(v => v.CompanyId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(v => !v.IsDeleted);
        }
    }
}
