// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

namespace OurStory.Core.Entities;

/// <summary>
/// 一条「点点滴滴」
/// </summary>
/// <remarks>
/// 正文按 Markdown 存原文，渲染后的 HTML 一起缓存在 <see cref="ContentHtml"/>，
/// 这样列表页和详情页都不必在每次请求时重新解析 Markdown。
/// </remarks>
public class Moment {
    /// <summary>
    /// 获取或设置动态的唯一标识符
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 获取或设置动态的标题
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置地址里用的短名，全站唯一
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置 Markdown 原文，后台编辑的就是它
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置由 <see cref="Content"/> 渲染出来的 HTML，保存时一并更新
    /// </summary>
    public string ContentHtml { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置手写摘要；留空时由正文自动截取
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// 获取或设置封面图；留空时取正文里的第一张图
    /// </summary>
    public string? CoverUrl { get; set; }

    /// <summary>
    /// 获取或设置此刻心情，例如「开心」「想念」
    /// </summary>
    public string? Mood { get; set; }

    /// <summary>
    /// 获取或设置地点，例如「北京 · 颐和园」
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// 获取或设置事情发生的日期，和发布时间分开，可以补记很久以前的事
    /// </summary>
    public DateTimeOffset MomentDate { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 获取或设置访问密码；非空即为「上锁」，列表页只露出标题
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 获取或设置动态的状态
    /// </summary>
    public MomentStatus Status { get; set; } = MomentStatus.Published;

    /// <summary>
    /// 获取或设置是否允许留言
    /// </summary>
    public bool AllowComment { get; set; } = true;

    /// <summary>
    /// 获取或设置动态的浏览次数
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// 获取或设置动态的评论数
    /// </summary>
    public int AuthorId { get; set; }

    /// <summary>
    /// 获取或设置动态的作者
    /// </summary>
    public User? Author { get; set; }

    /// <summary>
    /// 获取或设置动态的创建时间
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 获取或设置动态的最后更新时间
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// 获取或设置动态的评论列表
    /// </summary>
    public ICollection<Comment> Comments { get; set; } = [];

    /// <summary>
    /// 获取是否设了访问密码
    /// </summary>
    public bool IsProtected => !string.IsNullOrEmpty(Password);
}
