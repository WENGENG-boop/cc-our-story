// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using OurStory.Core.Options;

namespace OurStory.Core.Time;

/// <summary>
/// 站点时区下的「现在」和「今天」。
///
/// 数据库里一律存 UTC，展示和分日统计一律走这里换算。
/// 服务器换个时区部署，历史数据的归属不会跟着变。
/// </summary>
public class SiteClock {
    private readonly TimeZoneInfo _timeZone;

    /// <summary>
    /// 执行 SiteClock 操作
    /// </summary>
    public SiteClock(OurStoryOptions options) {
        ArgumentNullException.ThrowIfNull(options);
        _timeZone = Resolve(options.TimeZone);
    }

    /// <summary>
    /// 获取 TimeZone
    /// </summary>
    public TimeZoneInfo TimeZone => _timeZone;

    /// <summary>
    /// 获取 UtcNow
    /// </summary>
    public static DateTimeOffset UtcNow => DateTimeOffset.UtcNow;

    /// <summary>
    /// 获取站点时区的当前时间
    /// </summary>
    public DateTime LocalNow => ToLocal(DateTimeOffset.UtcNow);

    /// <summary>
    /// 获取站点时区的今天
    /// </summary>
    public DateOnly Today => DateOnly.FromDateTime(LocalNow);

    /// <summary>
    /// 转换Local
    /// </summary>
    public DateTime ToLocal(DateTimeOffset instant) =>
        TimeZoneInfo.ConvertTime(instant, _timeZone).DateTime;

    /// <summary>
    /// 把一个「站点时区的本地时间」还原成可入库的 UTC 时刻
    /// </summary>
    public DateTimeOffset ToUtc(DateTime local) {
        var unspecified = DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
        return new DateTimeOffset(unspecified, _timeZone.GetUtcOffset(unspecified)).ToUniversalTime();
    }

    /// <summary>
    /// 某个时刻落在站点时区的哪一天，形如 2026-08-07
    /// </summary>
    public string DayKey(DateTimeOffset instant) => ToLocal(instant).ToString("yyyy-MM-dd");

    /// <summary>
    /// 执行 TodayKey 操作
    /// </summary>
    public string TodayKey => LocalNow.ToString("yyyy-MM-dd");

    /// <summary>
    /// Windows 和 Linux 的时区 ID 不一样，两边都试一次；
    /// 都不认就退回 UTC，宁可差几小时也不要整站起不来。
    /// </summary>
    private static TimeZoneInfo Resolve(string id) {
        if (string.IsNullOrWhiteSpace(id)) {
            return TimeZoneInfo.Utc;
        }

        try {
            return TimeZoneInfo.FindSystemTimeZoneById(id);
        } catch (Exception exception) when (exception is TimeZoneNotFoundException or InvalidTimeZoneException) {
            return TimeZoneInfo.TryConvertIanaIdToWindowsId(id, out var windowsId)
                && TryFind(windowsId, out var byWindowsId)
                    ? byWindowsId
                    : TimeZoneInfo.Utc;
        }
    }

    private static bool TryFind(string id, out TimeZoneInfo zone) {
        try {
            zone = TimeZoneInfo.FindSystemTimeZoneById(id);
            return true;
        } catch (Exception exception) when (exception is TimeZoneNotFoundException or InvalidTimeZoneException) {
            zone = TimeZoneInfo.Utc;
            return false;
        }
    }
}
