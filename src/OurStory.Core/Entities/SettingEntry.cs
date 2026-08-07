// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

namespace OurStory.Core.Entities;

/// <summary>
/// 站点设置的一项
/// </summary>
/// <remarks>
/// 新增设置项不需要迁移表结构，读写都走 SettingsService
/// </remarks>
public class SettingEntry {
    /// <summary>
    /// 获取或设置设置项的键名
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置设置项的值
    /// </summary>
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置设置项的最后更新时间
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
