// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OurStory.Core.Abstractions;
using OurStory.Core.Options;
using OurStory.Core.Time;
using OurStory.Data;
using OurStory.Services.Accounts;
using OurStory.Services.Comments;
using OurStory.Services.Heartbeats;
using OurStory.Services.Moments;
using OurStory.Services.Settings;
using OurStory.Services.Storage;

namespace OurStory.Services;

/// <summary>
/// 表示 ServiceCollectionExtensions
/// </summary>
public static class ServiceCollectionExtensions {
    /// <summary>
    /// 把数据层和业务层一次性接进容器。
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="configuration">应用配置</param>
    /// <param name="contentRootPath">站点根目录，用来把配置里的相对数据目录解析成绝对路径。</param>
    public static IServiceCollection AddOurStory(this IServiceCollection services, IConfiguration configuration, string contentRootPath) {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        _ = services.AddOptions<OurStoryOptions>()
            .Bind(configuration.GetSection(OurStoryOptions.SectionName))
            .ValidateOnStart();

        _ = services.AddOptions<StorageOptions>()
            .Bind(configuration.GetSection(StorageOptions.SectionName))
            .ValidateOnStart();

        var options = configuration.GetSection(OurStoryOptions.SectionName).Get<OurStoryOptions>() ?? new OurStoryOptions();
        var dataDirectory = ResolveDataDirectory(contentRootPath, options.DataDirectory);
        var uploadsRoot = Path.Combine(dataDirectory, "uploads");

        _ = Directory.CreateDirectory(dataDirectory);
        _ = Directory.CreateDirectory(uploadsRoot);

        _ = services.AddSingleton(new StoragePaths(uploadsRoot, "/uploads"));
        _ = services.AddSingleton(provider => new SiteClock(provider.GetRequiredService<IOptions<OurStoryOptions>>().Value));

        var databasePath = Path.Combine(dataDirectory, options.DatabaseFileName);
        _ = services.AddDbContext<OurStoryDbContext>(builder => builder.UseSqlite($"Data Source={databasePath}"));

        _ = services.AddMemoryCache();
        _ = services.AddHttpClient(AliyunOssFileStorage.HttpClientName, client => {
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // 存储驱动在启动时定下来：选了 OSS 但参数没配全的，静默退回本地，
        // 免得后台一上传就报错
        _ = services.AddSingleton<IFileStorage>(provider => {
            var storage = provider.GetRequiredService<IOptions<StorageOptions>>().Value;
            return storage.Driver == StorageDriver.AliyunOss && storage.Oss.IsConfigured
                ? ActivatorUtilities.CreateInstance<AliyunOssFileStorage>(provider)
                : ActivatorUtilities.CreateInstance<LocalFileStorage>(provider);
        });

        _ = services.AddScoped<ISettingsService, SettingsService>();
        _ = services.AddScoped<IUserService, UserService>();
        _ = services.AddScoped<IMomentService, MomentService>();
        _ = services.AddScoped<ICommentService, CommentService>();
        _ = services.AddScoped<IHeartbeatService, HeartbeatService>();
        _ = services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
        _ = services.AddSingleton<IMarkdownRenderer, MarkdownRenderer>();
        _ = services.AddScoped<IAttachmentService, AttachmentService>();

        return services;
    }

    /// <summary>数据目录支持写相对路径（相对站点根目录），也支持写绝对路径。</summary>
    public static string ResolveDataDirectory(string contentRootPath, string configured) {
        var value = string.IsNullOrWhiteSpace(configured) ? "App_Data" : configured;
        return Path.IsPathRooted(value) ? value : Path.GetFullPath(Path.Combine(contentRootPath, value));
    }
}
