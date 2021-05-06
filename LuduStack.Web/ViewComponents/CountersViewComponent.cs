using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Home;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.ViewComponents.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LuduStack.Web.ViewComponents
{
    public class CountersViewComponent : BaseViewComponent
    {
        private readonly IGameAppService gameAppService;
        private readonly IProfileAppService profileAppService;
        private readonly IUserContentAppService contentService;
        private readonly ITeamAppService teamAppService;

        public CountersViewComponent(IHttpContextAccessor httpContextAccessor
            , IGameAppService gameAppService
            , IProfileAppService profileAppService
            , IUserContentAppService contentService
            , ITeamAppService teamAppService) : base(httpContextAccessor)
        {
            this.gameAppService = gameAppService;
            this.profileAppService = profileAppService;
            this.contentService = contentService;
            this.teamAppService = teamAppService;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool placeholder)
        {
            CountersViewModel model = new CountersViewModel();

            if (placeholder)
            {
                ViewData["placeholder"] = true;
            }
            else
            {
                OperationResultVo<int> gamesCount = await gameAppService.Count(CurrentUserId);

                if (gamesCount.Success)
                {
                    model.GamesCount = gamesCount.Value;
                }

                OperationResultVo<int> usersCount = await profileAppService.Count(CurrentUserId);

                if (usersCount.Success)
                {
                    model.UsersCount = usersCount.Value;
                }

                model.ArticlesCount = await contentService.CountArticles();

                OperationResultVo<int> teamCount = await teamAppService.Count(CurrentUserId);

                if (teamCount.Success)
                {
                    model.TeamCount = teamCount.Value;
                }
            }

            return await Task.Run(() => View(model));
        }
    }
}