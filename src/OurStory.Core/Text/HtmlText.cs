// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System.Net;
using System.Text.RegularExpressions;

namespace OurStory.Core.Text;

/// <summary>
/// 从渲染好的正文里取摘要和封面
/// </summary>
public static partial class HtmlText {
    /// <summary>
    /// 摘要：去掉标签、把连续空白压成一个空格，再按字符数截断
    /// </summary>
    /// <remarks>
    /// 中英文混排时按「字符数」比按「字节数」直观，这里就按字符数算
    /// </remarks>
    /// <param name="html">渲染好的正文</param>
    /// <param name="length">摘要长度</param>
    /// <param name="ellipsis">省略号</param>
    /// <returns>
    /// 纯文本摘要
    /// 
    /// - html 为空时返回空字符串；
    /// - 正文长度未超过限制时返回完整文本；
    /// - 超出限制时返回截断后的文本并追加省略符号
    /// </returns>
    public static string Excerpt(string? html, int length, string ellipsis = "…") {
        if (string.IsNullOrWhiteSpace(html)) {
            return string.Empty;
        }

        var text = WebUtility.HtmlDecode(Tags().Replace(html, " "));
        text = Whitespace().Replace(text, " ").Trim();

        return text.Length <= length ? text : text[..length].TrimEnd() + ellipsis;
    }

    /// <summary>
    /// 正文里的第一张图，没有就返回空串
    /// </summary>
    /// <param name="html">渲染好的正文</param>
    /// <returns>
    /// 返回正文中第一张图片的 URL
    /// 
    /// - 未找到图片时返回空字符串；
    /// - html 为空时返回空字符串；
    /// - 返回值已经进行 HTML 实体解码
    /// </returns>
    public static string FirstImage(string? html) {
        if (string.IsNullOrWhiteSpace(html)) {
            return string.Empty;
        }

        var match = ImageSource().Match(html);
        return match.Success ? WebUtility.HtmlDecode(match.Groups[1].Value) : string.Empty;
    }

    [GeneratedRegex("<[^>]+>")]
    private static partial Regex Tags();

    [GeneratedRegex(@"\s+")]
    private static partial Regex Whitespace();

    [GeneratedRegex("""<img[^>]+src=["']([^"']+)["']""", RegexOptions.IgnoreCase)]
    private static partial Regex ImageSource();
}
