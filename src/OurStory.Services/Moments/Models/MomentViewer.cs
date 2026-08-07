// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using OurStory.Core.Entities;

namespace OurStory.Services.Moments;

/// <summary>
/// 当前是谁在看这些记录
/// </summary>
/// <param name="IsOwner">已登录的男主 / 女主，能看到全部内容，包括上锁的和草稿。</param>
/// <param name="UnlockedIds">这次会话里已经输对密码的记录。</param>
public readonly record struct MomentViewer(bool IsOwner, IReadOnlySet<int> UnlockedIds) {
    /// <summary>
    /// 获取执行 Anonymous 操作
    /// </summary>
    public static MomentViewer Anonymous { get; } = new(false, new HashSet<int>());

    /// <summary>
    /// 获取一个值，指示这条记录对当前访客是否还锁着
    /// </summary>
    public bool IsLockedFor(Moment moment) =>
        moment.IsProtected && !IsOwner && !(UnlockedIds ?? new HashSet<int>()).Contains(moment.Id);
}
