﻿using LuduStack.Application;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.GameJam;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Areas.Community.Controllers;
using LuduStack.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Staff.Controllers
{
    public class GameJamController : CommunityBaseController
    {
        private readonly IGameJamAppService gameJamAppService;
        private readonly IGameAppService gameAppService;

        public GameJamController(IGameJamAppService gameJamAppService, IGameAppService gameAppService)
        {
            this.gameJamAppService = gameJamAppService;
            this.gameAppService = gameAppService;
        }

        [Route("/gamejam")]
        public IActionResult Index()
        {

            return View();
        }

        [Route("/gamejam/manage")]
        public IActionResult Manage()
        {

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
                    return Json(deleteResult);
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [HttpPost("/gamejam/{jamId:guid}/join")]
        public async Task<IActionResult> Join(Guid jamId, string handler)
        {
            OperationResultVo result;

            try
            {
                OperationResultVo joinResult = await gameJamAppService.Join(CurrentUserId, jamId);

                if (joinResult.Success)
                {
                    string url = Url.Action("myentry", "gamejam", new { area = "community", jamHandler = handler, msg = joinResult.Message });
                    joinResult.Message = null;

                    return Json(new OperationResultRedirectVo(joinResult, url));
                }
                else
                {
                    return Json(joinResult);
                }
            }
            catch (Exception)
            {
                result = new OperationResultVo(false);
            }

            return Json(result);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("/jam/{jamhandler}/myentry")]
        public async Task<IActionResult> MyEntry(string jamHandler)
        {
            try
            {
                OperationResultVo<GameJamEntryViewModel> serviceResult = await gameJamAppService.GetEntry(CurrentUserId, CurrentUserIsAdmin, jamHandler);

                if (serviceResult.Success)
                {
                    GameJamEntryViewModel model = serviceResult.Value;

                    if (model.Game == null)
                    {
                        model.Title = SharedLocalizer["{0}'s entry", model.AuthorName];

                        IEnumerable<SelectListItemVo> myGames = await gameAppService.GetByUser(CurrentUserId);
                        List<SelectListItem> gamesDropDown = myGames.ToSelectList();
                        ViewBag.UserGames = gamesDropDown;
                    }

                    return View("EntryDetails", model);
                }
                else
                {
                    return RedirectToWithMessage("index", "gamejam", "community", "Entry not found!");
                }
            }
            catch
            {
                return RedirectToWithMessage("index", "gamejam", "community", "Unable to get that Entry!");
            }
        }

        [HttpPost("/gamejam/submitgame")]
        public async Task<IActionResult> SubmitGame(string jamHandler, Guid gameId)
        {
            OperationResultVo result;

            try
            {
                OperationResultVo submitGameResult = await gameJamAppService.SubmitGame(CurrentUserId, jamHandler, gameId);

                if (submitGameResult.Success)
                {
                    string url = Url.Action("myentry", "gamejam", new { area = "community", jamHandler, msg = submitGameResult.Message, pointsEarned = submitGameResult.PointsEarned });
                    submitGameResult.Message = null;

                    return Json(new OperationResultRedirectVo(submitGameResult, url));
                }
                else
                {
                    return Json(submitGameResult);
                }
            }
            catch (Exception)
            {
                result = new OperationResultVo(false);
            }

            return Json(result);
        }

        private IActionResult RedirectToIndex()
        {
            return RedirectToWithMessage("index", "gamejam", "community", "Unable to get that GameJam!");
        }
    }
}