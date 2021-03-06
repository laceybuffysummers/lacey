﻿using Lacey.Medusa.Instagram.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lacey.Medusa.Instagram.Dal.EntityConfigurations
{
    internal sealed class ChannelConfiguration : IEntityTypeConfiguration<ChannelEntity>
    {
        public void Configure(EntityTypeBuilder<ChannelEntity> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.ChannelId).HasMaxLength(50).IsRequired();
            builder.Property(e => e.OriginalChannelId).HasMaxLength(50).IsRequired();
            builder.Property(e => e.Name).HasColumnName("Title").IsRequired(false);
            builder.Property(e => e.Description).IsRequired(false);
            builder.Property(e => e.PublishedAt).IsRequired(false);
            builder.Property(e => e.CreatedAt).HasDefaultValue().IsRequired();

            builder.ToTable("Channels");
        }
    }
}