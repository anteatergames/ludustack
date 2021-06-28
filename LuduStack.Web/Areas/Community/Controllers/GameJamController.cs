using LuduStack.Application;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.GameJam;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Areas.Community.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Staff.Controllers
{
    public class GameJamController : CommunityBaseController
    {
        private readonly IGameJamAppService gameJamAppService;

        public GameJamController(IGameJamAppService gameJamAppService)
        {
            this.gameJamAppService = gameJamAppService;
        }

        [Route("/gamejam")]
        public IActionResult Index(string msg)
        {
            if (!string.IsNullOrWhiteSpace(msg))
            {
                TempData["Message"] = SharedLocalizer[msg];
            }

            return View();
        }

        [Route("/gamejam/manage")]
        public IActionResult Manage(string msg)
        {
            if (!string.IsNullOrWhiteSpace(msg))
            {
                TempData["Message"] = SharedLocalizer[msg];
            }

            return View();
        }

        [Route("/gamejam/list")]
        public async Task<PartialViewResult> List()
        {
            List<GameJamViewModel> model;

            OperationResultVo serviceResult = await gameJamAppService.GetAll(CurrentUserId, CurrentUserIsAdmin);

            if (serviceResult.Success)
            {
                OperationResultListVo<GameJamViewModel> castResult = serviceResult as OperationResultListVo<GameJamViewModel>;

                model = castResult.Value.ToList();
            }
            else
            {
                model = new List<GameJamViewModel>();
            }

            ViewData["ListDescription"] = SharedLocalizer["All Game Jams"].ToString();

            return PartialView("_ListGameJams", model);
        }

        [Route("/gamejam/listmine")]
        public async Task<PartialViewResult> ListMine()
        {
            List<GameJamViewModel> model;

            OperationResultVo serviceResult = await gameJamAppService.GetByUserId(CurrentUserId, CurrentUserIsAdmin);

            if (serviceResult.Success)
            {
                OperationResultListVo<GameJamViewModel> castResult = serviceResult as OperationResultListVo<GameJamViewModel>;

                model = castResult.Value.ToList();
            }
            else
            {
                model = new List<GameJamViewModel>();
            }

            ViewData["ListDescription"] = SharedLocalizer["All Game Jams"].ToString();

            return PartialView("_ListMyGameJams", model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/jam/{handler?}")]
        [Route("/jam/{id:guid}")]
        public async Task<IActionResult> Details(Guid id, string handler)
        {
            try
            {
                OperationResultVo<GameJamViewModel> serviceResult = await gameJamAppService.GetForDetails(CurrentUserId, CurrentUserIsAdmin, id, handler);

                if (serviceResult.Success)
                {
                    return View(serviceResult.Value);
                }
                else
                {
                    return RedirectToIndex();
                }
            }
            catch
            {
                return RedirectToIndex();
            }
        }

        [Route("/gamejam/add")]
        public async Task<IActionResult> Add()
        {
            OperationResultVo serviceResult = await gameJamAppService.GenerateNew(CurrentUserId);

            if (serviceResult.Success)
            {
                OperationResultVo<GameJamViewModel> castResult = serviceResult as OperationResultVo<GameJamViewModel>;

                GameJamViewModel model = castResult.Value;

                return View("CreateEditWrapper", model);
            }
            else
            {
                return View("CreateEditWrapper", new GameJamViewModel());
            }
        }

        [HttpPost]
        public async Task<IActionResult> ValidateHandler(string handler, Guid Id)
        {
            OperationResultVo result;

            try
            {
                OperationResultVo validate = await gameJamAppService.ValidateHandler(CurrentUserId, handler, Id);

                return Json(validate.Success);
            }
            catch (Exception)
            {
                result = new OperationResultVo(false);
            }

            return Json(result);
        }

        [Route("/gamejam/edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            GameJamViewModel viewModel;

            OperationResultVo<GameJamViewModel> serviceResult = await gameJamAppService.GetForEdit(CurrentUserId, CurrentUserIsAdmin, id);

            if (!serviceResult.Success)
            {
                return RedirectToWithMessage("index", "gamejam", "community", serviceResult.Message);
            }

            viewModel = serviceResult.Value;

            return View("CreateEditWrapper", viewModel);
        }

        [HttpPost("/gamejam/save")]
        public async Task<JsonResult> Save(GameJamViewModel vm)
        {
            bool isNew = vm.Id == Guid.Empty;

            try
            {
                OperationResultVo<Guid> saveResult = await gameJamAppService.Save(CurrentUserId, vm);

                if (saveResult.Success)
                {
                    string url = Url.Action("edit", "gamejam", new { area = "community", id = saveResult.Value, msg = SharedLocalizer[saveResult.Message] });

                    if (isNew && EnvName.Equals(Constants.ProductionEnvironmentName))
                    {
                        await NotificationSender.SendTeamNotificationAsync("New Game Jam created!");
                    }

                    return Json(new OperationResultRedirectVo<Guid>(saveResult, url));
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

        [HttpDelete("/gamejam/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, bool edit)
        {
            try
            {
                OperationResultVo deleteResult = await gameJamAppService.Remove(CurrentUserId, id);

                if (deleteResult.Success)
                {
                    if (edit)
                    {
                        string url = Url.Action("manage", "gamejam", new { area = "community", msg = deleteResult.Message });
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

        private IActionResult RedirectToIndex()
        {
            return RedirectToWithMessage("index", "gamejam", "community", "Unable to get that GameJam!");
        }
    }
}