using LuduStack.Application;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Game;
using LuduStack.Web.ViewComponents.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.ViewComponents
{
    public class LatestGamesViewComponent : BaseViewComponent
    {
        private readonly IGameAppService _gameAppService;

        public LatestGamesViewComponent(IHttpContextAccessor httpContextAccessor, IGameAppService gameAppService) : base(httpContextAccessor)
        {
            _gameAppService = gameAppService;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool placeholder, int qtd, Guid userId)
        {
            ViewData["UserId"] = userId;

            if (placeholder)
            {
                ViewData["placeholder"] = true;

                List<GameListItemViewModel> model = new List<GameListItemViewModel>();
                for (int i = 0; i < qtd; i++)
                {
                    model.Add(new GameListItemViewModel
                    {
                        Title = "placeholder",
                        DeveloperHandler = "placeholder",
                        DeveloperName = "placeholder",
                        DeveloperImageUrl = Constants.DefaultAvatar30
                    });
                }

                return await Task.Run(() => View(model));
            }
            else
            {
                if (qtd == 0)
                {
                    qtd = 3;
                }

                IEnumerable<GameListItemViewModel> latestGames = await _gameAppService.GetLatest(CurrentUserId, qtd, userId, null, 0);

                List<GameListItemViewModel> model = latestGames.ToList();

                return await Task.Run(() => View(model));
            }
        }
    }
}