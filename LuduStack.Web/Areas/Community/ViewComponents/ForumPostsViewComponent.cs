using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Forum;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Identity.Models;
using LuduStack.Web.Helpers;
using LuduStack.Web.ViewComponents.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Community.ViewComponents
{
    public class ForumPostsViewComponent : BaseViewComponent
    {
        private UserManager<ApplicationUser> _userManager;
        public UserManager<ApplicationUser> UserManager => _userManager ?? (_userManager = HttpContext?.RequestServices.GetService<UserManager<ApplicationUser>>());

        private readonly IForumAppService forumAppService;

        public ForumPostsViewComponent(IHttpContextAccessor httpContextAccessor, IForumAppService forumAppService) : base(httpContextAccessor)
        {
            this.forumAppService = forumAppService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? count, int? page, Guid? categoryId)
        {
            List<SupportedLanguage> userLanguages = await GetCurrentUserContentLanguage();

            GetForumPostsRequestViewModel vm = new GetForumPostsRequestViewModel
            {
                ForumCategoryId = categoryId,
                Count = count,
                Page = page,
                Languages = userLanguages
            };

            OperationResultVo<ForumPostListVo> result = await forumAppService.GetPosts(CurrentUserId, vm);

            ForumPostListVo model = result.Value;

            FillMissingInformation(model.Posts);

            model.Pagination.Area = "community";
            model.Pagination.Controller = "forum";
            model.Pagination.Action = "postsbycategory";

            return await Task.Run(() => View(model));
        }

        private void FillMissingInformation(List<ForumPostListItemVo> model)
        {
            foreach (ForumPostListItemVo forumPost in model)
            {
                forumPost.CreatedRelativeTime = DateTimeHelper.DateTimeToCreatedAgoMessage(forumPost.CreateDate, SharedLocalizer);

                if (forumPost.LatestReply != null)
                {
                    forumPost.LatestReply.CreatedRelativeTime = DateTimeHelper.DateTimeToCreatedAgoMessage(forumPost.LatestReply.CreateDate, SharedLocalizer);
                }
            }
        }
    }
}