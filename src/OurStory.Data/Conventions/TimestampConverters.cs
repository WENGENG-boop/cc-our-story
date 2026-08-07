// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace OurStory.Data.Conventions;

/// <summary>
/// 时间戳一律以「Unix 毫秒」存成整数。
///
/// SQLite 没有真正的日期类型，EF 默认把 DateTimeOffset 存成带偏移量的字符串，
/// 那种列既不能 ORDER BY 也不能安全比较大小。存整数就没有这些问题，
/// 排序、翻页、取上一篇下一篇全都能下推到 SQL 里。
/// 站内所有时刻本来就统一按 UTC 存，丢掉偏移量不会丢信息。
/// </summary>
public static class TimestampConverters {
    private static readonly ValueConverter<DateTimeOffset, long> Required = new(
        value => value.ToUniversalTime().ToUnixTimeMilliseconds(),
        value => DateTimeOffset.FromUnixTimeMilliseconds(value));

    private static readonly ValueConverter<DateTimeOffset?, long?> Optional = new(
        value => value == null ? null : value.Value.ToUniversalTime().ToUnixTimeMilliseconds(),
        value => value == null ? null : DateTimeOffset.FromUnixTimeMilliseconds(value.Value));

    /// <summary>
    /// 应用时间戳转换规则。
    /// 
    /// 遍历当前模型中的所有实体属性，
    /// 自动为 DateTimeOffset 和 DateTimeOffset? 类型属性绑定对应转换器。
    /// 
    /// 调用后，EF Core 会自动按照 Unix 毫秒格式读写时间字段，
    /// 无需在每个实体配置单独转换规则。
    /// </summary>
    /// <param name="modelBuilder">EF Core 模型构建器</param>
    public static void Apply(ModelBuilder modelBuilder) {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        foreach (var entity in modelBuilder.Model.GetEntityTypes()) {
            foreach (var property in entity.GetProperties()) {
                if (property.ClrType == typeof(DateTimeOffset)) {
                    property.SetValueConverter(Required);
                } else if (property.ClrType == typeof(DateTimeOffset?)) {
                    property.SetValueConverter(Optional);
                }
            }
        }
    }
}
