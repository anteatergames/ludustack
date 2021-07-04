using LuduStack.Application;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.BillRate;
using LuduStack.Domain.Core.Attributes;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Areas.Member.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Member.Controllers
{
    [Route("member/billrate")]
    public class BillRateController : MemberBaseController
    {
        private readonly IBillRateAppService billRateAppService;

        public BillRateController(IBillRateAppService billRateAppService)
        {
            this.billRateAppService = billRateAppService;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View("Dashboard");
            }
            else
            {
                return View();
            }
        }

        [Route("listbyme")]
        public async Task<PartialViewResult> ListByMe()
        {
            List<BillRateViewModel> model;

            OperationResultVo serviceResult = await billRateAppService.GetByMe(CurrentUserId);

            if (serviceResult.Success)
            {
                OperationResultListVo<BillRateViewModel> castResult = serviceResult as OperationResultListVo<BillRateViewModel>;

                model = castResult.Value.ToList();
            }
            else
            {
                model = new List<BillRateViewModel>();
            }

            ViewData["ListDescription"] = SharedLocalizer["My Bill Rates"].ToString();

            SetIcons(model);

            return PartialView("_ListBillRates", model);
        }

        [Route("add")]
        public IActionResult Add()
        {
            OperationResultVo serviceResult = billRateAppService.GenerateNew(CurrentUserId);

            SetGameElements();

            if (serviceResult.Success)
            {
                OperationResultVo<BillRateViewModel> castResult = serviceResult as OperationResultVo<BillRateViewModel>;

                BillRateViewModel model = castResult.Value;

                return View("CreateEditWrapper", model);
            }
            else
            {
                return View("CreateEditWrapper", new BillRateViewModel());
            }
        }

        [Route("edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            BillRateViewModel viewModel;

            OperationResultVo serviceResult = await billRateAppService.GetForEdit(CurrentUserId, id);

            OperationResultVo<BillRateViewModel> castResult = serviceResult as OperationResultVo<BillRateViewModel>;

            viewModel = castResult.Value;

            if (!CurrentUserIsAdmin && viewModel.UserId != CurrentUserId)
            {
                return RedirectToAction("index", "billrate", new { area = "member", id, msg = SharedLocalizer["You cannot edit someone else's Bill Rate!"] });
            }

            SetGameElements();

            return View("CreateEditWrapper", viewModel);
        }

        [Route("save")]
        public async Task<JsonResult> Save(BillRateViewModel vm)
        {
            bool isNew = vm.Id == Guid.Empty;

            try
            {
                vm.UserId = CurrentUserId;

                OperationResultVo<Guid> saveResult = await billRateAppService.Save(CurrentUserId, vm);

                if (!saveResult.Success)
                {
                    return Json(saveResult);
                }
                else
                {
                    string url = Url.Action("edit", "billrate", new { area = "member", id = saveResult.Value, pointsEarned = saveResult.PointsEarned, msg = saveResult.Message });

                    if (isNew && EnvName.Equals(Constants.ProductionEnvironmentName))
                    {
                        await NotificationSender.SendTeamNotificationAsync("New Bill Rate created!");
                    }

                    return Json(new OperationResultRedirectVo<Guid>(saveResult, url));
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
                OperationResultVo deleteResult = await billRateAppService.Remove(CurrentUserId, id);

                if (deleteResult.Success)
                {
                    if (edit)
                    {
                        string url = Url.Action("index", "billrate", new { area = "member", msg = deleteResult.Message });
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

        private void SetGameElements()
        {
            Dictionary<GameElement, UiInfoAttribute> gameElements = Enum.GetValues(typeof(GameElement)).Cast<GameElement>().ToUiInfoDictionary();
            ViewData["GameElements"] = gameElements;
        }

        private void SetIcons(List<BillRateViewModel> rates)
        {
            foreach (BillRateViewModel item in rates)
            {
                switch (item.BillRateType)
                {
                    case BillRateType.Visual:
                        item.Icon = "text-success fa-2x fas fa-eye";
                        break;

                    case BillRateType.Audio:
                        item.Icon = "text-warning fa-2x fas fa-music";
                        break;

                    case BillRateType.Code:
                        item.Icon = "text-danger fa-2x fas fa-code";
                        break;

                    case BillRateType.Text:
                        item.Icon = "text-dark fa-2x fas fa-paragraph";
                        break;

                    default:
                        item.Icon = "text-muted fa-2x fas fa-question";
                        break;
                }
            }
        }
    }
}