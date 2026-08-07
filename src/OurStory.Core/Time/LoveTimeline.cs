// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

namespace OurStory.Core.Time;

/// <summary>
/// 相恋时间线计算工具
/// </summary>
public static class LoveTimeline {
    /// <summary>
    /// 计算从相恋开始时刻到目标时刻已经完整经过的天数。
    ///
    /// 该口径与首页的“天 / 小时 / 分钟 / 秒”计时器一致：
    /// 未满 24 小时为 0 天，满 24 小时为 1 天。
    /// </summary>
    /// <param name="moment">需要计算的目标时间</param>
    /// <param name="loveStartedAt">相恋开始时间</param>
    /// <returns>已经完整经过的 24 小时天数；目标时间较早时返回 0</returns>
    public static int ElapsedDays(DateTime moment, DateTime loveStartedAt) {
        var elapsedTicks = moment.Ticks - loveStartedAt.Ticks;
        return elapsedTicks <= 0 ? 0 : (int)(elapsedTicks / TimeSpan.TicksPerDay);
    }

    /// <summary>
    /// 计算相恋第几天
    /// 
    /// 计算规则：
    /// - 从在一起当天开始计算，当天记为第 1 天；
    /// - 仅比较日期部分，不比较具体时刻；
    /// - 在一起当天无论几点开始，都视为第 1 天；
    /// - 当前日期早于开始日期时返回 0，调用方可根据该结果决定是否显示
    /// 
    /// 示例：
    /// 在一起时间：2026-08-01 20:00
    /// 
    /// 2026-08-01 返回 1；
    /// 2026-08-02 返回 2；
    /// 2026-07-31 返回 0。
    /// </summary>
    /// <param name="moment">需要计算的目标时间</param>
    /// <param name="loveStartedAt">相恋开始时间</param>
    /// <returns>
    /// 相恋天数。
    /// 
    /// 返回值：
    /// - 大于等于 1：表示相恋第几天；
    /// - 0：表示目标时间早于相恋开始日期
    /// </returns>
    public static int DayNumber(DateTime moment, DateTime loveStartedAt) {
        var start = DateOnly.FromDateTime(loveStartedAt);
        var day = DateOnly.FromDateTime(moment);

        return day < start ? 0 : day.DayNumber - start.DayNumber + 1;
    }
}
