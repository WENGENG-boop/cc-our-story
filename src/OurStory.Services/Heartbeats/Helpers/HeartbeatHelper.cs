// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using OurStory.Core;

namespace OurStory.Services.Heartbeats;

/// <summary>
/// 心动记录辅助计算方法
/// </summary>
internal static class HeartbeatHelper {
    /// <summary>
    /// 计算连续心动天数。
    /// 日期列表包含今天或昨天才开始计算连续天数。
    /// </summary>
    /// <param name="days">存在心动记录的日期集合。</param>
    /// <param name="today">当前日期。</param>
    /// <returns>连续心动天数。</returns>
    public static int CalculateStreak(IEnumerable<string> days, string today) {
        var ordered = days
            .OrderByDescending(day => day, StringComparer.Ordinal)
            .ToList();

        if (ordered.Count == 0) {
            return 0;
        }

        var cursor = ordered[0];

        if (cursor != today && cursor != ShiftDay(today, -1)) {
            return 0;
        }

        var streak = 0;

        foreach (var day in ordered) {
            if (day != cursor) {
                break;
            }

            streak++;
            cursor = ShiftDay(cursor, -1);
        }

        return streak;
    }

    /// <summary>
    /// 日期偏移计算。
    /// </summary>
    /// <param name="day">日期字符串。</param>
    /// <param name="delta">偏移天数。</param>
    /// <returns>偏移后的日期字符串。</returns>
    private static string ShiftDay(string day, int delta) =>
        DateOnly.TryParse(day, out var parsed)
            ? parsed.AddDays(delta).ToString("yyyy-MM-dd")
            : day;

    /// <summary>
    /// 将用户角色转换为业务展示标识。
    /// </summary>
    /// <param name="role">用户角色。</param>
    /// <returns>角色键值。</returns>
    public static string RoleKey(UserRole role) => role switch {
        UserRole.Boy => "boy",
        UserRole.Girl => "girl",
        _ => "guest"
    };
}
