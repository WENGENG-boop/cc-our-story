// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

namespace OurStory.Core;

/// <summary>
/// 站点里的三种身份。用于描述「当前请求是谁发来的」
/// </summary>
public enum UserRole {
    /// <summary>
    /// 获取或设置 Guest
    /// </summary>
    Guest = 0,
    /// <summary>
    /// 获取或设置 Boy
    /// </summary>
    Boy = 1,
    /// <summary>
    /// 获取或设置 Girl
    /// </summary>
    Girl = 2
}

/// <summary>
/// 点点滴滴的发布状态
/// </summary>
public enum MomentStatus {
    /// <summary>
    /// 草稿状态
    /// </summary>
    Draft = 0,

    /// <summary>
    /// 已发布状态
    /// </summary>
    Published = 1
}
