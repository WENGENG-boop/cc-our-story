// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace OurStory.Core.Models;

/// <summary>
/// 首页心动面板上的四个数字
/// </summary>
public class HeartbeatStats {
    /// <summary>
    /// 获取或设置累计次数，访客视角是所有访客的聚合
    /// </summary>
    [JsonPropertyName("total")]
    public int Total { get; set; }

    /// <summary>
    /// 获取或设置当日次数，访客视角是所有访客的聚合
    /// </summary>
    [JsonPropertyName("today")]
    public int Today { get; set; }

    /// <summary>
    /// 获取或设置双方账号是连续天数，访客视角是送过祝福的人数
    /// </summary>
    [JsonPropertyName("streak")]
    public int Streak { get; set; }

    /// <summary>
    /// 获取或设置双方账号是心动天数，访客视角是单日最高
    /// </summary>
    [JsonPropertyName("best")]
    public int Best { get; set; }
}

/// <summary>
/// 首页渲染和心动接口回执共用的一份数据
/// </summary>
/// <remarks>
/// <see cref="Self"/> 永远是「当前这个人」自己的统计，用来判断每日上限
/// <see cref="Display"/> 是页面真正展示的那一份：双方账号看自己，访客看所有访客的聚合
/// </remarks>
public class HeartbeatSummary {
    /// <summary>
    /// 获取或设置当前访问者的身份，可能是 boy / girl / guest
    /// </summary>
    [JsonPropertyName("role")]
    public string Role { get; set; } = "guest";

    /// <summary>
    /// 获取或设置当日日期，形如 2026-08-07
    /// </summary>
    [JsonPropertyName("today")]
    public string Today { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置每日上限，访客视角是所有访客的聚合
    /// </summary>
    [JsonPropertyName("dailyLimit")]
    public int DailyLimit { get; set; }

    /// <summary>
    /// 获取或设置三种身份各自的累计次数，键是 boy / girl / guest
    /// </summary>
    [JsonPropertyName("totals")]
    public Dictionary<string, int> Totals { get; set; } = [];

    /// <summary>
    /// 获取或设置当前访问者自己的统计数据
    /// </summary>
    [JsonPropertyName("self")]
    public HeartbeatStats Self { get; set; } = new();

    /// <summary>
    /// 获取或设置页面上展示的统计数据，访客视角是所有访客的聚合
    /// </summary>
    [JsonPropertyName("display")]
    public HeartbeatStats Display { get; set; } = new();
}

/// <summary>一批点击写进去的结果</summary>
/// <param name="Accepted">实际记下了几次</param>
/// <param name="Limited">是否有点击因为撞到当日上限被丢掉</param>
public readonly record struct HeartbeatRecordResult(int Accepted, bool Limited);
