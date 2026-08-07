// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Options;
using OurStory.Core.Abstractions;
using OurStory.Core.Options;

namespace OurStory.Services.Storage;

internal class AttachmentService(IFileStorage storage, IOptions<StorageOptions> options) : IAttachmentService {
    private static readonly Dictionary<string, string> MimeByExtension = new(StringComparer.OrdinalIgnoreCase) {
        ["jpg"] = "image/jpeg",
        ["jpeg"] = "image/jpeg",
        ["png"] = "image/png",
        ["gif"] = "image/gif",
        ["webp"] = "image/webp",
        ["avif"] = "image/avif"
    };

    private readonly StorageOptions _options = options.Value;

    public string DriverName => storage.DriverName;

    public async Task<UploadResult> UploadAsync(Stream content, string fileName, long length, CancellationToken cancellationToken = default) {
        if (length <= 0) {
            return UploadResult.Fail("文件是空的。");
        }

        if (length > _options.MaxFileSize) {
            return UploadResult.Fail($"文件超过 {_options.MaxFileSize / 1024 / 1024} MB 的上限。");
        }

        var extension = Path.GetExtension(fileName).TrimStart('.').ToLowerInvariant();
        if (extension.Length == 0 || !_options.AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase)) {
            return UploadResult.Fail($"只支持这些格式：{string.Join("、", _options.AllowedExtensions)}。");
        }

        var contentType = MimeByExtension.GetValueOrDefault(extension, "application/octet-stream");
        var objectKey = await storage.SaveAsync(content, extension, contentType, cancellationToken);

        return UploadResult.Ok(storage.PublicUrl(objectKey), objectKey);
    }
}
