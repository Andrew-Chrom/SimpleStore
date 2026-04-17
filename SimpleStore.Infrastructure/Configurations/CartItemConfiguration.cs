using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleStore.Infrastructure.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Product)
                .WithMany()
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.Property(p => p.Quantity)
                .IsRequired();
        }
    }
}
