using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Infrastructure.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .IsRequired();

            builder.Property(p => p.Status)
                .IsRequired();

            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // StripeSessionId
            // StripePaymentIntentId
        }
    }
}
