// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System.Text;
using System.Text.RegularExpressions;

namespace OurStory.Core.Text;

/// <summary>
/// URL 地址中的短名称生成器
/// </summary>
/// <remarks>
/// 用于生成文章、页面等内容的友好访问标识（slug）。slug 仅包含小写英文字母、数字和连字符，适合作为 URL 路径的一部分
/// </remarks>
public static partial class SlugFactory {
    private const int MaxLength = 60;   // slug 最大长度限制

    /// <summary>
    /// 根据标题生成 slug
    /// 
    /// 生成规则：
    /// - 标题中存在可转换的英文或数字内容时，尝试生成规范 slug；
    /// - 中文标题不会强制转换拼音，避免引入拼音库依赖以及产生不准确结果；
    /// - 无法生成有效 slug 时，使用日期 + 随机后缀作为唯一标识。
    /// 
    /// 中文标题如果需要自定义漂亮地址，可以由用户在后台手动填写
    /// </summary>
    /// <param name="title">内容标题</param>
    /// <param name="momentDate">内容创建时间，用于生成默认 slug</param>
    /// <returns>
    /// 返回可用于 URL 的 slug。
    /// 
    /// 示例：
    /// - Hello World → hello-world
    /// - 2026 年旅行 → 2026
    /// - 你好世界 → 20260807-a1b2c3
    /// </returns>
    public static string FromTitle(string? title, DateTime momentDate) {
        var slug = Normalize(title);
        return slug.Length > 0
            ? slug
            : $"{momentDate:yyyyMMdd}-{RandomSuffix()}";
    }

    /// <summary>
    /// 清理并规范化用户输入的 slug
    /// 
    /// 处理规则：
    /// - 转换为小写；
    /// - 保留英文字符和数字；
    /// - 空格、下划线、点号统一转换为连字符；
    /// - 合并连续连字符；
    /// - 移除首尾连字符；
    /// - 限制最大长度。
    /// </summary>
    /// <param name="value">用户输入的原始 slug</param>
    /// <returns>
    /// 返回规范化后的 slug。
    /// 
    /// - 无有效字符时返回空字符串；
    /// - 有效输入返回仅包含小写字母、数字和连字符的字符串。
    /// </returns>
    public static string Normalize(string? value) {
        if (string.IsNullOrWhiteSpace(value)) {
            return string.Empty;
        }

        var builder = new StringBuilder(value.Length);
        foreach (var ch in value.Trim().ToLowerInvariant()) {
            if (char.IsAsciiLetterOrDigit(ch)) {
                _ = builder.Append(ch);
            } else if (ch is '-' or '_' or ' ' or '.') {
                _ = builder.Append('-');
            }
        }

        var slug = DashRuns().Replace(builder.ToString(), "-").Trim('-');
        return slug.Length > MaxLength ? slug[..MaxLength].Trim('-') : slug;
    }

    private static string RandomSuffix() => Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(3)).ToLowerInvariant();

    [GeneratedRegex("-{2,}")]
    private static partial Regex DashRuns();
}
