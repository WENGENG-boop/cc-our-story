// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

namespace OurStory.Services.Storage;

/// <summary>
/// 定义附件存储服务契约：提供附件上传以及存储驱动信息访问能力
/// </summary>
public interface IAttachmentService {
    /// <summary>
    /// 获取当前使用的文件存储驱动名称
    /// </summary>
    string DriverName { get; }

    /// <summary>
    /// 异步上传附件
    /// </summary>
    /// <param name="content">文件内容流</param>
    /// <param name="fileName">原始文件名称</param>
    /// <param name="length">文件长度，单位为字节</param>
    /// <param name="cancellationToken">异步取消令牌</param>
    /// <returns>异步操作任务结果，一个上传结果，包含文件访问地址以及存储对象标识</returns>
    Task<UploadResult> UploadAsync(
        Stream content,
        string fileName,
        long length,
        CancellationToken cancellationToken = default);
}
