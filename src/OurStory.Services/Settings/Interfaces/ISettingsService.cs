// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using OurStory.Core.Models;

namespace OurStory.Services.Settings;

/// <summary>
/// 定义站点配置服务契约
/// 提供站点配置读取、保存以及原始键值管理能力
/// </summary>
public interface ISettingsService {
    /// <summary>
    /// 异步获取站点配置
    /// 配置不存在时返回默认配置
    /// </summary>
    /// <param name="cancellationToken">异步取消令牌</param>
    /// <returns>站点配置模型</returns>
    Task<SiteSettings> GetAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步保存站点配置
    /// 保存完成后刷新配置缓存
    /// </summary>
    /// <param name="settings">需要保存的站点配置</param>
    /// <param name="cancellationToken">异步取消令牌</param>
    /// <returns>表示站点配置保存完成后的异步任务</returns>
    Task SaveAsync(SiteSettings settings, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取指定配置键的原始值
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="cancellationToken">异步取消令牌</param>
    /// <returns>配置值，不存在时返回 null</returns>
    Task<string?> GetRawAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步设置指定配置键的原始值
    /// 配置键不存在时创建，存在时更新
    /// </summary>
    /// <param name="key">配置键</param>
    /// <param name="value">配置值</param>
    /// <param name="cancellationToken">异步取消令牌</param>
    /// <returns>表示配置值保存完成后的异步任务</returns>
    Task SetRawAsync(string key, string value, CancellationToken cancellationToken = default);
}
