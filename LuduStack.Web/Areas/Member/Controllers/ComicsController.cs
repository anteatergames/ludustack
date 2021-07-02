using LuduStack.Application;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Comics;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Areas.Member.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Member.Controllers
{
    [Route("comics")]
    public class ComicsController : MemberBaseController
    {
        private readonly IComicsAppService comicsAppService;

        public ComicsController(IComicsAppService comicsAppService)
        {
            this.comicsAppService = comicsAppService;
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

        [AllowAnonymous]
        [Route("{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            OperationResultVo result = await comicsAppService.GetForDetails(CurrentUserId, id);

            if (result.Success)
            {
                OperationResultVo<ComicStripViewModel> castRestult = result as OperationResultVo<ComicStripViewModel>;

                ComicStripViewModel viewModel = castRestult.Value;

                viewModel.Url = Url.Action("details", "comics", new { area = "member", id = viewModel.Id }, (string)ViewData["protocol"], (string)ViewData["host"]);

                SetLocalization(viewModel);

                return View("Details", viewModel);
            }
            else
            {
                return RedirectToAction("index", "home", new { area = string.Empty });
            }
        }

        [Route("listbyme")]
        public async Task<PartialViewResult> ListByMe()
        {
            List<ComicsListItemVo> model;

            OperationResultVo serviceResult = await comicsAppService.GetComicsByMe(CurrentUserId);

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
        public async Task<ViewResult> Edit(Guid id)
        {
            ComicStripViewModel model;

            OperationResultVo serviceResult = await comicsAppService.GetForEdit(CurrentUserId, id);

            OperationResultVo<ComicStripViewModel> castResult = serviceResult as OperationResultVo<ComicStripViewModel>;

            model = castResult.Value;

            SetLocalization(model, true);

            return View("CreateEditWrapper", model);
        }

        [Route("save")]
        public async Task<JsonResult> Save(ComicStripViewModel vm)
        {
            bool isNew = vm.Id == Guid.Empty;

            try
            {
                vm.UserId = CurrentUserId;

                OperationResultVo<Guid> saveResult = await comicsAppService.Save(CurrentUserId, vm);

                if (!saveResult.Success)
                {
                    return Json(new OperationResultVo(false));
                }
                else
                {
                    string url = Url.Action("edit", "comics", new { area = "member", id = saveResult.Value, pointsEarned = saveResult.PointsEarned, msg = SharedLocalizer[saveResult.Message] });

                    if (isNew && EnvName.Equals(Constants.ProductionEnvironmentName))
                    {
                        await NotificationSender.SendTeamNotificationAsync("New Comic Strip created!");
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
                OperationResultVo deleteResult = await comicsAppService.Remove(CurrentUserId, id);

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
        public async Task<IActionResult> Rate(Guid id, string score)
        {
            try
            {
                decimal scoreDecimal = decimal.Parse(score, CultureInfo.InvariantCulture);

                OperationResultVo serviceResult = await comicsAppService.Rate(CurrentUserId, id, scoreDecimal);

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
            // Here goes the localization for each list VO
        }

        private void SetLocalization(ComicStripViewModel item, bool editing)
        {
            // Here goes the localization for each ViewModel
        }
    }
}