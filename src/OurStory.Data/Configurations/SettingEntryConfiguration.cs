// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OurStory.Core.Entities;

namespace OurStory.Data.Configurations;

/// <summary>
/// 表示 SettingEntryConfiguration
/// </summary>
public class SettingEntryConfiguration : IEntityTypeConfiguration<SettingEntry> {
    /// <summary>
    /// 配置站点设置实体的数据库映射
    /// </summary>
    public void Configure(EntityTypeBuilder<SettingEntry> builder) {
        _ = builder.ToTable("settings");

        _ = builder.HasKey(entry => entry.Key);

        _ = builder.Property(entry => entry.Key).HasMaxLength(64);
        _ = builder.Property(entry => entry.Value).IsRequired();
    }
}
