using Didar.Task.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Didar.Task.Infrastructure.Persistence.Configurations;


public sealed class ExpertConfiguration
    : IEntityTypeConfiguration<Expert>
{
    public void Configure(EntityTypeBuilder<Expert> builder)
    {
        builder.ToTable("Experts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Capacity)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasMany(x => x.Specialties)
            .WithOne()
            .HasForeignKey(x => x.ExpertId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Specialties)
            .UsePropertyAccessMode(PropertyAccessMode.Property);
    }
}
