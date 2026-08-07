// Copyright (c) 2026 Keeleycenc.
// Licensed under the MIT License.
// See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OurStory.Core.Models;
using OurStory.Services.Settings;
using System.ComponentModel.DataAnnotations;

namespace OurStory.Web.Areas.Admin.Pages;

/// <summary>
/// 表示 SettingsModel
/// </summary>
public class SettingsModel(ISettingsService settings) : PageModel {
    /// <summary>
    /// 执行 Input 操作
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; } = new();

    /// <summary>
    /// 获取或设置 Error
    /// </summary>
    public string? Error { get; private set; }

    /// <summary>
    /// 处理 GET 请求
    /// </summary>
    public async Task OnGetAsync(CancellationToken cancellationToken) {
        var site = await settings.GetAsync(cancellationToken);

        Input = new InputModel {
            SiteTitle = site.SiteTitle,
            SiteDescription = site.SiteDescription,
            BoyName = site.BoyName,
            GirlName = site.GirlName,
            BoyAvatar = site.BoyAvatar,
            GirlAvatar = site.GirlAvatar,
            BoySentence = site.BoySentence,
            GirlSentence = site.GirlSentence,
            LoveStartedAt = site.LoveStartedAt,
            HomeSentence = site.HomeSentence,
            DailyNote = site.DailyNote,
            LoveLetters = string.Join('\n', site.LoveLetters),
            ColorMode = site.ColorMode,
            MomentsPageSize = site.MomentsPageSize,
            HeartbeatDailyLimit = site.HeartbeatDailyLimit,
            CommentsRequireMail = site.CommentsRequireMail,
            AllowGuestComments = site.AllowGuestComments
        };
    }

    /// <summary>
    /// 处理 Async(CancellationToken) 的 POST 请求
    /// </summary>
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken) {
        if (!ModelState.IsValid) {
            Error = string.Join("；", ModelState.Values.SelectMany(state => state.Errors).Select(error => error.ErrorMessage));
            return Page();
        }

        var site = await settings.GetAsync(cancellationToken);

        site.SiteTitle = Input.SiteTitle;
        site.SiteDescription = Input.SiteDescription;
        site.BoyName = Input.BoyName;
        site.GirlName = Input.GirlName;
        site.BoyAvatar = Input.BoyAvatar ?? string.Empty;
        site.GirlAvatar = Input.GirlAvatar ?? string.Empty;
        site.BoySentence = Input.BoySentence;
        site.GirlSentence = Input.GirlSentence;
        site.LoveStartedAt = Input.LoveStartedAt;
        site.HomeSentence = Input.HomeSentence;
        site.DailyNote = Input.DailyNote;
        site.ColorMode = Input.ColorMode;
        site.MomentsPageSize = Input.MomentsPageSize;
        site.HeartbeatDailyLimit = Input.HeartbeatDailyLimit;
        site.CommentsRequireMail = Input.CommentsRequireMail;
        site.AllowGuestComments = Input.AllowGuestComments;

        // 情话在后台按「一行一句」编辑，存进去还是 JSON 数组
        var letters = (Input.LoveLetters ?? string.Empty)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();
        site.LoveLetters = letters.Count > 0 ? letters : SiteSettings.DefaultLoveLetters;

        await settings.SaveAsync(site, cancellationToken);

        TempData["Flash"] = "设置已经保存。";
        return RedirectToPage();
    }

    /// <summary>
    /// 表示 InputModel
    /// </summary>
    public class InputModel {
        /// <summary>
        /// 获取或设置 SiteTitle
        /// </summary>
        [Required(ErrorMessage = "站点名称不能为空")]
        [StringLength(60)]
        public string SiteTitle { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置 SiteDescription
        /// </summary>
        [StringLength(120)]
        public string SiteDescription { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置 BoyName
        /// </summary>
        [Required(ErrorMessage = "男主名字不能为空")]
        [StringLength(32)]
        public string BoyName { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置 GirlName
        /// </summary>
        [Required(ErrorMessage = "女主名字不能为空")]
        [StringLength(32)]
        public string GirlName { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置 BoyAvatar
        /// </summary>
        [StringLength(500)]
        public string? BoyAvatar { get; set; }

        /// <summary>
        /// 获取或设置 GirlAvatar
        /// </summary>
        [StringLength(500)]
        public string? GirlAvatar { get; set; }

        /// <summary>
        /// 获取或设置 BoySentence
        /// </summary>
        [StringLength(60)]
        public string BoySentence { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置 GirlSentence
        /// </summary>
        [StringLength(60)]
        public string GirlSentence { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置 LoveStartedAt
        /// </summary>
        [DataType(DataType.DateTime)]
        public DateTime LoveStartedAt { get; set; }

        /// <summary>
        /// 获取或设置 HomeSentence
        /// </summary>
        [StringLength(200)]
        public string HomeSentence { get; set; } = string.Empty;

        /// <summary>
        /// 获取或设置 DailyNote
        /// </summary>
        [StringLength(400)]
        public string DailyNote { get; set; } = string.Empty;

        /// <summary>一行一句。</summary>
        public string? LoveLetters { get; set; }

        /// <summary>
        /// 获取或设置 ColorMode
        /// </summary>
        public string ColorMode { get; set; } = "auto";

        /// <summary>
        /// 获取或设置 MomentsPageSize
        /// </summary>
        [Range(1, 100, ErrorMessage = "每页条数要在 1 到 100 之间")]
        public int MomentsPageSize { get; set; } = 10;

        /// <summary>
        /// 获取或设置 HeartbeatDailyLimit
        /// </summary>
        [Range(1, 9999, ErrorMessage = "每日上限要在 1 到 9999 之间")]
        public int HeartbeatDailyLimit { get; set; } = 99;

        /// <summary>
        /// 获取或设置 CommentsRequireMail
        /// </summary>
        public bool CommentsRequireMail { get; set; }

        /// <summary>
        /// 获取或设置 AllowGuestComments
        /// </summary>
        public bool AllowGuestComments { get; set; } = true;
    }
}
