using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Giveaway;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Areas.Tools.Controllers.Base;
using LuduStack.Web.Enums;
using LuduStack.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Tools.Controllers
{
    public class GiveawayController : ToolsBaseController
    {
        private readonly IGiveawayAppService giveawayAppService;

        public GiveawayController(IGiveawayAppService giveawayAppService)
        {
            this.giveawayAppService = giveawayAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("tools/giveaway/listbyme")]
        public PartialViewResult ListByMe()
        {
            List<GiveawayListItemVo> model;

            OperationResultVo serviceResult = giveawayAppService.GetGiveawaysByMe(CurrentUserId);

            if (serviceResult.Success)
            {
                OperationResultListVo<GiveawayListItemVo> castResult = serviceResult as OperationResultListVo<GiveawayListItemVo>;

                model = castResult.Value.ToList();
            }
            else
            {
                model = new List<GiveawayListItemVo>();
            }


            foreach (GiveawayListItemVo item in model)
            {
                SetLocalization(item);
            }

            ViewData["ListDescription"] = SharedLocalizer["My Giveaways"].ToString();

            return PartialView("_ListGiveaways", model);
        }

        [Authorize]
        [Route("tools/giveaway/add/")]
        public IActionResult Add()
        {
            OperationResultVo serviceResult = giveawayAppService.GenerateNew(CurrentUserId);

            if (serviceResult.Success)
            {
                OperationResultVo<GiveawayViewModel> castResult = serviceResult as OperationResultVo<GiveawayViewModel>;

                GiveawayViewModel model = castResult.Value;

                SetLocalization(model);

                return View("CreateEditWrapper", model);
            }
            else
            {
                return View("CreateEditWrapper", new GiveawayViewModel());
            }
        }

        [Route("tools/giveaway/edit/{id:guid}")]
        public ViewResult Edit(Guid id)
        {
            GiveawayViewModel model;

            OperationResultVo serviceResult = giveawayAppService.GetGiveawayBasicInfoById(CurrentUserId, id);

            OperationResultVo<GiveawayViewModel> castResult = serviceResult as OperationResultVo<GiveawayViewModel>;

            model = castResult.Value;

            model.Description = ContentFormatter.FormatCFormatTextAreaBreaks(model.Description);

            var timeZones = ConstantHelper.TimeZones.ToList();

            foreach (var timeZone in timeZones)
            {
                timeZone.Selected = model.TimeZone.Equals(timeZone.Value);
            }

            ViewBag.TimeZones = timeZones;

            return View("CreateEditWrapper", model);
        }

        [Route("tools/giveaway/save")]
        public JsonResult SaveGiveaway(GiveawayViewModel vm)
        {
            bool isNew = vm.Id == Guid.Empty;

            try
            {
                vm.UserId = CurrentUserId;

                OperationResultVo<Guid> saveResult = giveawayAppService.SaveGiveaway(CurrentUserId, vm);

                if (saveResult.Success)
                {
                    string url = Url.Action("index", "giveaway", new { area = "tools" });

                    if (isNew)
                    {
                        url = Url.Action("edit", "giveaway", new { area = "tools", id = vm.Id, pointsEarned = saveResult.PointsEarned });

                        NotificationSender.SendTeamNotificationAsync("New Giveaway created!");
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

        [Authorize]
        [HttpDelete("tools/giveaway/{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                OperationResultVo saveResult = giveawayAppService.RemoveGiveaway(CurrentUserId, id);

                if (saveResult.Success)
                {
                    string url = Url.Action("index", "giveaway", new { area = "tools" });

                    return Json(new OperationResultRedirectVo(saveResult, url));
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


        [Route("giveaway/{id:guid}")]
        public IActionResult Details(Guid id)
        {

            var sessionEmail = GetSessionValue(SessionValues.Email);

            if (!string.IsNullOrWhiteSpace(sessionEmail))
            {
                return RedirectToAction("youarein", "giveaway", new { area = "tools", id = id });
            }

            OperationResultVo result = giveawayAppService.GetGiveawayBasicInfoById(CurrentUserId, id);

            if (result.Success)
            {
                OperationResultVo<GiveawayViewModel> castRestult = result as OperationResultVo<GiveawayViewModel>;

                var serialized = JsonConvert.SerializeObject(castRestult.Value);
                var model = JsonConvert.DeserializeObject<GiveawayDetailsViewModel>(serialized);

                model.Enter = new GiveawayEnterViewModel
                {
                    GiveawayId = id
                };

                SetAuthorDetails(model);

                SetLocalization(model);

                return View("Details", model);
            }
            else
            {
                return null;
            }
        }

        [Route("giveaway/{id:guid}/terms")]
        public IActionResult Terms(Guid id)
        {
            OperationResultVo result = giveawayAppService.GetGiveawayBasicInfoById(CurrentUserId, id);

            if (result.Success)
            {
                OperationResultVo<GiveawayViewModel> castResult = result as OperationResultVo<GiveawayViewModel>;

                var model = new KeyValuePair<Guid, string>(castResult.Value.Id, castResult.Value.TermsAndConditions);

                return View("Terms", model);
            }
            else
            {
                return null;
            }
        }


        [HttpPost("tools/giveaway/enter")]
        public IActionResult Enter(GiveawayEnterViewModel enter)
        {
            try
            {
                OperationResultVo result = giveawayAppService.EnterGiveaway(CurrentUserId, enter);

                if (result.Success)
                {
                    SetSessionValue(SessionValues.Email, enter.Email);

                    string url = Url.Action("youarein", "giveaway", new { area = "tools", id = enter.GiveawayId });

                    NotificationSender.SendTeamNotificationAsync(String.Format("{0} joined the giveaway {1}", enter.Email, enter.GiveawayId.ToString()));

                    return Json(new OperationResultRedirectVo(result, url));
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [Route("giveaway/{id:guid}/youarein")]
        public IActionResult YouAreIn(Guid id)
        {
            var sessionEmail = GetSessionValue(SessionValues.Email);

            if (string.IsNullOrWhiteSpace(sessionEmail))
            {
                return RedirectToAction("details", "giveaway", new { area = "tools", id = id });
            }

            ViewData["mailProvider"] = String.Format("https://{0}", sessionEmail.Split("@")[1]);

            OperationResultVo result = giveawayAppService.GetGiveawayParticipantInfo(CurrentUserId, id, sessionEmail);

            if (result.Success)
            {
                OperationResultVo<GiveawayParticipationViewModel> castRestult = result as OperationResultVo<GiveawayParticipationViewModel>;

                var model = castRestult.Value;

                var url = Url.Action("details", "giveaway", new { area = "tools", id = id, referal = model.ShareUrl });

                model.ShareUrl = string.Format("{0}{1}", ViewBag.BaseUrl, url);

                return View("YouAreIn", model);
            }
            else
            {
                return null;
            }
        }



        private void SetLocalization(GiveawayViewModel model)
        {
            SetLocalization(model, false);
        }

        private void SetLocalization(GiveawayListItemVo item)
        {
            SetLocalization(item, false);
        }

        private void SetLocalization(GiveawayViewModel model, bool v)
        {
            if (model != null)
            {
                DisplayAttribute displayStatus = model.Status.GetAttributeOfType<DisplayAttribute>();
                model.StatusLocalized = SharedLocalizer[displayStatus != null ? displayStatus.Name : model.Status.ToString()];
            }
        }

        private void SetLocalization(GiveawayListItemVo item, bool editing)
        {
            if (item != null)
            {
                DisplayAttribute displayStatus = item.Status.GetAttributeOfType<DisplayAttribute>();
                item.StatusLocalized = SharedLocalizer[displayStatus != null ? displayStatus.Name : item.Status.ToString()];
            }
        }
    }
}