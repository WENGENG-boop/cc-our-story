// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using OurStory.Core.Models;

namespace OurStory.Services.Heartbeats;

/// <summary>
/// 提供心动记录统计、记录提交等业务能力
/// </summary>
public interface IHeartbeatService {
    /// <summary>
    /// 异步获取心动统计摘要
    /// </summary>
    /// <param name="who">访问者身份信息</param>
    /// <param name="cancellationToken">异步取消令牌</param>
    /// <returns>异步操作任务结果，心动统计摘要</returns>
    Task<HeartbeatSummary> GetSummaryAsync(VisitorIdentity who, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步记录心动点击。
    /// 
    /// 单次请求最多接受指定数量的点击记录，
    /// 超出每日限制的记录会被忽略
    /// </summary>
    /// <param name="who">访问者身份信息</param>
    /// <param name="agesMs">
    /// 点击发生时间距离当前时间的毫秒数列表。
    /// 服务端会限制最大允许回溯时间
    /// </param>
    /// <param name="cancellationToken">异步取消令牌</param>
    /// <returns>
    /// 异步操作任务结果，一个记录结果，包括实际接受数量以及是否存在未接受记录
    /// </returns>
    Task<HeartbeatRecordResult> RecordAsync(
        VisitorIdentity who,
        IReadOnlyList<int> agesMs,
        CancellationToken cancellationToken = default);
}
