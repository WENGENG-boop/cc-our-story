// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

namespace OurStory.Services.Storage;

/// <summary>上传结果：成功时给出可以直接贴进正文的地址</summary>
/// <param name="Success">是否上传成功</param>
/// <param name="Url">对外访问地址</param>
/// <param name="ObjectKey">存储层的 key，删除时要用</param>
/// <param name="Error">失败原因，直接显示给后台</param>
public readonly record struct UploadResult(bool Success, string Url, string ObjectKey, string Error) {
    /// <summary>
    /// 创建失败结果
    /// </summary>
    public static UploadResult Fail(string error) => new(false, string.Empty, string.Empty, error);

    /// <summary>
    /// 创建成功结果
    /// </summary>
    public static UploadResult Ok(string url, string objectKey) => new(true, url, objectKey, string.Empty);
}
