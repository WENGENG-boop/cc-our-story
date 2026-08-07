// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

namespace OurStory.Services.Moments;

/// <summary>
/// 定义 Markdown 内容渲染服务契约。提供 Markdown 文本到 HTML 内容的转换能力。
/// </summary>
public interface IMarkdownRenderer {
    /// <summary>
    /// 将 Markdown 文本异步转换为 HTML 字符串。空文本或 null 内容将返回空字符串
    /// </summary>
    /// <param name="markdown">待转换的 Markdown 文本</param>
    /// <returns>转换后的 HTML 内容</returns>
    string ToHtml(string? markdown);
}
