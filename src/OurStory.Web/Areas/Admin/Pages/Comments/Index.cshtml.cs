// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OurStory.Core.Entities;
using OurStory.Core.Models;
using OurStory.Core.Time;
using OurStory.Services.Comments;

namespace OurStory.Web.Areas.Admin.Pages.Comments;

/// <summary>
/// 表示 IndexModel
/// </summary>
public class IndexModel(ICommentService comments, SiteClock clock) : PageModel {
    private const int PageSize = 25;

    /// <summary>
    /// 获取或设置 PageNumber
    /// </summary>
    [BindProperty(SupportsGet = true, Name = "page")]
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// 执行 Items 操作
    /// </summary>
    public PagedList<Comment> Items { get; private set; } = PagedList<Comment>.Empty(PageSize);

    /// <summary>
    /// 处理 GET 请求
    /// </summary>
    public async Task OnGetAsync(CancellationToken cancellationToken) {
        Items = await comments.ListForAdminAsync(PageNumber, PageSize, cancellationToken);
    }

    /// <summary>
    /// 处理 ToggleAsync(int, bool, CancellationToken) 的 POST 请求
    /// </summary>
    public async Task<IActionResult> OnPostToggleAsync(int id, bool approved, CancellationToken cancellationToken) {
        _ = await comments.SetApprovedAsync(id, approved, cancellationToken);
        TempData["Flash"] = approved ? "这条留言已经放出来了。" : "这条留言先压下去了。";
        return RedirectToPage(new { page = PageNumber });
    }

    /// <summary>
    /// 处理 DeleteAsync(int, CancellationToken) 的 POST 请求
    /// </summary>
    public async Task<IActionResult> OnPostDeleteAsync(int id, CancellationToken cancellationToken) {
        _ = await comments.DeleteAsync(id, cancellationToken);
        TempData["Flash"] = "留言已经删掉了。";
        return RedirectToPage(new { page = PageNumber });
    }

    /// <summary>
    /// 转换Local
    /// </summary>
    public DateTime ToLocal(DateTimeOffset instant) => clock.ToLocal(instant);
}
