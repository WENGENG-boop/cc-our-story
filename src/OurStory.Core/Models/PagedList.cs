// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

namespace OurStory.Core.Models;

/// <summary>一页数据，外加翻页需要的几个数字。</summary>
/// <remarks>
/// 使用指定的数据、页码、每页数量和总记录数初始化分页列表
/// </remarks>
public class PagedList<T>(IReadOnlyList<T> items, int page, int pageSize, int totalCount) {

    /// <summary>
    /// 获取或设置 Items
    /// </summary>
    public IReadOnlyList<T> Items { get; } = items;

    /// <summary>
    /// 获取或设置 Page
    /// </summary>
    public int Page { get; } = page < 1 ? 1 : page;

    /// <summary>
    /// 获取或设置 PageSize
    /// </summary>
    public int PageSize { get; } = pageSize < 1 ? 1 : pageSize;

    /// <summary>
    /// 获取或设置 TotalCount
    /// </summary>
    public int TotalCount { get; } = totalCount;

    /// <summary>
    /// 执行 TotalPages 操作
    /// </summary>
    public int TotalPages => TotalCount == 0 ? 1 : (TotalCount + PageSize - 1) / PageSize;

    /// <summary>
    /// 获取 HasPrevious
    /// </summary>
    public bool HasPrevious => Page > 1;

    /// <summary>
    /// 获取 HasNext
    /// </summary>
    public bool HasNext => Page < TotalPages;

    /// <summary>
    /// 执行 Empty 操作
    /// </summary>
    public static PagedList<T> Empty(int pageSize) => new([], 1, pageSize, 0);
}
