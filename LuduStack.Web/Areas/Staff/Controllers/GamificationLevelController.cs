using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Gamification;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Identity.Models;
using LuduStack.Web.Areas.Staff.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Staff.Controllers
{
    [Route("staff/gamificationlevel")]
    public class GamificationLevelController : StaffBaseController
    {
        private readonly IGamificationLevelAppService gamificationLevelAppService;

        public GamificationLevelController(IGamificationLevelAppService gamificationLevelAppService)
        {
            this.gamificationLevelAppService = gamificationLevelAppService;
        }

        public IActionResult Index(string msg)
        {
            if (!string.IsNullOrWhiteSpace(msg))
            {
                TempData["Message"] = SharedLocalizer[msg];
            }

            return View();
        }

        [Route("list")]
        public async Task<PartialViewResult> List()
        {
            List<GamificationLevelViewModel> model;

            OperationResultVo serviceResult = gamificationLevelAppService.GetAll(CurrentUserId);

            if (serviceResult.Success)
            {
                OperationResultListVo<GamificationLevelViewModel> castResult = serviceResult as OperationResultListVo<GamificationLevelViewModel>;

                model = castResult.Value.OrderBy(x => x.Number).ToList();
            }
            else
            {
                model = new List<GamificationLevelViewModel>();
            }

            foreach (GamificationLevelViewModel item in model)
            {
                await SetPermissions(item);
            }

            ViewData["ListDescription"] = SharedLocalizer["All Gamification Levels"].ToString();

            return PartialView("_ListGamificationLevels", model);
        }

        [Route("add")]
        public async Task<IActionResult> Add()
        {
            OperationResultVo serviceResult = await gamificationLevelAppService.GenerateNew(CurrentUserId);

            if (serviceResult.Success)
            {
                OperationResultVo<GamificationLevelViewModel> castResult = serviceResult as OperationResultVo<GamificationLevelViewModel>;

                GamificationLevelViewModel model = castResult.Value;
                return View("CreateEditWrapper", model);
            }
            else
            {
                return View("CreateEditWrapper", new GamificationLevelViewModel());
            }
        }

        [Route("edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            GamificationLevelViewModel model;

            OperationResultVo serviceResult = gamificationLevelAppService.GetById(CurrentUserId, id);

            OperationResultVo<GamificationLevelViewModel> castResult = serviceResult as OperationResultVo<GamificationLevelViewModel>;

            model = castResult.Value;

            await SetPermissions(model);

            return View("CreateEditWrapper", model);
        }

        [HttpPost]
        public async Task<IActionResult> ValidateXp(int xpToAchieve, Guid id)
        {
            OperationResultVo result;

            try
            {
                OperationResultVo validate = await gamificationLevelAppService.ValidateXp(CurrentUserId, xpToAchieve, id);

                return Json(validate.Success);
            }
            catch (Exception ex)
            {
                result = new OperationResultVo(false);
            }

            return Json(result);
        }

        [HttpPost("save")]
        public JsonResult Save(GamificationLevelViewModel vm)
        {
            bool isNew = vm.Id == Guid.Empty;

            try
            {
                vm.UserId = CurrentUserId;

                OperationResultVo<Guid> saveResult = gamificationLevelAppService.Save(CurrentUserId, vm);

                if (saveResult.Success)
                {
                    string url = Url.Action("index", "gamificationlevel", new { area = "staff" });

                    if (isNew)
                    {
                        NotificationSender.SendTeamNotificationAsync("New Gamification Level created!");
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

        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id, bool edit)
        {
            try
            {
                OperationResultVo deleteResult = gamificationLevelAppService.Remove(CurrentUserId, id);

                if (deleteResult.Success)
                {
                    if (edit)
                    {
                        string url = Url.Action("index", "gamificationlevel", new { area = "staff", msg = deleteResult.Message });
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

        private async Task SetPermissions(GamificationLevelViewModel model)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(CurrentUserId.ToString());
            bool userIsAdmin = await UserManager.IsInRoleAsync(user, Roles.Administrator.ToString());
            model.Permissions.IsAdmin = userIsAdmin;
            model.Permissions.CanDelete = model.Permissions.IsAdmin;
        }
    }
}
