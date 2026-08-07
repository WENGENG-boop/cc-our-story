// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

namespace OurStory.Core.Entities;

/// <summary>
/// 首页那颗爱心的一次点击
/// </summary>
public class Heartbeat {
    /// <summary>
    /// 获取或设置点击的唯一标识符
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 获取或设置点的人是谁。访客统一记 <see cref="UserRole.Guest"/>
    /// </summary>
    public UserRole Role { get; set; } = UserRole.Guest;

    /// <summary>
    /// 获取或设置登录用户的主键；访客为 null。
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// 获取或设置登录用户；访客为 null
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// 获取或设置访客指纹（IP + UA + 站点密钥的哈希），登录用户为空串
    /// </summary>
    public string VisitorHash { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置点击的时间戳
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 获取或设置点击落在站点时区的哪一天，形如 2026-08-07；每日上限和连续天数都靠它
    /// </summary>
    public string ClickDay { get; set; } = string.Empty;
}
