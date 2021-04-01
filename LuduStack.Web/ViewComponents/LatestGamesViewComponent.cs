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

        public async Task<IViewComponentResult> InvokeAsync(int qtd, Guid userId)
        {
            if (qtd == 0)
            {
                qtd = 3;
            }

            var latestGames = await _gameAppService.GetLatest(CurrentUserId, qtd, userId, null, 0);

            List<GameListItemViewModel> model = latestGames.ToList();

            ViewData["UserId"] = userId;

            return await Task.Run(() => View(model));
        }
    }
}