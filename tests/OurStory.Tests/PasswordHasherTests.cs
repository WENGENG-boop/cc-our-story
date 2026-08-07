// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.
using OurStory.Services.Accounts;
using Xunit;

namespace OurStory.Tests;

/// <summary>
/// 表示 PasswordHasherTests
/// </summary>
public class PasswordHasherTests {
    /// <summary>
    /// 验证同一个口令能校验通过()
    /// </summary>
    [Fact]
    public void 同一个口令能校验通过() {
        var hash = PasswordHasher.Hash("我们的小宇宙 2026");

        Assert.True(PasswordHasher.Verify("我们的小宇宙 2026", hash));
    }

    /// <summary>
    /// 验证换个口令就通不过()
    /// </summary>
    [Fact]
    public void 换个口令就通不过() {
        var hash = PasswordHasher.Hash("我们的小宇宙 2026");

        Assert.False(PasswordHasher.Verify("我们的小宇宙 2027", hash));
    }

    /// <summary>
    /// 验证每次散列都带不同的盐()
    /// </summary>
    [Fact]
    public void 每次散列都带不同的盐() {
        Assert.NotEqual(PasswordHasher.Hash("同一个口令"), PasswordHasher.Hash("同一个口令"));
    }

    /// <summary>
    /// 验证存储串坏掉时只判不匹配不抛异常(string)
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("乱七八糟")]
    [InlineData("pbkdf2-sha256$不是数字$aaaa$bbbb")]
    [InlineData("pbkdf2-sha256$1000$@@@@$####")]
    public void 存储串坏掉时只判不匹配不抛异常(string hash) {
        Assert.False(PasswordHasher.Verify("随便什么", hash));
    }

    /// <summary>
    /// 验证生成的初始口令长度符合预期()
    /// </summary>
    [Fact]
    public void 生成的初始口令长度符合预期() {
        Assert.Equal(14, PasswordHasher.GenerateReadablePassword().Length);
    }
}
