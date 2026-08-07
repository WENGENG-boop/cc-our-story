// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using OurStory.Core;
using OurStory.Core.Entities;
using OurStory.Data;

namespace OurStory.Services.Accounts;

internal class UserService(OurStoryDbContext db) : IUserService {
    public async Task<User?> AuthenticateAsync(string userName, string password, CancellationToken cancellationToken = default) {
        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrEmpty(password)) {
            return null;
        }

        var normalized = userName.Trim();
        var user = await db.Users.FirstOrDefaultAsync(item => item.UserName == normalized, cancellationToken);

        if (user is null || !user.IsActive || !PasswordHasher.Verify(password, user.PasswordHash)) {
            return null;
        }

        user.LastLoginAt = DateTimeOffset.UtcNow;
        _ = await db.SaveChangesAsync(cancellationToken);
        return user;
    }

    public Task<User?> FindAsync(int id, CancellationToken cancellationToken = default) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Id == id, cancellationToken);

    public Task<User?> FindByNameAsync(string userName, CancellationToken cancellationToken = default) {
        // 登录名那一列建表时就是 NOCASE，比较直接落到 SQL 里，大小写无所谓
        var normalized = (userName ?? string.Empty).Trim();
        return db.Users.AsNoTracking().FirstOrDefaultAsync(user => user.UserName == normalized, cancellationToken);
    }

    public async Task<IReadOnlyList<User>> ListAsync(CancellationToken cancellationToken = default) =>
        await db.Users.AsNoTracking().OrderBy(user => user.Id).ToListAsync(cancellationToken);

    public async Task<bool> ChangePasswordAsync(int id, string currentPassword, string newPassword, CancellationToken cancellationToken = default) {
        var user = await db.Users.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (user is null || !PasswordHasher.Verify(currentPassword, user.PasswordHash)) {
            return false;
        }

        user.PasswordHash = PasswordHasher.Hash(newPassword);
        _ = await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task ResetPasswordAsync(int id, string newPassword, CancellationToken cancellationToken = default) {
        var user = await db.Users.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (user is null) {
            return;
        }

        user.PasswordHash = PasswordHasher.Hash(newPassword);
        _ = await db.SaveChangesAsync(cancellationToken);
    }

    public Task<User?> FindByRoleAsync(UserRole role, CancellationToken cancellationToken = default) => 
        db.Users.AsNoTracking().FirstOrDefaultAsync(user => user.Role == role, cancellationToken);
}
