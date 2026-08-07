// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OurStory.Core.Entities;
using OurStory.Services.Accounts;
using OurStory.Web.Infrastructure;
using System.ComponentModel.DataAnnotations;

namespace OurStory.Web.Areas.Admin.Pages;

/// <summary>
/// 表示 AccountModel
/// </summary>
public class AccountModel(IUserService users) : PageModel {
    /// <summary>
    /// 执行 Password 操作
    /// </summary>
    [BindProperty]
    public PasswordInput Password { get; set; } = new();

    /// <summary>
    /// 获取或设置 Accounts
    /// </summary>
    public IReadOnlyList<User> Accounts { get; private set; } = [];

    /// <summary>
    /// 获取或设置 Error
    /// </summary>
    public string? Error { get; private set; }

    /// <summary>
    /// 处理 GET 请求
    /// </summary>
    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken) =>
        await LoadAsync(cancellationToken) is null ? NotFound() : Page();

    /// <summary>
    /// 处理 PasswordAsync(CancellationToken) 的 POST 请求
    /// </summary>
    public async Task<IActionResult> OnPostPasswordAsync(CancellationToken cancellationToken) {
        var me = await LoadAsync(cancellationToken);
        if (me is null) {
            return NotFound();
        }

        if (!ModelState.IsValid) {
            Error = string.Join("；", ModelState.Values.SelectMany(state => state.Errors).Select(error => error.ErrorMessage));
            return Page();
        }

        if (Password.NewPassword != Password.ConfirmPassword) {
            Error = "两次输入的新口令不一样。";
            return Page();
        }

        if (!await users.ChangePasswordAsync(me.Id, Password.CurrentPassword, Password.NewPassword, cancellationToken)) {
            Error = "当前口令不对。";
            return Page();
        }

        TempData["Flash"] = "口令已经改好了。";
        return RedirectToPage();
    }

    private async Task<User?> LoadAsync(CancellationToken cancellationToken) {
        Accounts = await users.ListAsync(cancellationToken);

        var id = User.UserId();
        return id is null ? null : Accounts.FirstOrDefault(user => user.Id == id.Value);
    }

    /// <summary>
    /// 表示 PasswordInput
    /// </summary>
    public class PasswordInput {
        /// <summary>
        /// 获取或设置 CurrentPassword
        /// </summary>
        [Required(ErrorMessage = "请输入当前口令")]
        public string CurrentPassword { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置 NewPassword
        /// </summary>
        [Required(ErrorMessage = "请输入新口令")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "新口令至少 8 位")]
        public string NewPassword { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置 ConfirmPassword
        /// </summary>
        [Required(ErrorMessage = "请再输入一次新口令")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
