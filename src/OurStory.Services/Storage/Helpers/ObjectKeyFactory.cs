// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace OurStory.Services.Storage;

/// <summary>
/// 附件对象键生成工具
/// 本地存储和对象存储使用统一路径规则
/// </summary>
/// <remarks>
/// 默认格式为：前缀/年/月/随机文件名.扩展名
/// 保持目录结构一致，方便后续切换不同存储驱动
/// </remarks>
public static partial class ObjectKeyFactory {
    /// <summary>
    /// 创建附件对象键
    /// 根据指定前缀、扩展名以及时间生成唯一对象路径
    /// </summary>
    /// <param name="prefix">对象路径前缀</param>
    /// <param name="extension">文件扩展名</param>
    /// <param name="now">用于生成日期目录的时间</param>
    /// <returns>生成后的对象键</returns>
    public static string Create(string prefix, string extension, DateTime now) {
        var cleanExtension = NonAlphanumeric().Replace(extension, string.Empty).ToLowerInvariant();
        var name = Convert.ToHexString(RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();

        return $"{NormalizePrefix(prefix)}/{now:yyyy}/{now:MM}/{name}.{cleanExtension}";
    }

    /// <summary>
    /// 标准化对象路径前缀
    /// 清理非法字符、重复斜杠以及首尾分隔符
    /// </summary>
    /// <param name="prefix">原始路径前缀</param>
    /// <returns>安全规范化后的路径前缀</returns>
    public static string NormalizePrefix(string? prefix) {
        var cleaned = IllegalPrefixChars().Replace((prefix ?? string.Empty).Replace('\\', '/'), string.Empty);
        cleaned = SlashRuns().Replace(cleaned, "/").Trim('/');

        return cleaned.Length > 0 ? cleaned : "ourstory/public";
    }

    /// <summary>
    /// 判断对象键是否安全有效
    /// 检查路径片段是否包含非法字符或目录穿越风险
    /// </summary>
    /// <param name="objectKey">待检查的对象键</param>
    /// <returns>安全返回 true，否则返回 false</returns>
    public static bool IsSafe(string objectKey) {
        if (string.IsNullOrWhiteSpace(objectKey)) {
            return false;
        }

        foreach (var segment in objectKey.Replace('\\', '/').Trim('/').Split('/')) {
            if (segment is "." or ".." || !SafeSegment().IsMatch(segment)) {
                return false;
            }
        }

        return true;
    }

    [GeneratedRegex("[^A-Za-z0-9]")]
    private static partial Regex NonAlphanumeric();

    [GeneratedRegex("[^A-Za-z0-9._/-]")]
    private static partial Regex IllegalPrefixChars();

    [GeneratedRegex("/{2,}")]
    private static partial Regex SlashRuns();

    [GeneratedRegex("^[A-Za-z0-9._-]+$")]
    private static partial Regex SafeSegment();
}
