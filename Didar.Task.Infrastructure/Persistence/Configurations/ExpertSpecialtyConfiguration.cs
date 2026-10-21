using Didar.Task.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Didar.Task.Infrastructure.Persistence.Configurations;

public sealed class ExpertSpecialtyConfiguration
    : IEntityTypeConfiguration<ExpertSpecialty>
{
    public void Configure(EntityTypeBuilder<ExpertSpecialty> builder)
    {
        builder.ToTable("ExpertSpecialties");

        builder.HasKey(x => new { x.ExpertId, x.Specialty });

        builder.Property(x => x.Specialty)
            .HasConversion<int>()
            .IsRequired();
    }
}