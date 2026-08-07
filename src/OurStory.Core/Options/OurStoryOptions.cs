// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

namespace OurStory.Core.Options;

/// <summary>
/// appsettings.json 里 "OurStory" 节点，站点级别的运行参数
/// </summary>
public class OurStoryOptions {
    /// <summary>
    /// 表示 SectionName
    /// </summary>
    public const string SectionName = "OurStory";

    /// <summary>
    /// 获取或设置数据目录：SQLite 数据库、上传的附件和数据保护密钥都放这里       
    /// </summary>
    /// <remarks>
    /// 相对路径按站点根目录解析。部署时只要把这一个目录挂出来 / 备份下来，整个站点的数据就都在了
    /// </remarks>
    public string DataDirectory { get; set; } = "App_Data";

    /// <summary>
    /// 获取或设置 SQLite 文件名，位于 <see cref="DataDirectory"/> 下
    /// </summary>
    public string DatabaseFileName { get; set; } = "ourstory.db";

    /// <summary>
    /// 获取或设置站点时区。「今天」「相恋第 N 天」「每日 99 次上限」都按它算，不跟着服务器所在时区跑
    /// </summary>
    public string TimeZone { get; set; } = "Asia/Shanghai";

    /// <summary>
    /// 获取或设置生成访客指纹时掺进去的密钥。留空会在首次启动时随机生成并写进数据库，这样同一份部署重启后访客的统计不会断
    /// </summary>
    public string VisitorSecret { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置首次启动时自动创建的两个账号
    /// </summary>
    public SeedAccountOptions Seed { get; set; } = new();
}

/// <summary>
/// 空库时创建男主 / 女主账号用的初始信息
/// </summary>
/// <remarks>
/// 密码留空时随机生成一串并打到启动日志里，绝不会出现「默认密码」这种东西
/// </remarks>
public class SeedAccountOptions {
    /// <summary>
    /// 获取或设置 BoyUserName
    /// </summary>
    public string BoyUserName { get; set; } = "boy";

    /// <summary>
    /// 获取或设置 BoyPassword
    /// </summary>
    public string BoyPassword { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置 GirlUserName
    /// </summary>
    public string GirlUserName { get; set; } = "girl";

    /// <summary>
    /// 获取或设置 GirlPassword
    /// </summary>
    public string GirlPassword { get; set; } = string.Empty;
}
