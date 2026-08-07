// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using OurStory.Services.Heartbeats;
using Xunit;

namespace OurStory.Tests;

/// <summary>
/// 表示 HeartbeatStreakTests
/// </summary>
public class HeartbeatStreakTests {
    /// <summary>
    /// 验证没有记录时连续天数是零()
    /// </summary>
    [Fact]
    public void 没有记录时连续天数是零() {
        Assert.Equal(0, HeartbeatHelper.CalculateStreak([], "2026-08-07"));
    }

    /// <summary>
    /// 验证连着几天都点过就数几天()
    /// </summary>
    [Fact]
    public void 连着几天都点过就数几天() {
        var days = new List<string> { "2026-08-07", "2026-08-06", "2026-08-05" };

        Assert.Equal(3, HeartbeatHelper.CalculateStreak(days, "2026-08-07"));
    }

    /// <summary>
    /// 验证昨天点过今天还没点连续记录仍然算数()
    /// </summary>
    [Fact]
    public void 昨天点过今天还没点连续记录仍然算数() {
        var days = new List<string> { "2026-08-06", "2026-08-05" };

        Assert.Equal(2, HeartbeatHelper.CalculateStreak(days, "2026-08-07"));
    }

    /// <summary>
    /// 验证断了一天就要从头开始()
    /// </summary>
    [Fact]
    public void 断了一天就要从头开始() {
        var days = new List<string> { "2026-08-04", "2026-08-03" };

        Assert.Equal(0, HeartbeatHelper.CalculateStreak(days, "2026-08-07"));
    }

    /// <summary>
    /// 验证中间断掉的那一段不计入()
    /// </summary>
    [Fact]
    public void 中间断掉的那一段不计入() {
        var days = new List<string> { "2026-08-07", "2026-08-06", "2026-08-03", "2026-08-02" };

        Assert.Equal(2, HeartbeatHelper.CalculateStreak(days, "2026-08-07"));
    }

    /// <summary>
    /// 验证日期顺序打乱也不影响结果()
    /// </summary>
    [Fact]
    public void 日期顺序打乱也不影响结果() {
        var days = new List<string> { "2026-08-05", "2026-08-07", "2026-08-06" };

        Assert.Equal(3, HeartbeatHelper.CalculateStreak(days, "2026-08-07"));
    }

    /// <summary>
    /// 验证跨月也能接上()
    /// </summary>
    [Fact]
    public void 跨月也能接上() {
        var days = new List<string> { "2026-08-01", "2026-07-31", "2026-07-30" };

        Assert.Equal(3, HeartbeatHelper.CalculateStreak(days, "2026-08-01"));
    }
}
