// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System.Net;

namespace OurStory.Services.Notifications;

/// <summary>
/// VAPID 的 <c>sub</c>：推送网关眼里「这个站点出问题时联系谁」
/// </summary>
/// <remarks>
/// 规范只说必须是 <c>mailto:</c> 或 <c>https://</c>，但 Apple 的网关会真的去看里面的域名：
/// <c>.local</c>、<c>localhost</c>、裸 IP 这种联系不到人的写法，它一律回
/// <c>403 BadJwtToken</c>，整台 iPhone 就此收不到任何通知。Chrome 的 FCM 宽松得多，
/// 所以这类问题在本地开发时完全看不出来，一上线才炸。
///
/// 这里的活就是：挡住网关不认的写法，并且永远兜得住一个一定过得去的值
/// </remarks>
internal static class VapidSubject {
    /// <summary>
    /// 实在推不出站点域名时用的兜底联系方式
    /// </summary>
    /// <remarks>
    /// 必须是个真实存在的公网地址，不能是 <c>example.local</c> 这类占位符 ——
    /// 兜底值的唯一职责就是「无论如何都能让通知发出去」
    /// </remarks>
    public const string Fallback = "https://github.com/Keeleycenc/cc-our-story";

    /// <summary>
    /// 保留给内网和测试的顶级域，写进 sub 里网关联系不到人
    /// </summary>
    private static readonly string[] PrivateTopLevelDomains = [
        "local", "localhost", "internal", "intranet", "lan", "home", "corp", "test", "invalid", "example"
    ];

    /// <summary>
    /// 取一个网关一定认的 sub：配置里那个能用就用它，否则兜底
    /// </summary>
    public static string Normalize(string? configured) =>
        IsUsable(configured) ? configured!.Trim() : Fallback;

    /// <summary>
    /// 判断一个 sub 推送网关认不认
    /// </summary>
    public static bool IsUsable(string? value) {
        var text = (value ?? string.Empty).Trim();

        if (text.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase)) {
            var at = text.LastIndexOf('@');
            return at > "mailto:".Length && IsPublicHost(text[(at + 1)..]);
        }

        return Uri.TryCreate(text, UriKind.Absolute, out var uri)
            && uri.Scheme == Uri.UriSchemeHttps
            && IsPublicHost(uri.Host);
    }

    /// <summary>
    /// 从站点自己的地址推出一个 sub；推不出合格的值时返回 <see langword="null"/>
    /// </summary>
    /// <param name="siteOrigin">当次请求看到的站点地址</param>
    /// <remarks>
    /// 一律按 https 写：sub 里的协议跟站点实际用什么协议无关，而反代很可能只递上 http，
    /// 照抄就会白白丢掉一个本来合格的域名
    /// </remarks>
    public static string? FromOrigin(string? siteOrigin) =>
        Uri.TryCreate(siteOrigin, UriKind.Absolute, out var origin) && IsPublicHost(origin.Host)
            ? $"https://{origin.Host}"
            : null;

    /// <summary>
    /// 这个主机名在公网上联系得到人吗
    /// </summary>
    private static bool IsPublicHost(string host) {
        // 裸 IP 联系不到人，Apple 直接拒
        if (host.Length == 0 || IPAddress.TryParse(host, out _)) {
            return false;
        }

        var dot = host.LastIndexOf('.');

        // 没有点的（localhost、内网机器名）和以点结尾的都不算
        return dot > 0
            && dot < host.Length - 1
            && !PrivateTopLevelDomains.Contains(host[(dot + 1)..], StringComparer.OrdinalIgnoreCase);
    }
}
