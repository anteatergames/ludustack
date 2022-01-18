using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Forum;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Identity.Models;
using LuduStack.Web.ViewComponents.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Community.ViewComponents
{
    public class ForumTopicRepliesViewComponent : BaseViewComponent
    {
        private UserManager<ApplicationUser> _userManager;
        public UserManager<ApplicationUser> UserManager => _userManager ?? (_userManager = HttpContext?.RequestServices.GetService<UserManager<ApplicationUser>>());

        private readonly IForumAppService forumAppService;

        public ForumTopicRepliesViewComponent(IHttpContextAccessor httpContextAccessor, IForumAppService forumAppService) : base(httpContextAccessor)
        {
            this.forumAppService = forumAppService;
        }

        public async Task<IViewComponentResult> InvokeAsync(Guid topicId, bool latest, int? count, int? page)
        {
            GetForumTopicRepliesRequestViewModel vm = new GetForumTopicRepliesRequestViewModel
            {
                TopicId = topicId,
                Count = count,
                Page = page,
                Latest = latest
            };

            OperationResultListVo<ForumPostViewModel> result = await forumAppService.GetTopicReplies(CurrentUserId, CurrentUserIsAdmin, vm);

            result.Pagination.Area = "community";
            result.Pagination.Controller = "forum";
            result.Pagination.Action = "repliesbytopic";

            ViewData["Pagination"] = result.Pagination;

            List<ForumPostViewModel> list = result.Value.ToList();

            return await Task.Run(() => View(list));
        }
    }
}