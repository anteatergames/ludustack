using LuduStack.Application;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.GameIdea;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Areas.Staff.Controllers.Base;
using LuduStack.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Staff.Controllers
{
    [Route("staff/gameidea")]
    public class GameIdeaController : StaffBaseController
    {
        private readonly IGameIdeaAppService gameIdeaAppService;

        public GameIdeaController(IGameIdeaAppService gameIdeaAppService)
        {
            this.gameIdeaAppService = gameIdeaAppService;
        }

        [Route("{language?}")]
        public IActionResult Index(SupportedLanguage? language)
        {
            GameIdeaFilterViewModel model = new GameIdeaFilterViewModel();

            if (language.HasValue)
            {
                model.Language = language.Value;
            }

            return View(model);
        }

        [Route("list/{language}")]
        public async Task<PartialViewResult> List(SupportedLanguage? language)
        {
            GameIdeaListViewModel model;

            OperationResultVo serviceResult = await gameIdeaAppService.Get(CurrentUserId, language);

            if (serviceResult.Success)
            {
                OperationResultVo<GameIdeaListViewModel> castResult = serviceResult as OperationResultVo<GameIdeaListViewModel>;

                model = castResult.Value;
            }
            else
            {
                model = new GameIdeaListViewModel();
            }

            SetPermissions(model.Elements);

            return PartialView("_ListGameIdeas", model);
        }

        [Route("add/{type}/{language}")]
        public IActionResult Add(GameIdeaElementType type, SupportedLanguage language)
        {
            GameIdeaViewModel model = new GameIdeaViewModel();

            model.Type = type;
            model.Language = language;

            return View("CreateEditWrapper", model);
        }

        [HttpPost("gameidea/validate")]
        public async Task<IActionResult> ValidateDescription(string description, Guid Id, SupportedLanguage language, GameIdeaElementType type)
        {
            OperationResultVo result;

            try
            {
                OperationResultVo validate = await gameIdeaAppService.ValidateDescription(CurrentUserId, description, Id, language, type);

                return Json(validate.Success);
            }
            catch (Exception)
            {
                result = new OperationResultVo(false);
            }

            return Json(result);
        }

        [Route("edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            GameIdeaViewModel viewModel;

            OperationResultVo serviceResult = await gameIdeaAppService.GetById(CurrentUserId, id);

            OperationResultVo<GameIdeaViewModel> castResult = serviceResult as OperationResultVo<GameIdeaViewModel>;

            viewModel = castResult.Value;

            SetPermissions(viewModel);

            return View("CreateEditWrapper", viewModel);
        }

        [HttpPost("save")]
        public async Task<JsonResult> Save(GameIdeaViewModel vm)
        {
            bool isNew = vm.Id == Guid.Empty;

            try
            {
                vm.UserId = CurrentUserId;

                OperationResultVo<Guid> saveResult = await gameIdeaAppService.Save(CurrentUserId, vm);

                if (saveResult.Success)
                {
                    string url = Url.Action("index", "gameidea", new { area = "staff", language = (int)vm.Language, msg = SharedLocalizer[saveResult.Message] });

                    if (isNew && EnvName.Equals(Constants.ProductionEnvironmentName))
                    {
                        await NotificationSender.SendTeamNotificationAsync("New Game Idea created!");
                    }

                    return Json(new OperationResultRedirectVo<Guid>(saveResult, url));
                }
                else
                {
                    return Json(saveResult);
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, bool edit)
        {
            try
            {
                OperationResultVo deleteResult = await gameIdeaAppService.Remove(CurrentUserId, id);

                if (deleteResult.Success)
                {
                    if (edit)
                    {
                        string url = Url.Action("index", "gameidea", new { area = "staff", msg = deleteResult.Message });
                        deleteResult.Message = null;

                        return Json(new OperationResultRedirectVo(deleteResult, url));
                    }

                    return Json(deleteResult);
                }
                else
                {
                    return Json(new OperationResultVo(false));
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        private void SetPermissions(Dictionary<GameIdeaElementType, List<GameIdeaViewModel>> dictionary)
        {
            foreach (KeyValuePair<GameIdeaElementType, List<GameIdeaViewModel>> entry in dictionary)
            {
                foreach (GameIdeaViewModel gameIdeaElement in entry.Value)
                {
                    SetPermissions(gameIdeaElement);
                }
            }
        }

        private void SetPermissions(GameIdeaViewModel viewModel)
        {
            viewModel.Permissions.IsAdmin = CurrentUserIsAdmin;
            viewModel.Permissions.CanDelete = viewModel.Permissions.IsAdmin;
        }
    }
}