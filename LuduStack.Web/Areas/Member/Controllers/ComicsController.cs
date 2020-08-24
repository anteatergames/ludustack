using LuduStack.Application.Interfaces;
using LuduStack.Application.Requests.Notification;
using LuduStack.Application.ViewModels.Comics;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Areas.Member.Controllers.Base;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Member.Controllers
{
    [Authorize]
    [Route("comics")]
    public class ComicsController : MemberBaseController
    {
        private readonly IComicsAppService comicsAppService;

        public ComicsController(IComicsAppService comicsAppService)
        {
            this.comicsAppService = comicsAppService;
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

        [AllowAnonymous]
        [Route("{id:guid}")]
        public IActionResult Details(Guid id)
        {
            OperationResultVo result = comicsAppService.GetForDetails(CurrentUserId, id);

            if (result.Success)
            {
                OperationResultVo<ComicStripViewModel> castRestult = result as OperationResultVo<ComicStripViewModel>;

                ComicStripViewModel model = castRestult.Value;

                SetAuthorDetails(model);

                SetLocalization(model);

                return View("Details", model);
            }
            else
            {
                return RedirectToAction("index", "home", new { area = string.Empty });
            }
        }

        [Route("listbyme")]
        public PartialViewResult ListByMe()
        {
            List<ComicsListItemVo> model;

            OperationResultVo serviceResult = comicsAppService.GetComicsByMe(CurrentUserId);

            if (serviceResult.Success)
            {
                OperationResultListVo<ComicsListItemVo> castResult = serviceResult as OperationResultListVo<ComicsListItemVo>;

                model = castResult.Value.ToList();
            }
            else
            {
                model = new List<ComicsListItemVo>();
            }

            foreach (ComicsListItemVo item in model)
            {
                SetLocalization(item);
            }

            ViewData["ListDescription"] = SharedLocalizer["My Comics Strips"].ToString();

            return PartialView("_ListComics", model);
        }

        [Route("add/")]
        public IActionResult Add()
        {
            OperationResultVo serviceResult = comicsAppService.GenerateNew(CurrentUserId);

            if (serviceResult.Success)
            {
                OperationResultVo<ComicStripViewModel> castResult = serviceResult as OperationResultVo<ComicStripViewModel>;

                ComicStripViewModel model = castResult.Value;

                SetLocalization(model);

                return View("CreateEditWrapper", model);
            }
            else
            {
                return View("CreateEditWrapper", new ComicStripViewModel());
            }
        }

        [Route("edit/{id:guid}")]
        public ViewResult Edit(Guid id)
        {
            ComicStripViewModel model;

            OperationResultVo serviceResult = comicsAppService.GetForEdit(CurrentUserId, id);

            OperationResultVo<ComicStripViewModel> castResult = serviceResult as OperationResultVo<ComicStripViewModel>;

            model = castResult.Value;

            SetLocalization(model, true);

            return View("CreateEditWrapper", model);
        }

        [Route("save")]
        public JsonResult Save(ComicStripViewModel vm)
        {
            bool isNew = vm.Id == Guid.Empty;

            try
            {
                vm.UserId = CurrentUserId;

                OperationResultVo<Guid> saveResult = comicsAppService.Save(CurrentUserId, vm);

                if (saveResult.Success)
                {
                    string url = Url.Action("edit", "comics", new { area = "member", id = vm.Id, pointsEarned = saveResult.PointsEarned });

                    if (isNew && EnvName.Equals(ConstantHelper.ProductionEnvironmentName))
                    {
                        NotificationSender.SendTeamNotificationAsync("New Comic Strip created!");
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
                OperationResultVo deleteResult = comicsAppService.Remove(CurrentUserId, id);

                if (deleteResult.Success)
                {
                    if (edit)
                    {
                        string url = Url.Action("index", "comics", new { area = "member", msg = deleteResult.Message });
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

        [HttpPost("rate/{id:guid}")]
        public IActionResult Rate(Guid id, string score)
        {
            try
            {
                decimal scoreDecimal = decimal.Parse(score, CultureInfo.InvariantCulture);

                OperationResultVo serviceResult = comicsAppService.Rate(CurrentUserId, id, scoreDecimal);

                return Json(new OperationResultVo(serviceResult.Success, serviceResult.Message));
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        private void SetLocalization(ComicStripViewModel model)
        {
            SetLocalization(model, false);
        }

        private void SetLocalization(ComicsListItemVo item)
        {
        }

        private void SetLocalization(ComicStripViewModel item, bool editing)
        {
            //if (item != null)
            //{
            //    DisplayAttribute displayStatus = item.Status.GetAttributeOfType<DisplayAttribute>();
            //    item.StatusLocalized = SharedLocalizer[displayStatus != null ? displayStatus.Name : item.Status.ToString()];
            //}
        }
    }
}