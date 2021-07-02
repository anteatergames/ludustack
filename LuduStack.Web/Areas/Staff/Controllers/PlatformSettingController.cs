using LuduStack.Application;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.PlatformSetting;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Areas.Staff.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Staff.Controllers
{
    [Route("staff/platformsetting")]
    public class PlatformSettingController : StaffBaseController
    {
        private readonly IPlatformSettingAppService platformSettingAppService;

        public PlatformSettingController(IPlatformSettingAppService platformSettingAppService)
        {
            this.platformSettingAppService = platformSettingAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("list")]
        public async Task<PartialViewResult> List()
        {
            List<PlatformSettingViewModel> model;

            OperationResultVo serviceResult = await platformSettingAppService.GetAll(CurrentUserId);

            if (serviceResult.Success)
            {
                OperationResultListVo<PlatformSettingViewModel> castResult = serviceResult as OperationResultListVo<PlatformSettingViewModel>;

                model = castResult.Value.ToList();
            }
            else
            {
                model = new List<PlatformSettingViewModel>();
            }

            foreach (PlatformSettingViewModel item in model)
            {
                SetPermissions(item);
            }

            ViewData["ListDescription"] = SharedLocalizer["All Platform Settings"].ToString();

            return PartialView("_ListPlatformSetting", model);
        }

        [Route("edit/{element}")]
        public async Task<IActionResult> Edit(PlatformSettingElement element)
        {
            PlatformSettingViewModel viewModel;

            OperationResultVo serviceResult = await platformSettingAppService.GetForEdit(CurrentUserId, element);

            OperationResultVo<PlatformSettingViewModel> castResult = serviceResult as OperationResultVo<PlatformSettingViewModel>;

            viewModel = castResult.Value;

            return View("CreateEditWrapper", viewModel);
        }

        [HttpPost("save")]
        public async Task<JsonResult> Save(PlatformSettingViewModel vm)
        {
            bool isNew = vm.Id == Guid.Empty;

            try
            {
                vm.UserId = CurrentUserId;

                OperationResultVo<Guid> saveResult = await platformSettingAppService.Save(CurrentUserId, vm);

                if (saveResult.Success)
                {
                    string url = Url.Action("index", "platformsetting", new { area = "staff", msg = SharedLocalizer[saveResult.Message] });

                    if (isNew && EnvName.Equals(Constants.ProductionEnvironmentName))
                    {
                        await NotificationSender.SendTeamNotificationAsync("New Platform Setting created!");
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
        public async Task<IActionResult> Reset(Guid id, bool edit)
        {
            try
            {
                OperationResultVo deleteResult = await platformSettingAppService.Reset(CurrentUserId, id);

                if (deleteResult.Success)
                {
                    if (edit)
                    {
                        string url = Url.Action("index", "platformsetting", new { area = "staff", msg = deleteResult.Message });
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

        private void SetPermissions(PlatformSettingViewModel model)
        {
            model.Permissions.IsAdmin = CurrentUserIsAdmin;
            model.Permissions.CanDelete = model.Permissions.IsAdmin;
        }
    }
}