// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

namespace OurStory.Services.Comments;

/// <summary>
/// 页面上的一条留言，正文已经转义成可以直接输出的 HTML
/// </summary>
public class CommentNode {
    /// <summary>
    /// 获取或设置 Id
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// 获取或设置 AuthorName
    /// </summary>
    public string AuthorName { get; init; } = string.Empty;

    /// <summary>
    /// 获取或设置 AuthorUrl
    /// </summary>
    public string? AuthorUrl { get; init; }

    /// <summary>已转义并按空行分段的正文。</summary>
    public string ContentHtml { get; init; } = string.Empty;

    /// <summary>
    /// 获取或设置 CreatedAt
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>是不是站主两个人自己回的。</summary>
    public bool IsOwner { get; init; }

    /// <summary>
    /// 获取或设置 Replies
    /// </summary>
    public List<CommentNode> Replies { get; } = [];
}
