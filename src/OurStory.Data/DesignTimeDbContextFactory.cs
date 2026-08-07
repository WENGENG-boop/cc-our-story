// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OurStory.Data;

/// <summary>
/// 只给 <c>dotnet ef migrations</c> 用。
///
/// 生成迁移时并不会真的连库，随便指一个文件名即可，
/// 这样就不需要把 Web 项目当作启动项目来跑设计时代码。
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OurStoryDbContext> {
    /// <summary>
    /// 创建 DbContext
    /// </summary>
    public OurStoryDbContext CreateDbContext(string[] args) {
        var builder = new DbContextOptionsBuilder<OurStoryDbContext>();
        _ = builder.UseSqlite("Data Source=design-time.db");
        return new OurStoryDbContext(builder.Options);
    }
}
