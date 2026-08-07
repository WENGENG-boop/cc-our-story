// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

namespace OurStory.Core.Models;

/// <summary>
/// 列表页和首页用的一条点点滴滴
/// </summary>
/// <remarks>
/// 上锁的记录在服务层就已经把摘要和封面清空了，模板拿到什么就能显示什么，不必再各自判断一遍，也不会漏
/// </remarks>
public class MomentCard {
    /// <summary>
    /// 获取或设置 Id
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// 获取或设置 Title
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// 获取或设置 Slug
    /// </summary>
    public string Slug { get; init; } = string.Empty;

    /// <summary>
    /// 获取或设置 Excerpt
    /// </summary>
    public string Excerpt { get; init; } = string.Empty;

    /// <summary>
    /// 获取或设置 CoverUrl
    /// </summary>
    public string CoverUrl { get; init; } = string.Empty;

    /// <summary>
    /// 获取或设置 Mood
    /// </summary>
    public string Mood { get; init; } = "日常";

    /// <summary>
    /// 获取或设置 Location
    /// </summary>
    public string Location { get; init; } = string.Empty;

    /// <summary>已换算到站点时区。</summary>
    public DateTime MomentDate { get; init; }

    /// <summary>
    /// 获取或设置 IsLocked
    /// </summary>
    public bool IsLocked { get; init; }

    /// <summary>
    /// 获取或设置 AuthorName
    /// </summary>
    public string AuthorName { get; init; } = string.Empty;

    /// <summary>
    /// 获取或设置 CommentCount
    /// </summary>
    public int CommentCount { get; init; }

    /// <summary>相恋第几天，0 表示算不出来（早于在一起的日子）。</summary>
    public int LoveDay { get; init; }

    /// <summary>
    /// 获取 Url
    /// </summary>
    public string Url => "/moments/" + Slug;
}

/// <summary>
/// 详情页需要的全部内容
/// </summary>
public class MomentDetail {
    /// <summary>
    /// 获取或设置 Id
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// 获取或设置 Title
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// 获取或设置 Slug
    /// </summary>
    public string Slug { get; init; } = string.Empty;

    /// <summary>上锁且没解锁时为空串，模板会改为显示密码表单。</summary>
    public string ContentHtml { get; init; } = string.Empty;

    /// <summary>
    /// 获取或设置 Mood
    /// </summary>
    public string Mood { get; init; } = "日常";

    /// <summary>
    /// 获取或设置 Location
    /// </summary>
    public string Location { get; init; } = string.Empty;

    /// <summary>
    /// 获取或设置 MomentDate
    /// </summary>
    public DateTime MomentDate { get; init; }

    /// <summary>
    /// 获取或设置 IsLocked
    /// </summary>
    public bool IsLocked { get; init; }

    /// <summary>
    /// 获取或设置 AllowComment
    /// </summary>
    public bool AllowComment { get; init; }

    /// <summary>
    /// 获取或设置 AuthorName
    /// </summary>
    public string AuthorName { get; init; } = string.Empty;

    /// <summary>
    /// 获取或设置 LoveDay
    /// </summary>
    public int LoveDay { get; init; }

    /// <summary>
    /// 获取或设置 CommentCount
    /// </summary>
    public int CommentCount { get; init; }

    /// <summary>
    /// 获取或设置 Previous
    /// </summary>
    public MomentLink? Previous { get; init; }

    /// <summary>
    /// 获取或设置 Next
    /// </summary>
    public MomentLink? Next { get; init; }
}

/// <summary>上一篇 / 下一篇。</summary>
public record MomentLink(string Title, string Slug) {
    /// <summary>
    /// 获取 Url
    /// </summary>
    public string Url => "/moments/" + Slug;
}
