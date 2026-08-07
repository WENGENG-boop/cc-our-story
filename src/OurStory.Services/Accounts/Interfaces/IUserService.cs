// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using OurStory.Core;
using OurStory.Core.Entities;

namespace OurStory.Services.Accounts;

/// <summary>
/// 提供用户认证、查询及密码管理等服务
/// </summary>
/// <remarks>
/// 页面上的称呼不在这里，男主 / 女主叫什么由站点设置说了算，见 <see cref="OurStory.Core.Models.SiteSettings.RoleName"/>
/// </remarks>
public interface IUserService {
    /// <summary>
    /// 异步校验用户登录信息
    /// </summary>
    /// <param name="userName">用户登录名</param>
    /// <param name="password">用户登录密码</param>
    /// <param name="cancellationToken">异步取消令牌</param>
    /// <returns>异步操作任务结果，验证成功的用户信息；验证失败返回 null</returns>
    Task<User?> AuthenticateAsync(string userName, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据用户编号异步查询用户信息
    /// </summary>
    /// <param name="id">用户唯一编号</param>
    /// <param name="cancellationToken">异步取消令牌</param>
    /// <returns>异步操作任务结果，对应用户信息；不存在时返回 null</returns>
    Task<User?> FindAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据登录名异步查询用户信息。登录名比较不区分大小写，并会自动去除首尾空白字符
    /// </summary>
    /// <param name="userName">用户登录名</param>
    /// <param name="cancellationToken">异步取消令牌</param>
    /// <returns>异步操作任务结果，对应用户信息；不存在时返回 null</returns>
    Task<User?> FindByNameAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步获取全部用户列表。查询结果按照用户编号升序排列
    /// </summary>
    /// <param name="cancellationToken">异步取消令牌</param>
    /// <returns>异步操作任务结果，用户只读列表</returns>
    Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步修改用户密码
    /// </summary>
    /// <param name="id">用户唯一编号</param>
    /// <param name="currentPassword">当前密码</param>
    /// <param name="newPassword">新密码</param>
    /// <param name="cancellationToken">异步取消令牌</param>
    /// <returns>异步操作任务结果，修改成功返回 true，否则返回 false</returns>
    Task<bool> ChangePasswordAsync(int id, string currentPassword, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步重置用户密码
    /// </summary>
    /// <param name="id">用户唯一编号</param>
    /// <param name="newPassword">新的密码</param>
    /// <param name="cancellationToken">异步取消令牌</param>
    /// <returns>表示一个异步操作任务</returns>
    Task ResetPasswordAsync(int id, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步根据用户角色异步查询对应账号
    /// </summary>
    /// <param name="role">用户角色</param>
    /// <param name="cancellationToken">异步取消令牌</param>
    /// <returns>异步操作任务结果，对应角色用户；不存在时返回 null</returns>
    Task<User?> FindByRoleAsync(UserRole role, CancellationToken cancellationToken = default);
}
