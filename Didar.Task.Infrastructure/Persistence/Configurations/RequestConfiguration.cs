using Didar.Task.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Didar.Task.Infrastructure.Persistence.Configurations;


public sealed class RequestConfiguration
    : IEntityTypeConfiguration<Request>
{
    public void Configure(EntityTypeBuilder<Request> builder)
    {
        builder.ToTable("Requests");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.RequestKey)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => x.RequestKey)
            .IsUnique();

        builder.Property(x => x.Subject)
            .HasConversion<int>();

        builder.Property(x => x.Priority)
            .HasConversion<int>();

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.Status,
            x.Priority,
            x.CreatedAt
        });

        builder.HasIndex(x => new
        {
            x.AssignedExpertId,
            x.Status
        });
    }
}