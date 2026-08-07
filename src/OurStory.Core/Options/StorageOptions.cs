// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

namespace OurStory.Core.Options;

/// <summary>
/// 附件存储方式
/// </summary>
public enum StorageDriver {
    /// <summary>
    /// 本地
    /// </summary>
    Local = 0,

    /// <summary>
    /// 阿里云 oss
    /// </summary>
    AliyunOss = 1
}

/// <summary>
/// appsettings.json 里 "Storage" 节点
/// </summary>
/// <remarks>
/// 本地和 OSS 用同一套 object key（<c>前缀/年/月/随机名.后缀</c>），
/// 所以把本地目录整个传到 Bucket 再切换 Driver，已有图片的地址会自动跟着变
/// </remarks>
public class StorageOptions {
    /// <summary>
    /// 表示 SectionName
    /// </summary>
    public const string SectionName = "Storage";

    /// <summary>
    /// 获取或设置 Driver
    /// </summary>
    public StorageDriver Driver { get; set; } = StorageDriver.Local;

    /// <summary>
    /// 获取或设置 object key 的前缀，只保留字母、数字、下划线、连字符和斜线
    /// </summary>
    public string Prefix { get; set; } = "ourstory/public";

    /// <summary>
    /// 获取或设置单个附件的大小上限（字节）
    /// </summary>
    public long MaxFileSize { get; set; } = 20 * 1024 * 1024;

    /// <summary>
    /// 获取或设置允许上传的扩展名，小写、不带点
    /// </summary>
    public string[] AllowedExtensions { get; set; } = ["jpg", "jpeg", "png", "gif", "webp", "avif"];

    /// <summary>
    /// 获取或设置执行 Oss 操作
    /// </summary>
    public OssOptions Oss { get; set; } = new();
}

/// <summary>
/// 阿里云 OSS 参数。字段名和原来插件用的环境变量一一对应（OSS_REGION、OSS_BUCKET……）
/// </summary>
public class OssOptions {
    /// <summary>
    /// 获取或设置 Region
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置 Bucket
    /// </summary>
    public string Bucket { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置 AccessKeyId
    /// </summary>
    public string AccessKeyId { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置 AccessKeySecret
    /// </summary>
    public string AccessKeySecret { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置对外访问用的域名，例如 https://img.example.com
    /// </summary>
    public string PublicBaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// 获取或设置自定义 API 端点；留空时按 Region 拼出 https://oss-&lt;region&gt;.aliyuncs.com
    /// </summary>
    public string ApiEndpoint { get; set; } = string.Empty;

    /// <summary>
    /// 获取一个值，指示五项必填参数是否都齐了，缺任意一个都会自动退回本地存储
    /// </summary>
    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Region)
        && !string.IsNullOrWhiteSpace(Bucket)
        && !string.IsNullOrWhiteSpace(AccessKeyId)
        && !string.IsNullOrWhiteSpace(AccessKeySecret)
        && !string.IsNullOrWhiteSpace(PublicBaseUrl);
}
