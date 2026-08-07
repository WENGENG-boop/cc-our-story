// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.
using OurStory.Web.Infrastructure;
using Xunit;

namespace OurStory.Tests;

/// <summary>
/// 表示 MaintenanceCommandTests
/// </summary>
public class MaintenanceCommandTests {
    /// <summary>
    /// 验证没有维护开关时返回空并原样启动站点()
    /// </summary>
    [Fact]
    public void 没有维护开关时返回空并原样启动站点() {
        Assert.Null(MaintenanceCommand.Parse(["--urls", "http://localhost:5080"]));
    }

    /// <summary>
    /// 验证能认出重置口令的账号名()
    /// </summary>
    [Fact]
    public void 能认出重置口令的账号名() {
        var command = MaintenanceCommand.Parse(["--reset-password", "boy"]);

        Assert.NotNull(command);
        Assert.Equal(MaintenanceAction.ResetPassword, command.Action);
        Assert.Equal("boy", command.UserName);
        Assert.Null(command.NewPassword);
    }

    /// <summary>
    /// 验证能同时认出指定的新口令()
    /// </summary>
    [Fact]
    public void 能同时认出指定的新口令() {
        var command = MaintenanceCommand.Parse(["--reset-password", "girl", "--new-password", "Recover#2026"]);

        Assert.NotNull(command);
        Assert.Equal("girl", command.UserName);
        Assert.Equal("Recover#2026", command.NewPassword);
    }

    /// <summary>
    /// 验证开关后面没跟账号名时不当作维护命令()
    /// </summary>
    [Fact]
    public void 开关后面没跟账号名时不当作维护命令() {
        // 末尾单独一个开关，按普通启动处理，交给 Host 去报参数错
        Assert.Null(MaintenanceCommand.Parse(["--reset-password"]));
    }

    /// <summary>
    /// 验证开关后面跟着另一个开关时也不算()
    /// </summary>
    [Fact]
    public void 开关后面跟着另一个开关时也不算() {
        Assert.Null(MaintenanceCommand.Parse(["--reset-password", "--urls", "http://localhost:5080"]));
    }

    /// <summary>
    /// 验证帮助开关会被认出来(string)
    /// </summary>
    [Theory]
    [InlineData("--help")]
    [InlineData("-h")]
    public void 帮助开关会被认出来(string flag) {
        Assert.Equal(MaintenanceAction.Help, MaintenanceCommand.Parse([flag])!.Action);
    }

    /// <summary>
    /// 验证列账号的开关会被认出来()
    /// </summary>
    [Fact]
    public void 列账号的开关会被认出来() {
        Assert.Equal(MaintenanceAction.ListAccounts, MaintenanceCommand.Parse(["--list-accounts"])!.Action);
    }

    /// <summary>
    /// 验证自己的开关连同它的值都会被摘掉()
    /// </summary>
    [Fact]
    public void 自己的开关连同它的值都会被摘掉() {
        // 留给 Host 的参数里不能再有这些，否则命令行配置提供程序会把它们当配置项
        var kept = MaintenanceCommand.StripFrom(
            ["--reset-password", "boy", "--new-password", "Recover#2026", "--urls", "http://localhost:5080"]);

        Assert.Equal(["--urls", "http://localhost:5080"], kept);
    }

    /// <summary>
    /// 验证末尾单独一个开关也能摘干净()
    /// </summary>
    [Fact]
    public void 末尾单独一个开关也能摘干净() {
        Assert.Equal(["--urls", "http://x"], MaintenanceCommand.StripFrom(["--urls", "http://x", "--reset-password"]));
    }

    /// <summary>
    /// 验证没有维护开关时参数原样透传()
    /// </summary>
    [Fact]
    public void 没有维护开关时参数原样透传() {
        string[] args = ["--urls", "http://localhost:5080", "--environment", "Production"];

        Assert.Equal(args, MaintenanceCommand.StripFrom(args));
    }
}
