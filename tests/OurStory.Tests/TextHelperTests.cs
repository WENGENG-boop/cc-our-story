// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.
using OurStory.Core.Text;
using Xunit;

namespace OurStory.Tests;

/// <summary>
/// 表示 SlugFactoryTests
/// </summary>
public class SlugFactoryTests {
    /// <summary>
    /// 验证英文标题转成短横线连接的小写()
    /// </summary>
    [Fact]
    public void 英文标题转成短横线连接的小写() {
        Assert.Equal("our-little-world", SlugFactory.Normalize("Our Little World"));
    }

    /// <summary>
    /// 验证连续的分隔符会被压成一个()
    /// </summary>
    [Fact]
    public void 连续的分隔符会被压成一个() {
        Assert.Equal("a-b", SlugFactory.Normalize("a --__  b"));
    }

    /// <summary>
    /// 验证纯中文标题退回日期加随机后缀()
    /// </summary>
    [Fact]
    public void 纯中文标题退回日期加随机后缀() {
        var slug = SlugFactory.FromTitle("今天去看海了", new DateTime(2026, 8, 7));

        Assert.StartsWith("20260807-", slug, StringComparison.Ordinal);
        Assert.Equal("20260807-".Length + 6, slug.Length);
    }

    /// <summary>
    /// 验证中英混排只保留能用的部分()
    /// </summary>
    [Fact]
    public void 中英混排只保留能用的部分() {
        Assert.Equal("beach-day", SlugFactory.Normalize("海边 beach day"));
    }
}

/// <summary>
/// 表示 HtmlTextTests
/// </summary>
public class HtmlTextTests {
    /// <summary>
    /// 验证摘要会去掉标签并压缩空白()
    /// </summary>
    [Fact]
    public void 摘要会去掉标签并压缩空白() {
        var html = "<p>今天   去看海</p>\n<p>浪很大</p>";

        Assert.Equal("今天 去看海 浪很大", HtmlText.Excerpt(html, 100));
    }

    /// <summary>
    /// 验证超长时截断并补省略号()
    /// </summary>
    [Fact]
    public void 超长时截断并补省略号() {
        Assert.Equal("今天去…", HtmlText.Excerpt("<p>今天去看海了</p>", 3));
    }

    /// <summary>
    /// 验证能取出正文里的第一张图()
    /// </summary>
    [Fact]
    public void 能取出正文里的第一张图() {
        var html = """<p>a</p><img src="/uploads/a.png"><img src="/uploads/b.png">""";

        Assert.Equal("/uploads/a.png", HtmlText.FirstImage(html));
    }

    /// <summary>
    /// 验证没有图片时返回空串()
    /// </summary>
    [Fact]
    public void 没有图片时返回空串() {
        Assert.Equal(string.Empty, HtmlText.FirstImage("<p>只有文字</p>"));
    }
}
