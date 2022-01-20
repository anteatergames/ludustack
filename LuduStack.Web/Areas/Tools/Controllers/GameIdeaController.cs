using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.GameIdea;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Areas.Tools.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Tools.Controllers
{
    public class GameIdeaController : ToolsBaseController
    {
        private readonly IGameIdeaAppService gameIdeaAppService;

        public GameIdeaController(IGameIdeaAppService gameIdeaAppService)
        {
            this.gameIdeaAppService = gameIdeaAppService;
        }

        [Route("tools/gameidea")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("tools/gameidea/getelements")]
        public async Task<IActionResult> GetElements()
        {
            List<SupportedLanguage> allLanguages = Enum.GetValues(typeof(SupportedLanguage)).Cast<SupportedLanguage>().ToList();
            SupportedLanguage langEnum = allLanguages.FirstOrDefault(x => x.ToUiInfo().Locale.Equals(CurrentLocale));

            OperationResultVo<GameIdeaListViewModel> elements = await gameIdeaAppService.Get(CurrentUserId, langEnum);

            return Json(elements);
        }
    }
}