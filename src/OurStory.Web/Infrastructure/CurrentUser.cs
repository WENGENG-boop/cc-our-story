// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using OurStory.Core;
using System.Globalization;
using System.Security.Claims;

namespace OurStory.Web.Infrastructure;

/// <summary>
/// 从登录票据里读身份。
///
/// 票据里只有「是谁、是什么身份」，页面上叫什么一律现查站点设置，
/// 这样在后台改了男主 / 女主名字，不用重新登录就能生效。
/// </summary>
public static class CurrentUser {
    /// <summary>
    /// 判断Owner
    /// </summary>
    public static bool IsOwner(this ClaimsPrincipal? principal) =>
        principal?.Identity?.IsAuthenticated == true;

    /// <summary>
    /// 获取用户标识
    /// </summary>
    public static int? UserId(this ClaimsPrincipal? principal) {
        var value = principal?.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(value, CultureInfo.InvariantCulture, out var id) ? id : null;
    }

    /// <summary>没登录的一律是访客。</summary>
    public static UserRole Role(this ClaimsPrincipal? principal) {
        var value = principal?.FindFirstValue(ClaimTypes.Role);
        return Enum.TryParse<UserRole>(value, out var role) ? role : UserRole.Guest;
    }
}
