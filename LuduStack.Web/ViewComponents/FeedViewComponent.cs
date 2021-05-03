﻿using LuduStack.Application;
using LuduStack.Application.Helpers;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels;
using LuduStack.Application.ViewModels.Content;
using LuduStack.Application.ViewModels.Jobs;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Infra.CrossCutting.Identity.Models;
using LuduStack.Web.Helpers;
using LuduStack.Web.ViewComponents.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.ViewComponents
{
    public class FeedViewComponent : BaseViewComponent
    {
        private UserManager<ApplicationUser> _userManager;
        public UserManager<ApplicationUser> UserManager => _userManager ?? (_userManager = HttpContext?.RequestServices.GetService<UserManager<ApplicationUser>>());

        private readonly IUserPreferencesAppService _userPreferencesAppService;

        private readonly IUserContentAppService _userContentAppService;

        public FeedViewComponent(IHttpContextAccessor httpContextAccessor, IUserContentAppService userContentAppService, IUserPreferencesAppService userPreferencesAppService) : base(httpContextAccessor)
        {
            _userContentAppService = userContentAppService;
            _userPreferencesAppService = userPreferencesAppService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? count, Guid? gameId, Guid? userId, Guid? singleContentId, Guid? oldestId, DateTime? oldestDate, bool? articlesOnly)
        {
            int defaultCount = 10;
            List<SupportedLanguage> userLanguages = await _userPreferencesAppService.GetLanguagesByUserId(CurrentUserId);

            ActivityFeedRequestViewModel vm = new ActivityFeedRequestViewModel
            {
                CurrentUserId = CurrentUserId,
                Count = count ?? defaultCount,
                GameId = gameId,
                UserId = userId,
                SingleContentId = singleContentId,
                Languages = userLanguages,
                OldestId = oldestId,
                OldestDate = oldestDate,
                ArticlesOnly = articlesOnly
            };

            IEnumerable<UserContentViewModel> model = await _userContentAppService.GetActivityFeed(vm);

            bool userIsAdmin = User.Identity.IsAuthenticated && User.IsInRole(Roles.Administrator.ToString());

            foreach (UserContentViewModel item in model)
            {
                FillMissingInformation(userIsAdmin, item);
            }

            if (model.Any())
            {
                UserContentViewModel oldest = model.OrderByDescending(x => x.CreateDate).Last();

                ViewData["OldestPostGuid"] = oldest.Id;
                ViewData["OldestPostDate"] = oldest.CreateDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffff");
            }

            ViewData["IsMorePosts"] = oldestId.HasValue;

            ViewData["UserId"] = userId;

            ViewData["AddMoreButton"] = !singleContentId.HasValue && model.Count() >= defaultCount;

            return await Task.Run(() => View(model));
        }

        private void FillMissingInformation(bool userIsAdmin, UserContentViewModel item)
        {
            switch (item.UserContentType)
            {
                case UserContentType.TeamCreation:
                    FormatTeamCreationPost(item);
                    break;

                case UserContentType.JobPosition:
                    FormatJobPositionPostForTheFeed(item);
                    break;

                case UserContentType.ComicStrip:
                    FormatComicStripPost(item);
                    break;

                default:
                    FormatPost(item);
                    break;
            }

            FormatCommon(userIsAdmin, item);
        }

        private void FormatCommon(bool userIsAdmin, UserContentViewModel item)
        {
            foreach (CommentViewModel comment in item.Comments)
            {
                comment.Text = ContentFormatter.FormatHashTagsToShow(comment.Text);
            }

            item.Permissions.CanEdit = !item.HasPoll && (item.UserId == CurrentUserId || userIsAdmin);

            item.Permissions.CanDelete = item.UserId == CurrentUserId || userIsAdmin;
        }

        private void FormatComicStripPost(UserContentViewModel item)
        {
            item.Url = Url.Action("details", "comics", new { area = "member", id = item.Id }, (string)ViewData["protocol"], (string)ViewData["host"]);
        }

        private void FormatPost(UserContentViewModel item)
        {
            item.Content = ContentFormatter.FormatContentToShow(item.Content);
            if (item.FeaturedMediaType == MediaType.Youtube)
            {
                item.FeaturedImageResponsive = ContentFormatter.GetYoutubeVideoId(item.FeaturedImage);
                item.FeaturedImageLquip = ContentHelper.FormatFeaturedImageUrl(Guid.Empty, Constants.DefaultFeaturedImageLquip, ImageRenderType.LowQuality);
            }

            item.Url = Url.Action("details", "content", new { area = string.Empty, id = item.Id }, (string)ViewData["protocol"], (string)ViewData["host"]);
        }

        private void FormatTeamCreationPost(UserContentViewModel item)
        {
            string[] teamData = item.Content.Split('|');
            string id = teamData[0];
            string name = teamData[1];
            string motto = teamData[2];
            string memberCount = teamData[3];
            bool recruiting = false;

            if (teamData.Length > 4)
            {
                recruiting = bool.Parse(teamData[4] ?? "False");
            }

            string postTemplate = ContentFormatter.FormatUrlContentToShow(item.UserContentType);
            string translatedText = SharedLocalizer["A new team has been created with {0} members.", memberCount].ToString();

            if (recruiting)
            {
                translatedText = SharedLocalizer["A team is recruiting!", memberCount].ToString();
            }

            item.Content = String.Format(postTemplate, translatedText, name, motto);
            item.Url = Url.Action("Details", "Team", new { area = string.Empty, teamId = id }, (string)ViewData["protocol"], (string)ViewData["host"]);
            item.Language = SupportedLanguage.English;
        }

        private void FormatJobPositionPostForTheFeed(UserContentViewModel item)
        {
            JobPositionViewModel obj = JsonConvert.DeserializeObject<JobPositionViewModel>(item.Content);

            SupportedLanguage language = SupportedLanguage.English;

            if (obj.Remote)
            {
                obj.Location = SharedLocalizer["remote"];
            }

            if (obj.Language != 0)
            {
                language = obj.Language;
            }

            string postTemplate = ContentFormatter.FormatUrlContentToShow(item.UserContentType);
            string translatedText = SharedLocalizer["A new job position for {0}({1}) is open for applications.", SharedLocalizer[obj.WorkType.ToDisplayName()], obj.Location].ToString();

            item.Content = String.Format(postTemplate, translatedText, SharedLocalizer[obj.WorkType.ToDisplayName()], obj.Location);
            item.Url = Url.Action("Details", "JobPosition", new { area = "Work", id = obj.Id.ToString() }, (string)ViewData["protocol"], (string)ViewData["host"]);
            item.Language = language;
        }
    }
}