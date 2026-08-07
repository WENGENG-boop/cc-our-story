// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace OurStory.Web.Pages;

/// <summary>退出登录。页面上的「退出」是普通链接，所以 GET 和 POST 都收。</summary>
public class LogoutModel : PageModel {
    /// <summary>
    /// 处理 GET 请求
    /// </summary>
    public Task<IActionResult> OnGetAsync() => SignOutAsync();

    /// <summary>
    /// 处理 Async() 的 POST 请求
    /// </summary>
    public Task<IActionResult> OnPostAsync() => SignOutAsync();

    private async Task<IActionResult> SignOutAsync() {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Redirect("/");
    }
}
