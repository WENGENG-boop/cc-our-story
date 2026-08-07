// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using OurStory.Core;

namespace OurStory.Services.Heartbeats;

/// <summary>
/// 「谁在点这颗爱心」。
///
/// 身份由服务端的登录态决定，前端改不了：主题设置里绑定的两个账号分别是男主和女主，
/// 其余所有人（含未登录）都是访客。访客靠一串指纹区分，不存原始 IP
/// </summary>
/// <param name="Role">boy / girl / guest</param>
/// <param name="UserId">登录用户的主键，访客为 null</param>
/// <param name="VisitorHash">访客指纹，登录用户为空串</param>
public readonly record struct VisitorIdentity(UserRole Role, int? UserId, string VisitorHash) {
    /// <summary>
    /// 创建访客身份
    /// </summary>
    public static VisitorIdentity Guest(string visitorHash) => new(UserRole.Guest, null, visitorHash);
}
