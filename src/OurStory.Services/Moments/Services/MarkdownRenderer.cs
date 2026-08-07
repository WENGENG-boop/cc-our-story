// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Markdig;

namespace OurStory.Services.Moments;

internal class MarkdownRenderer : IMarkdownRenderer {
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .UseSoftlineBreakAsHardlineBreak()
        .Build();

    public string ToHtml(string? markdown) => string.IsNullOrWhiteSpace(markdown) ? string.Empty : Markdown.ToHtml(markdown, Pipeline);
}
