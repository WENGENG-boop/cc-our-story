// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OurStory.Web.Infrastructure;

namespace OurStory.Web.Pages;

/// <summary>
/// 所有状态码都落到这里。
///
/// 404 用整页的「这一页暂时走丢了」，其余状态码用那张浮在半透明背景上的提示卡，
/// 和原主题两种异常页的分工一致。
/// </summary>
public class ErrorModel : PageModel {
    /// <summary>
    /// 获取或设置 Code
    /// </summary>
    public int Code { get; private set; } = 500;

    /// <summary>
    /// 处理 GET 请求
    /// </summary>
    public IActionResult OnGet(int? code) {
        Code = code is >= 400 and < 600 ? code.Value : 500;
        Response.StatusCode = Code;

        if (Code == 404) {
            return Page();
        }

        return Partial("_ExceptionDocument", new ExceptionDocumentModel(Code, Describe(Code)));
    }

    private static string Describe(int code) => code switch {
        400 => "请求里有看不懂的内容，返回上一页再试试。",
        401 => "需要先登录才能看这一页。",
        403 => "这一页不对外开放。",
        500 => "站点出了点小状况，稍后再来看看。",
        503 => "站点正在忙，缓一会儿再试。",
        _ => "出了点小状况，返回上一页再试试。"
    };
}
