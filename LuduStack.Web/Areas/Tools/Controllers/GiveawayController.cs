using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Giveaway;
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

            List<SelectListItem> timeZones = ConstantHelper.TimeZones.ToList();

            foreach (SelectListItem timeZone in timeZones)
            {
                timeZone.Selected = !string.IsNullOrWhiteSpace(model.TimeZone) && model.TimeZone.Equals(timeZone.Value);
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


        [Authorize]
        [Route("giveaway/{id:guid}/manage")]
        public IActionResult Manage(Guid id)
        {
            OperationResultVo result = giveawayAppService.GetGiveawayFullById(CurrentUserId, id);

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
        public IActionResult Details(Guid id, string referralCode)
        {

            string sessionEmail = GetSessionValue(SessionValues.Email);

            if (!string.IsNullOrWhiteSpace(sessionEmail))
            {
                return RedirectToAction("youarein", "giveaway", new { area = "tools", id = id });
            }

            OperationResultVo result = giveawayAppService.GetGiveawayBasicInfoById(CurrentUserId, id);

            if (result.Success)
            {
                OperationResultVo<GiveawayViewModel> castRestult = result as OperationResultVo<GiveawayViewModel>;

                string serialized = JsonConvert.SerializeObject(castRestult.Value);
                GiveawayDetailsViewModel model = JsonConvert.DeserializeObject<GiveawayDetailsViewModel>(serialized);

                model.Enter = new GiveawayEnterViewModel
                {
                    GiveawayId = id,
                    ReferralCode = referralCode
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

        [Route("giveaway/{id:guid}/terms")]
        public IActionResult Terms(Guid id)
        {
            OperationResultVo result = giveawayAppService.GetGiveawayBasicInfoById(CurrentUserId, id);

            if (result.Success)
            {
                OperationResultVo<GiveawayViewModel> castResult = result as OperationResultVo<GiveawayViewModel>;

                KeyValuePair<Guid, string> model = new KeyValuePair<Guid, string>(castResult.Value.Id, castResult.Value.TermsAndConditions);

                return View("Terms", model);
            }
            else
            {
                return RedirectToAction("details", "giveaway", new { area = "tools", id = id });
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

                    OperationResultVo resultGiveawayInfo = giveawayAppService.GetGiveawayBasicInfoById(CurrentUserId, enter.GiveawayId);

                    if (resultGiveawayInfo.Success)
                    {
                        OperationResultVo<GiveawayViewModel> castRestultGiveawayInfo = resultGiveawayInfo as OperationResultVo<GiveawayViewModel>;

                        if (!string.IsNullOrWhiteSpace(castRestult.Value))
                        {
                            string emailConfirmationUrl = Url.GiveawayEmailConfirmationLink(Request.Scheme, enter.GiveawayId.ToString(), castRestult.Value);

                            await NotificationSender.SendGiveawayEmailConfirmationAsync(enter.Email, emailConfirmationUrl, castRestultGiveawayInfo.Value.Name);
                        }
                    }

                    string url = Url.Action("youarein", "giveaway", new { area = "tools", id = enter.GiveawayId });

                    await NotificationSender.SendTeamNotificationAsync(String.Format("{0} joined the giveaway {1}", enter.Email, enter.GiveawayId.ToString()));

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

            if (string.IsNullOrWhiteSpace(sessionEmail))
            {
                return RedirectToAction("details", "giveaway", new { area = "tools", id = id });
            }

            ViewData["mailProvider"] = String.Format("https://{0}", sessionEmail.Split("@")[1]);

            OperationResultVo result = giveawayAppService.GetGiveawayParticipantInfo(CurrentUserId, id, sessionEmail);

            if (result.Success)
            {
                OperationResultVo<GiveawayParticipationViewModel> castRestult = result as OperationResultVo<GiveawayParticipationViewModel>;

                GiveawayParticipationViewModel model = castRestult.Value;

                model.ShareUrl = string.Format("{0}{1}", ViewBag.BaseUrl, model.ShareUrl);

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
                    return Json(new OperationResultVo(false));
                }
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
    }
}