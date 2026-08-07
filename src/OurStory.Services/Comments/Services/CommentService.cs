// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using OurStory.Core.Entities;
using OurStory.Core.Models;
using OurStory.Core.Time;
using OurStory.Data;
using OurStory.Services.Settings;
using System.Net;
using System.Text;

namespace OurStory.Services.Comments;

internal class CommentService(OurStoryDbContext db, ISettingsService settings, SiteClock clock) : ICommentService {
    public async Task<IReadOnlyList<CommentNode>> GetTreeAsync(int momentId, CancellationToken cancellationToken = default) {
        var site = await settings.GetAsync(cancellationToken);

        var comments = await db.Comments
            .Where(comment => comment.MomentId == momentId && comment.IsApproved)
            .OrderBy(comment => comment.CreatedAt)
            .Include(comment => comment.Author)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var nodes = comments.ToDictionary(
            comment => comment.Id,
            comment => new CommentNode {
                Id = comment.Id,
                AuthorName = DisplayName(comment, site),
                AuthorUrl = comment.AuthorUrl,
                ContentHtml = ToHtml(comment.Content),
                CreatedAt = clock.ToLocal(comment.CreatedAt),
                IsOwner = comment.AuthorId is not null
            });

        var roots = new List<CommentNode>();
        foreach (var comment in comments) {
            var node = nodes[comment.Id];
            // 父留言被删掉的孤儿回复直接当顶层显示，不让它凭空消失
            if (comment.ParentId is { } parentId && nodes.TryGetValue(parentId, out var parent)) {
                parent.Replies.Add(node);
            } else {
                roots.Add(node);
            }
        }

        return roots;
    }

    public async Task<Comment> AddAsync(CommentSubmission submission, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(submission);

        var comment = new Comment {
            MomentId = submission.MomentId,
            ParentId = submission.ParentId,
            AuthorId = submission.AuthorId,
            AuthorName = submission.AuthorName.Trim(),
            AuthorMail = Trim(submission.AuthorMail),
            AuthorUrl = Trim(submission.AuthorUrl),
            Content = submission.Content.Trim(),
            VisitorHash = submission.VisitorHash,
            IsApproved = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _ = db.Comments.Add(comment);
        _ = await db.SaveChangesAsync(cancellationToken);
        return comment;
    }

    public async Task<PagedList<Comment>> ListForAdminAsync(int page, int pageSize, CancellationToken cancellationToken = default) {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 20 : pageSize;

        var total = await db.Comments.CountAsync(cancellationToken);
        var items = await db.Comments
            .OrderByDescending(comment => comment.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Include(comment => comment.Moment)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return new PagedList<Comment>(items, page, pageSize, total);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default) =>
        db.Comments.CountAsync(cancellationToken);

    public async Task<bool> SetApprovedAsync(int id, bool approved, CancellationToken cancellationToken = default) {
        var comment = await db.Comments.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (comment is null) {
            return false;
        }

        comment.IsApproved = approved;
        _ = await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default) {
        var comment = await db.Comments.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (comment is null) {
            return false;
        }

        // 外键是 Restrict，所以先把挂在它下面的回复摘下来变成顶层
        var replies = await db.Comments.Where(item => item.ParentId == id).ToListAsync(cancellationToken);
        foreach (var reply in replies) {
            reply.ParentId = null;
        }

        _ = db.Comments.Remove(comment);
        _ = await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    #region 私有方法

    private static string ToHtml(string text) {
        if (string.IsNullOrWhiteSpace(text)) {
            return string.Empty;
        }

        var builder = new StringBuilder();
        var paragraphs = text.Replace("\r\n", "\n", StringComparison.Ordinal)
            .Split("\n\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var paragraph in paragraphs) {
            _ = builder.Append("<p>")
                .Append(WebUtility.HtmlEncode(paragraph).Replace("\n", "<br>", StringComparison.Ordinal))
                .Append("</p>");
        }

        return builder.ToString();
    }

    /// <summary>
    /// 自己人的留言按站点设置里的称呼显示，改了名字连旧留言一起跟着变；
    /// 访客没有账号，只能用当时填的名字。
    /// </summary>
    private static string DisplayName(Comment comment, SiteSettings site) =>
        comment.Author is null ? comment.AuthorName : site.RoleName(comment.Author.Role);

    private static string? Trim(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    #endregion
}
