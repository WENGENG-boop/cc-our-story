// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurStory.Core.Entities;

namespace OurStory.Data.Configurations;

/// <summary>
/// 表示 HeartbeatConfiguration
/// </summary>
public class HeartbeatConfiguration : IEntityTypeConfiguration<Heartbeat> {
    /// <summary>
    /// 配置心动记录实体的数据库映射
    /// </summary>
    public void Configure(EntityTypeBuilder<Heartbeat> builder) {
        _ = builder.ToTable("heartbeats");

        _ = builder.HasKey(beat => beat.Id);

        _ = builder.Property(beat => beat.VisitorHash).HasMaxLength(64).IsRequired();
        _ = builder.Property(beat => beat.ClickDay).HasMaxLength(10).IsRequired();

        // 三条按天的索引分别对应：身份聚合、登录用户的额度、访客的额度
        _ = builder.HasIndex(beat => new { beat.Role, beat.ClickDay });
        _ = builder.HasIndex(beat => new { beat.UserId, beat.ClickDay });
        _ = builder.HasIndex(beat => new { beat.VisitorHash, beat.ClickDay });

        _ = builder.HasOne(beat => beat.User)
            .WithMany()
            .HasForeignKey(beat => beat.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
