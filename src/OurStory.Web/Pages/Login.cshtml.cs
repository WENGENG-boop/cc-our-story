// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OurStory.Services.Accounts;
using OurStory.Services.Settings;
using OurStory.Web.Infrastructure;
using System.Globalization;
using System.Security.Claims;

namespace OurStory.Web.Pages;

/// <summary>
/// 表示 LoginModel
/// </summary>
public class LoginModel(IUserService users, ISettingsService settingsService) : PageModel {
    /// <summary>
    /// 获取或设置 UserName
    /// </summary>
    [BindProperty]
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置 Password
    /// </summary>
    [BindProperty]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置 RememberMe
    /// </summary>
    [BindProperty]
    public bool RememberMe { get; set; } = true;

    /// <summary>
    /// 获取或设置 Error
    /// </summary>
    public string? Error { get; private set; }

    /// <summary>
    /// 获取或设置 SiteTitle
    /// </summary>
    public string SiteTitle { get; private set; } = "CC Our Story";

    /// <summary>
    /// 处理 GET 请求
    /// </summary>
    public async Task<IActionResult> OnGetAsync(string? returnUrl, CancellationToken cancellationToken) {
        SiteTitle = (await settingsService.GetAsync(cancellationToken)).SiteTitle;

        return User.IsOwner()
            ? Redirect(SafeReturnUrl(returnUrl))
            : Page();
    }

    /// <summary>
    /// 处理 Async(string?, CancellationToken) 的 POST 请求
    /// </summary>
    public async Task<IActionResult> OnPostAsync(string? returnUrl, CancellationToken cancellationToken) {
        SiteTitle = (await settingsService.GetAsync(cancellationToken)).SiteTitle;

        var user = await users.AuthenticateAsync(UserName, Password, cancellationToken);
        if (user is null) {
            // 不区分「没这个人」和「口令不对」，避免把账号是否存在漏出去
            Error = "登录名或口令不对。";
            return Page();
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString(CultureInfo.InvariantCulture)),
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties {
                IsPersistent = RememberMe,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
            });

        return Redirect(SafeReturnUrl(returnUrl));
    }

    /// <summary>只认站内地址，挡掉把人送到外站的开放重定向。</summary>
    private string SafeReturnUrl(string? returnUrl) =>
        !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl) ? returnUrl : "/admin";
}
