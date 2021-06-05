using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Forum;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Identity.Models;
using LuduStack.Web.ViewComponents.Base;
using Markdig;
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
    public class ForumTopicAnswersViewComponent : BaseViewComponent
    {
        private UserManager<ApplicationUser> _userManager;
        public UserManager<ApplicationUser> UserManager => _userManager ?? (_userManager = HttpContext?.RequestServices.GetService<UserManager<ApplicationUser>>());

        private readonly IUserPreferencesAppService _userPreferencesAppService;

        private readonly IForumAppService forumAppService;

        public ForumTopicAnswersViewComponent(IHttpContextAccessor httpContextAccessor, IForumAppService forumAppService, IUserPreferencesAppService userPreferencesAppService) : base(httpContextAccessor)
        {
            this.forumAppService = forumAppService;
            _userPreferencesAppService = userPreferencesAppService;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? count, Guid topicId)
        {

            GetForumTopicAnswersRequestViewModel vm = new GetForumTopicAnswersRequestViewModel
            {
                TopicId = topicId
            };

            OperationResultListVo<ForumPostViewModel> model = await forumAppService.GetTopicAnswers(CurrentUserId, vm);

            List<ForumPostViewModel> list = model.Value.ToList();

            return await Task.Run(() => View(list));
        }
    }
}