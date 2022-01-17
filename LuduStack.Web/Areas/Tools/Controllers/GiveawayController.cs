using LuduStack.Application;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Giveaway;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Identity.Services;
using LuduStack.Web.Areas.Tools.Controllers.Base;
using LuduStack.Web.Enums;
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

        [Route("tools/giveaway")]
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

        [Authorize]
        [Route("tools/giveaway/listbyme")]
        public async Task<PartialViewResult> ListByMe()
        {
            List<GiveawayListItemVo> model;

            OperationResultVo serviceResult = await giveawayAppService.GetGiveawaysByMe(CurrentUserId);

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
        public async Task<ViewResult> Edit(Guid id)
        {
            GiveawayViewModel model;

            OperationResultVo serviceResult = await giveawayAppService.GetForEdit(CurrentUserId, id);

            OperationResultVo<GiveawayViewModel> castResult = serviceResult as OperationResultVo<GiveawayViewModel>;

            model = castResult.Value;

            SetTimeZoneDropdown(model);

            return View("CreateEditWrapper", model);
        }

        [Authorize]
        [Route("tools/giveaway/save")]
        public async Task<JsonResult> SaveGiveaway(GiveawayViewModel vm)
        {
            bool isNew = vm.Id == Guid.Empty;

            try
            {
                vm.UserId = CurrentUserId;

                OperationResultVo<Guid> saveResult = await giveawayAppService.SaveGiveaway(CurrentUserId, vm);

                if (!saveResult.Success)
                {
                    return Json(new OperationResultVo(saveResult.Message));
                }
                else
                {
                    string url = Url.Action("edit", "giveaway", new { area = "tools", id = saveResult.Value, pointsEarned = saveResult.PointsEarned, msg = SharedLocalizer[saveResult.Message] });

                    if (isNew && EnvName.Equals(Constants.ProductionEnvironmentName))
                    {
                        await NotificationSender.SendTeamNotificationAsync("New Giveaway created!");
                    }

                    return Json(new OperationResultRedirectVo<Guid>(saveResult, url));
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [Authorize]
        [HttpDelete("tools/giveaway/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, bool edit)
        {
            try
            {
                OperationResultVo deleteResult = await giveawayAppService.DeleteGiveaway(CurrentUserId, id);

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
        public async Task<IActionResult> Duplicate(Guid giveawayId, bool edit)
        {
            try
            {
                OperationResultVo duplicateResult = await giveawayAppService.DuplicateGiveaway(CurrentUserId, giveawayId);

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
        public async Task<IActionResult> Manage(Guid id)
        {
            OperationResultVo result = await giveawayAppService.GetGiveawayForManagement(CurrentUserId, id);

            if (result.Success)
            {
                OperationResultVo<GiveawayViewModel> castRestult = result as OperationResultVo<GiveawayViewModel>;

                GiveawayViewModel model = castRestult.Value;

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
        public async Task<IActionResult> Details(Guid id, string referralCode, string source)
        {
            OperationResultVo result = await giveawayAppService.GetForDetails(CurrentUserId, id);

            if (result.Success)
            {
                OperationResultVo<GiveawayViewModel> castRestult = result as OperationResultVo<GiveawayViewModel>;

                string sessionEmail = GetSessionValue(SessionValues.Email);

                if (!string.IsNullOrWhiteSpace(sessionEmail) && castRestult.Value.Status != GiveawayStatus.Ended)
                {
                    OperationResultVo checkParticipant = await giveawayAppService.CheckParticipant(CurrentUserId, id, sessionEmail);

                    if (checkParticipant.Success)
                    {
                        return RedirectToAction("youarein", "giveaway", new { area = "tools", id });
                    }
                }

                string serialized = JsonConvert.SerializeObject(castRestult.Value);
                GiveawayDetailsViewModel model = JsonConvert.DeserializeObject<GiveawayDetailsViewModel>(serialized);

                model.Enter = new GiveawayEnterViewModel
                {
                    GiveawayId = id,
                    ReferralCode = referralCode,
                    EntryType = Enum.TryParse(source, out GiveawayEntryType entryType) ? entryType : new GiveawayEntryType?()
                };

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

                OperationResultVo result = await giveawayAppService.EnterGiveaway(CurrentUserId, enter, urlReferalBase);

                if (result.Success)
                {
                    OperationResultVo<string> castRestult = result as OperationResultVo<string>;

                    SetSessionValue(SessionValues.Email, enter.Email);

                    OperationResultVo resultGiveawayInfo = await giveawayAppService.GetForEdit(CurrentUserId, enter.GiveawayId);

                    if (resultGiveawayInfo.Success)
                    {
                        OperationResultVo<GiveawayViewModel> castRestultGiveawayInfo = resultGiveawayInfo as OperationResultVo<GiveawayViewModel>;

                        if (!string.IsNullOrWhiteSpace(castRestult.Value))
                        {
                            string emailConfirmationUrl = Url.GiveawayEmailConfirmationLink(Request.Scheme, enter.GiveawayId.ToString(), castRestult.Value);

                            await NotificationSender.SendGiveawayEmailConfirmationAsync(enter.Email, emailConfirmationUrl, castRestultGiveawayInfo.Value.Name);

                            if (EnvName.Equals(Constants.ProductionEnvironmentName))
                            {
                                await NotificationSender.SendTeamNotificationAsync(string.Format("{0} joined the giveaway {1}", enter.Email, castRestultGiveawayInfo.Value.Name));
                            }
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
                return RedirectToAction("youarein", "giveaway", new { area = "tools", id });
            }
            else
            {
                return RedirectToAction("details", "giveaway", new { area = "tools", id });
            }
        }

        [Route("giveaway/{id:guid}/youarein")]
        public async Task<IActionResult> YouAreIn(Guid id)
        {
            string sessionEmail = GetSessionValue(SessionValues.Email);

            OperationResultVo result = await giveawayAppService.GetGiveawayParticipantInfo(CurrentUserId, id, sessionEmail);

            if (result.Success)
            {
                OperationResultVo<GiveawayParticipationViewModel> castRestult = result as OperationResultVo<GiveawayParticipationViewModel>;

                if (string.IsNullOrWhiteSpace(sessionEmail) || castRestult.Value.Status == GiveawayStatus.Ended)
                {
                    return RedirectToAction("details", "giveaway", new { area = "tools", id });
                }

                string[] emailSplit = sessionEmail.Split("@");

                ViewData["mailProvider"] = emailSplit.Length > 1 ? string.Format("https://{0}", emailSplit[1]) : "#";

                GiveawayParticipationViewModel model = castRestult.Value;

                model.ShareUrl = string.Format("{0}{1}", ViewData["BaseUrl"], model.ShareUrl);

                SetEntryOptions(model, id);

                return View("YouAreIn", model);
            }
            else
            {
                return RedirectToAction("details", "giveaway", new { area = "tools", id });
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

        private void SetLocalization(GiveawayViewModel model, bool editing)
        {
            if (model != null && !editing)
            {
                DisplayAttribute displayStatus = model.Status.GetAttributeOfType<DisplayAttribute>();
                model.StatusLocalized = SharedLocalizer[displayStatus != null ? displayStatus.Name : model.Status.ToString()];
            }
        }

        private void SetLocalization(GiveawayListItemVo item, bool editing)
        {
            if (item != null && !editing)
            {
                DisplayAttribute displayStatus = item.Status.GetAttributeOfType<DisplayAttribute>();
                item.StatusLocalized = SharedLocalizer[displayStatus != null ? displayStatus.Name : item.Status.ToString()];
            }
        }

        private void SetTimeZoneDropdown(GiveawayViewModel model)
        {
            List<SelectListItem> timeZones = Constants.TimeZoneSelectList.ToList();

            foreach (SelectListItem timeZone in timeZones)
            {
                timeZone.Selected = (!string.IsNullOrWhiteSpace(model.TimeZone) && model.TimeZone.Equals(timeZone.Value)) || timeZone.Value.Equals("0");
            }

            ViewData["TimeZones"] = timeZones;
        }

        private void SetEntryOptions(GiveawayParticipationViewModel model, Guid giveawayId)
        {
            foreach (GiveawayEntryOptionViewModel item in model.EntryOptions)
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