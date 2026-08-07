// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using OurStory.Core.Entities;
using OurStory.Core.Models;

namespace OurStory.Services.Comments;

/// <summary>
/// 留言服务契约接口
/// </summary>
public interface ICommentService {
    /// <summary>
    /// 异步获取留言树
    /// </summary>
    /// <param name="momentId">动态编号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>异步操作任务，留言树列表</returns>
    Task<IReadOnlyList<CommentNode>> GetTreeAsync(int momentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步添加留言
    /// </summary>
    /// <param name="submission">评论提交信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>异步操作任务，新增评论实体</returns>
    Task<Comment> AddAsync(CommentSubmission submission, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取留言列表
    /// </summary>
    /// <param name="page">页码</param>
    /// <param name="pageSize">每页数量</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>异步操作任务，分页留言列表</returns>
    Task<PagedList<Comment>> ListForAdminAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取留言总数
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>异步操作任务，留言总数量</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步设置留言是否审核通过
    /// </summary>
    /// <param name="id">留言编号</param>
    /// <param name="approved">是否审核通过</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>异步操作任务，设置结果</returns>
    Task<bool> SetApprovedAsync(int id, bool approved, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步删除留言
    /// </summary>
    /// <param name="id">留言编号</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>异步操作任务，删除结果</returns>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
