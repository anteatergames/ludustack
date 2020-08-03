using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Giveaway;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Identity.Services;
using LuduStack.Web.Areas.Tools.Controllers.Base;
using LuduStack.Web.Enums;
using LuduStack.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
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

        public IActionResult Index(string msg)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrWhiteSpace(msg))
                {
                    TempData["Message"] = SharedLocalizer[msg];
                }
                return View("Dashboard");
            }
            else
            {
                return View();
            }
        }

        [Authorize]
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

                SetTimeZoneDropdown(model);

                SetLocalization(model);

                return View("CreateEditWrapper", model);
            }
            else
            {
                return View("CreateEditWrapper", new GiveawayViewModel());
            }
        }

        [Authorize]
        [Route("tools/giveaway/edit/{id:guid}")]
        public ViewResult Edit(Guid id)
        {
            GiveawayViewModel model;

            OperationResultVo serviceResult = giveawayAppService.GetForEdit(CurrentUserId, id);

            OperationResultVo<GiveawayViewModel> castResult = serviceResult as OperationResultVo<GiveawayViewModel>;

            model = castResult.Value;

            model.Description = ContentFormatter.FormatCFormatTextAreaBreaks(model.Description);

            SetTimeZoneDropdown(model);

            return View("CreateEditWrapper", model);
        }

        [Authorize]
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
                    string url = Url.Action("edit", "giveaway", new { area = "tools", id = vm.Id, pointsEarned = saveResult.PointsEarned });

                    if (isNew)
                    {
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
        public IActionResult Delete(Guid id, bool edit)
        {
            try
            {
                OperationResultVo deleteResult = giveawayAppService.DeleteGiveaway(CurrentUserId, id);

                if (deleteResult.Success)
                {
                    if (edit)
                    {
                        string url = Url.Action("index", "giveaway", new { area = "tools", msg = deleteResult.Message });
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

        [Authorize]
        [HttpPost("tools/giveaway/{giveawayId:guid}/duplicate")]
        public IActionResult Duplicate(Guid giveawayId, bool edit)
        {
            try
            {
                OperationResultVo duplicateResult = giveawayAppService.DuplicateGiveaway(CurrentUserId, giveawayId);

                if (duplicateResult.Success)
                {
                    if (edit)
                    {
                        OperationResultVo<Guid> castRestult = duplicateResult as OperationResultVo<Guid>;

                        string url = Url.Action("edit", "giveaway", new { area = "tools", id = castRestult.Value });

                        return Json(new OperationResultRedirectVo(castRestult, url));
                    }

                    return Json(duplicateResult);
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
        [Route("giveaway/{id:guid}/manage")]
        public IActionResult Manage(Guid id)
        {
            OperationResultVo result = giveawayAppService.GetGiveawayForManagement(CurrentUserId, id);

            if (result.Success)
            {
                OperationResultVo<GiveawayViewModel> castRestult = result as OperationResultVo<GiveawayViewModel>;

                var model = castRestult.Value;

                SetAuthorDetails(model);

                SetLocalization(model);

                ViewData["giveawayId"] = model.Id;

                return View("Manage", model);
            }
            else
            {
                return RedirectToAction("index", "giveaway", new { area = "tools" });
            }
        }

        [Route("giveaway/{id:guid}")]
        public IActionResult Details(Guid id, string referralCode, string source)
        {
            OperationResultVo result = giveawayAppService.GetForDetails(CurrentUserId, id);

            if (result.Success)
            {
                OperationResultVo<GiveawayViewModel> castRestult = result as OperationResultVo<GiveawayViewModel>;

                string sessionEmail = GetSessionValue(SessionValues.Email);

                if (!string.IsNullOrWhiteSpace(sessionEmail) && castRestult.Value.Status != GiveawayStatus.Ended)
                {
                    var checkParticipant = giveawayAppService.CheckParticipant(CurrentUserId, id, sessionEmail);

                    if (checkParticipant.Success)
                    {
                        return RedirectToAction("youarein", "giveaway", new { area = "tools", id = id });
                    }
                }

                string serialized = JsonConvert.SerializeObject(castRestult.Value);
                GiveawayDetailsViewModel model = JsonConvert.DeserializeObject<GiveawayDetailsViewModel>(serialized);

                GiveawayEntryType entryType;
                model.Enter = new GiveawayEnterViewModel
                {
                    GiveawayId = id,
                    ReferralCode = referralCode,
                    EntryType = Enum.TryParse(source, out entryType) ? entryType : new GiveawayEntryType?()
                };

                SetAuthorDetails(model);

                SetLocalization(model);

                return View("Details", model);
            }
            else
            {
                return RedirectToAction("index", "giveaway", new { area = "tools" });
            }
        }

        [HttpPost("tools/giveaway/enter")]
        public async Task<IActionResult> Enter(GiveawayEnterViewModel enter)
        {
            try
            {
                string urlReferalBase = Url.Action("details", "giveaway", new { area = "tools", id = enter.GiveawayId });

                OperationResultVo result = giveawayAppService.EnterGiveaway(CurrentUserId, enter, urlReferalBase);

                if (result.Success)
                {
                    OperationResultVo<string> castRestult = result as OperationResultVo<string>;

                    SetSessionValue(SessionValues.Email, enter.Email);

                    OperationResultVo resultGiveawayInfo = giveawayAppService.GetForEdit(CurrentUserId, enter.GiveawayId);

                    if (resultGiveawayInfo.Success)
                    {
                        OperationResultVo<GiveawayViewModel> castRestultGiveawayInfo = resultGiveawayInfo as OperationResultVo<GiveawayViewModel>;

                        if (!string.IsNullOrWhiteSpace(castRestult.Value))
                        {
                            string emailConfirmationUrl = Url.GiveawayEmailConfirmationLink(Request.Scheme, enter.GiveawayId.ToString(), castRestult.Value);

                            await NotificationSender.SendGiveawayEmailConfirmationAsync(enter.Email, emailConfirmationUrl, castRestultGiveawayInfo.Value.Name);

                            await NotificationSender.SendTeamNotificationAsync(String.Format("{0} joined the giveaway {1}", enter.Email, castRestultGiveawayInfo.Value.Name));
                        }
                    }

                    string url = Url.Action("youarein", "giveaway", new { area = "tools", id = enter.GiveawayId });

                    return Json(new OperationResultRedirectVo(result, url));
                }

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [Route("giveaway/{id:guid}/emailconfirmation/{referralCode}")]
        public IActionResult EmailConfirmation(Guid id, string referralCode)
        {
            OperationResultVo result = giveawayAppService.ConfirmParticipant(CurrentUserId, id, referralCode);

            if (result.Success)
            {
                return RedirectToAction("youarein", "giveaway", new { area = "tools", id = id });
            }
            else
            {
                return RedirectToAction("details", "giveaway", new { area = "tools", id = id });
            }
        }

        [Route("giveaway/{id:guid}/youarein")]
        public IActionResult YouAreIn(Guid id)
        {
            string sessionEmail = GetSessionValue(SessionValues.Email);

            OperationResultVo result = giveawayAppService.GetGiveawayParticipantInfo(CurrentUserId, id, sessionEmail);

            if (result.Success)
            {
                OperationResultVo<GiveawayParticipationViewModel> castRestult = result as OperationResultVo<GiveawayParticipationViewModel>;

                if (string.IsNullOrWhiteSpace(sessionEmail) || castRestult.Value.Status == GiveawayStatus.Ended)
                {
                    return RedirectToAction("details", "giveaway", new { area = "tools", id = id });
                }

                var emailSplit = sessionEmail.Split("@");

                ViewData["mailProvider"] = emailSplit.Length > 1 ? String.Format("https://{0}", emailSplit[1]) : "#";

                GiveawayParticipationViewModel model = castRestult.Value;

                model.ShareUrl = string.Format("{0}{1}", ViewBag.BaseUrl, model.ShareUrl);

                SetEntryOptions(model, id);

                return View("YouAreIn", model);
            }
            else
            {
                return RedirectToAction("details", "giveaway", new { area = "tools", id = id });
            }
        }

        [Authorize]
        [HttpDelete("tools/giveaway/{giveawayId:guid}/deleteparticipant/{participantId:guid}")]
        public IActionResult DeleteParticipant(Guid giveawayId, Guid participantId)
        {
            try
            {
                OperationResultVo saveResult = giveawayAppService.RemoveParticipant(CurrentUserId, giveawayId, participantId);

                if (saveResult.Success)
                {
                    string url = Url.Action("manage", "giveaway", new { area = "tools", id = giveawayId });

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

        [Authorize]
        [HttpDelete("tools/giveaway/{giveawayId:guid}/clearparticipants")]
        public IActionResult ClearParticipants(Guid giveawayId)
        {
            try
            {
                OperationResultVo saveResult = giveawayAppService.ClearParticipants(CurrentUserId, giveawayId);

                if (saveResult.Success)
                {
                    string url = Url.Action("manage", "giveaway", new { area = "tools", id = giveawayId });

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

        [Authorize]
        [HttpPost("tools/giveaway/{giveawayId:guid}/picksinglewinner")]
        public IActionResult PickSingleWinner(Guid giveawayId)
        {
            try
            {
                OperationResultVo saveResult = giveawayAppService.PickSingleWinner(CurrentUserId, giveawayId);

                if (saveResult.Success)
                {
                    string url = Url.Action("manage", "giveaway", new { area = "tools", id = giveawayId });

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

        [Authorize]
        [HttpPost("tools/giveaway/{giveawayId:guid}/pickallwinners")]
        public IActionResult PickAllWinners(Guid giveawayId)
        {
            try
            {
                OperationResultVo saveResult = giveawayAppService.PickAllWinners(CurrentUserId, giveawayId);

                if (saveResult.Success)
                {
                    string url = Url.Action("manage", "giveaway", new { area = "tools", id = giveawayId });

                    return Json(new OperationResultRedirectVo(saveResult, url));
                }
                else
                {
                    return Json(new OperationResultVo(false, "Unable to pick winners."));
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [Authorize]
        [HttpPost("tools/giveaway/{giveawayId:guid}/declarenotwinner/{participantId:guid}")]
        public IActionResult DeclareNotWinner(Guid giveawayId, Guid participantId)
        {
            try
            {
                OperationResultVo saveResult = giveawayAppService.DeclareNotWinner(CurrentUserId, giveawayId, participantId);

                if (saveResult.Success)
                {
                    string url = Url.Action("manage", "giveaway", new { area = "tools", id = giveawayId });

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

        [HttpPost("tools/giveaway/{giveawayId:guid}/dailyentry/{participantId:guid}")]
        public IActionResult DailyEntry(Guid giveawayId, Guid participantId)
        {
            try
            {
                OperationResultVo saveResult = giveawayAppService.DailyEntry(CurrentUserId, giveawayId, participantId);

                return Json(saveResult);
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
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

        private void SetTimeZoneDropdown(GiveawayViewModel model)
        {
            List<SelectListItem> timeZones = ConstantHelper.TimeZones.ToList();

            foreach (SelectListItem timeZone in timeZones)
            {
                timeZone.Selected = (!string.IsNullOrWhiteSpace(model.TimeZone) && model.TimeZone.Equals(timeZone.Value)) || timeZone.Value.Equals("0");
            }

            ViewBag.TimeZones = timeZones;
        }

        private void SetEntryOptions(GiveawayParticipationViewModel model, Guid giveawayId)
        {
            foreach (var item in model.EntryOptions)
            {
                switch (item.Type)
                {
                    case GiveawayEntryType.Daily:
                        item.Url = Url.Action("dailyentry", "giveaway", new { area = "tools", giveawayId = giveawayId, participantId = model.ParticipantId });
                        break;

                    default:
                        break;
                }
            }
        }
    }
}