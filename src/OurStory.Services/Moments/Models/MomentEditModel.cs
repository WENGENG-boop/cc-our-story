// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using OurStory.Core;

namespace OurStory.Services.Moments;

/// <summary>
/// 后台编辑表单提交上来的内容
/// </summary>
public class MomentEditModel {
    /// <summary>
    /// 获取或设置 Title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置 Slug
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// 获取或设置 Content
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置 Summary
    /// </summary>
    public string? Summary { get; set; }

    /// <summary>
    /// 获取或设置 CoverUrl
    /// </summary>
    public string? CoverUrl { get; set; }

    /// <summary>
    /// 获取或设置 Mood
    /// </summary>
    public string? Mood { get; set; }

    /// <summary>
    /// 获取或设置 Location
    /// </summary>
    public string? Location { get; set; }

    /// <summary>站点时区下的日期时间。</summary>
    public DateTime MomentDate { get; set; } = DateTime.Now;

    /// <summary>
    /// 获取或设置 Password
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 获取或设置 Status
    /// </summary>
    public MomentStatus Status { get; set; } = MomentStatus.Published;

    /// <summary>
    /// 获取或设置 AllowComment
    /// </summary>
    public bool AllowComment { get; set; } = true;
}
