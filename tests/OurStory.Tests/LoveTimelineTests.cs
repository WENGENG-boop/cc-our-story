// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.
using OurStory.Core.Time;
using Xunit;

namespace OurStory.Tests;

/// <summary>
/// 表示 LoveTimelineTests
/// </summary>
public class LoveTimelineTests {
    private static readonly DateTime Start = new(2024, 5, 20, 20, 0, 0);

    /// <summary>
    /// 验证在一起那天算第一天()
    /// </summary>
    [Fact]
    public void 在一起那天算第一天() {
        Assert.Equal(1, LoveTimeline.DayNumber(new DateTime(2024, 5, 20, 23, 30, 0), Start));
    }

    /// <summary>
    /// 验证在一起当天更早的时刻也算第一天()
    /// </summary>
    [Fact]
    public void 在一起当天更早的时刻也算第一天() {
        // 只比日期不比时刻，否则当天下午发的记录会被判成第 0 天
        Assert.Equal(1, LoveTimeline.DayNumber(new DateTime(2024, 5, 20, 8, 0, 0), Start));
    }

    /// <summary>
    /// 验证第二天算第二天()
    /// </summary>
    [Fact]
    public void 第二天算第二天() {
        Assert.Equal(2, LoveTimeline.DayNumber(new DateTime(2024, 5, 21, 0, 5, 0), Start));
    }

    /// <summary>
    /// 验证早于在一起的日子返回零()
    /// </summary>
    [Fact]
    public void 早于在一起的日子返回零() {
        Assert.Equal(0, LoveTimeline.DayNumber(new DateTime(2024, 5, 19, 23, 59, 0), Start));
    }

    /// <summary>
    /// 验证跨年也能正确累加()
    /// </summary>
    [Fact]
    public void 跨年也能正确累加() {
        Assert.Equal(227, LoveTimeline.DayNumber(new DateTime(2025, 1, 1), Start));
    }

    /// <summary>
    /// 验证文章经过天数与首页计时器保持一致()
    /// </summary>
    [Fact]
    public void 文章经过天数与首页计时器保持一致() {
        var moment = new DateTime(2026, 8, 7, 15, 21, 48);

        Assert.Equal(808, LoveTimeline.ElapsedDays(moment, Start));
    }

    /// <summary>
    /// 验证未满二十四小时不计为完整一天()
    /// </summary>
    [Fact]
    public void 未满二十四小时不计为完整一天() {
        Assert.Equal(0, LoveTimeline.ElapsedDays(new DateTime(2024, 5, 21, 19, 59, 59), Start));
        Assert.Equal(1, LoveTimeline.ElapsedDays(new DateTime(2024, 5, 21, 20, 0, 0), Start));
    }
}
