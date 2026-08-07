// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using OurStory.Services.Heartbeats;
using OurStory.Web.Infrastructure;
using System.Globalization;

namespace OurStory.Web.Api;

/// <summary>
/// 表示 HeartbeatEndpoints
/// </summary>
public static class HeartbeatEndpoints {
    /// <summary>
    /// 首页爱心的接收端。
    ///
    /// 请求体沿用原来的字段名（<c>_</c> 是令牌，<c>taps[]</c> 是每次点击距今多少毫秒），
    /// 所以前端脚本一行都不用改。
    /// </summary>
    public static void MapHeartbeatEndpoints(this WebApplication app) {
        ArgumentNullException.ThrowIfNull(app);

        _ = app.MapPost("/api/heartbeat", async (
            HttpContext context,
            IHeartbeatService heartbeats,
            VisitorIdentityAccessor visitors,
            HeartbeatTokenService tokens,
            ILoggerFactory loggerFactory,
            CancellationToken cancellationToken) => {
                if (!context.Request.HasFormContentType) {
                    return Results.Json(new { ok = false, message = "这个地址只接受表单提交。" }, statusCode: StatusCodes.Status415UnsupportedMediaType);
                }

                var form = await context.Request.ReadFormAsync(cancellationToken);
                var who = await visitors.GetAsync(cancellationToken);

                if (!tokens.Validate(form["_"].ToString(), who)) {
                    return Results.Json(new { ok = false, message = "页面停留太久啦，刷新一下再点。" }, statusCode: StatusCodes.Status403Forbidden);
                }

                var ages = form["taps[]"]
                    .Select(value => int.TryParse(value, CultureInfo.InvariantCulture, out var age) ? age : 0)
                    .ToList();

                try {
                    var result = await heartbeats.RecordAsync(who, ages, cancellationToken);
                    var summary = await heartbeats.GetSummaryAsync(who, cancellationToken);

                    return Results.Json(new {
                        ok = true,
                        accepted = result.Accepted,
                        limited = result.Limited,
                        dailyLimit = summary.DailyLimit,
                        role = summary.Role,
                        totals = summary.Totals,
                        self = summary.Self,
                        display = summary.Display
                    });
                } catch (Exception exception) when (exception is not OperationCanceledException) {
                    loggerFactory.CreateLogger("OurStory.Heartbeat").LogError(exception, "心动记录失败");
                    return Results.Json(new { ok = false, message = "这一下没记上，等会儿再点点看。" }, statusCode: StatusCodes.Status500InternalServerError);
                }
            })
        // 令牌自己带在 _ 字段里，不走 Razor Pages 那套防伪
        .DisableAntiforgery();
    }
}
