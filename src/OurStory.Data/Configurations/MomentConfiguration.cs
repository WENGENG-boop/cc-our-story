// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurStory.Core.Entities;

namespace OurStory.Data.Configurations;

/// <summary>
/// 表示 MomentConfiguration
/// </summary>
public class MomentConfiguration : IEntityTypeConfiguration<Moment> {
    /// <summary>
    /// 配置动态实体的数据库映射
    /// </summary>
    public void Configure(EntityTypeBuilder<Moment> builder) {
        _ = builder.ToTable("moments");

        _ = builder.HasKey(moment => moment.Id);

        _ = builder.Property(moment => moment.Title).HasMaxLength(180).IsRequired();
        _ = builder.Property(moment => moment.Slug).HasMaxLength(80).IsRequired();
        _ = builder.Property(moment => moment.Summary).HasMaxLength(400);
        _ = builder.Property(moment => moment.CoverUrl).HasMaxLength(500);
        _ = builder.Property(moment => moment.Mood).HasMaxLength(32);
        _ = builder.Property(moment => moment.Location).HasMaxLength(120);
        _ = builder.Property(moment => moment.Password).HasMaxLength(128);

        _ = builder.HasIndex(moment => moment.Slug).IsUnique();

        // 列表页固定按「发生日期倒序」翻页，这条复合索引就是给它用的
        _ = builder.HasIndex(moment => new { moment.Status, moment.MomentDate });

        _ = builder.HasOne(moment => moment.Author)
            .WithMany(user => user.Moments)
            .HasForeignKey(moment => moment.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
