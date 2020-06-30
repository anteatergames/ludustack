using LuduStack.Application.ViewModels.Giveaway;
using LuduStack.Web.Areas.Tools.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LuduStack.Web.Areas.Tools.Controllers
{
    public class GiveawayController : ToolsBaseController
    {
        public IActionResult Index()
        {
            return View();
        }


        [Authorize]
        [Route("tools/giveaway/add/")]
        public IActionResult Add()
        {
            //OperationResultVo serviceResult = translationAppService.GenerateNew(CurrentUserId);

            //if (serviceResult.Success)
            //{
            //    OperationResultVo<LocalizationViewModel> castResult = serviceResult as OperationResultVo<LocalizationViewModel>;

            //    GiveawayViewModel model = castResult.Value;

            //    SetLocalization(model, true);

            //    OperationResultVo gamesResult = translationAppService.GetMyUntranslatedGames(CurrentUserId);
            //    if (gamesResult.Success)
            //    {
            //        OperationResultListVo<SelectListItemVo> castResultGames = gamesResult as OperationResultListVo<SelectListItemVo>;

            //        IEnumerable<SelectListItemVo> games = castResultGames.Value;

            //        List<SelectListItem> gamesDropDown = games.ToSelectList();
            //        ViewBag.UserGames = gamesDropDown;
            //    }
            //    else
            //    {
            //        ViewBag.UserGames = new List<SelectListItem>();
            //    }
            //    return View("CreateEditWrapper", model);
            //}
            //else
            //{
            //    return View("CreateEditWrapper", new GiveawayViewModel());
            //}

            return View("CreateEditWrapper", new GiveawayViewModel());
        }
    }
}
