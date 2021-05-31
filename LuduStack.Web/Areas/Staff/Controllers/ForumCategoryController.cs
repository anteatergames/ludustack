using LuduStack.Application;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Forum;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Areas.Staff.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Staff.Controllers
{
    [Route("staff/forumcategory")]
    public class ForumCategoryController : StaffBaseController
    {
        private readonly IForumCategoryAppService forumCategoryAppService;

        public ForumCategoryController(IForumCategoryAppService forumCategoryAppService)
        {
            this.forumCategoryAppService = forumCategoryAppService;
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
            List<ForumCategoryViewModel> model;

            OperationResultVo serviceResult = await forumCategoryAppService.GetAll(CurrentUserId);

            if (serviceResult.Success)
            {
                OperationResultListVo<ForumCategoryViewModel> castResult = serviceResult as OperationResultListVo<ForumCategoryViewModel>;

                model = castResult.Value.ToList();
            }
            else
            {
                model = new List<ForumCategoryViewModel>();
            }

            foreach (ForumCategoryViewModel item in model)
            {
                SetPermissions(item);
            }

            ViewData["ListDescription"] = SharedLocalizer["All Forum Categories"].ToString();

            return PartialView("_ListForumCategories", model);
        }

        [Route("add")]
        public async Task<IActionResult> Add()
        {
            OperationResultVo serviceResult = await forumCategoryAppService.GenerateNew(CurrentUserId);

            if (serviceResult.Success)
            {
                OperationResultVo<ForumCategoryViewModel> castResult = serviceResult as OperationResultVo<ForumCategoryViewModel>;

                ForumCategoryViewModel model = castResult.Value;

                return View("CreateEditWrapper", model);
            }
            else
            {
                return View("CreateEditWrapper", new ForumCategoryViewModel());
            }
        }

        [Route("edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            ForumCategoryViewModel viewModel;

            OperationResultVo serviceResult = await forumCategoryAppService.GetById(CurrentUserId, id);

            OperationResultVo<ForumCategoryViewModel> castResult = serviceResult as OperationResultVo<ForumCategoryViewModel>;

            viewModel = castResult.Value;

            SetPermissions(viewModel);

            return View("CreateEditWrapper", viewModel);
        }

        [HttpPost("save")]
        public async Task<JsonResult> Save(ForumCategoryViewModel vm)
        {
            bool isNew = vm.Id == Guid.Empty;

            try
            {
                vm.UserId = CurrentUserId;

                OperationResultVo<Guid> saveResult = await forumCategoryAppService.Save(CurrentUserId, vm);

                if (saveResult.Success)
                {
                    string url = Url.Action("index", "forumcategory", new { area = "staff", msg = SharedLocalizer[saveResult.Message] });

                    if (isNew && EnvName.Equals(Constants.ProductionEnvironmentName))
                    {
                        await NotificationSender.SendTeamNotificationAsync("New Forum Category created!");
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
        public async Task<IActionResult> Delete(Guid id, bool edit)
        {
            try
            {
                OperationResultVo deleteResult = await forumCategoryAppService.Remove(CurrentUserId, id);

                if (deleteResult.Success)
                {
                    if (edit)
                    {
                        string url = Url.Action("index", "forumcategory", new { area = "staff", msg = deleteResult.Message });
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

        private void SetPermissions(ForumCategoryViewModel model)
        {
            model.Permissions.IsAdmin = CurrentUserIsAdmin;
            model.Permissions.CanDelete = model.Permissions.IsAdmin;
        }
    }
}