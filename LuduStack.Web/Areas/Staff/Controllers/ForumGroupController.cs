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
    [Route("staff/forumgroup")]
    public class ForumGroupController : StaffBaseController
    {
        private readonly IForumGroupAppService forumGroupAppService;

        public ForumGroupController(IForumGroupAppService forumGroupAppService)
        {
            this.forumGroupAppService = forumGroupAppService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("list")]
        public async Task<PartialViewResult> List()
        {
            List<ForumGroupViewModel> model;

            OperationResultVo serviceResult = await forumGroupAppService.GetAll(CurrentUserId);

            if (serviceResult.Success)
            {
                OperationResultListVo<ForumGroupViewModel> castResult = serviceResult as OperationResultListVo<ForumGroupViewModel>;

                model = castResult.Value.ToList();
            }
            else
            {
                model = new List<ForumGroupViewModel>();
            }

            foreach (ForumGroupViewModel item in model)
            {
                SetPermissions(item);
            }

            ViewData["ListDescription"] = SharedLocalizer["All Forum Groups"].ToString();

            return PartialView("_ListForumGroups", model);
        }

        [Route("add")]
        public async Task<IActionResult> Add()
        {
            OperationResultVo serviceResult = await forumGroupAppService.GenerateNew(CurrentUserId);

            if (serviceResult.Success)
            {
                OperationResultVo<ForumGroupViewModel> castResult = serviceResult as OperationResultVo<ForumGroupViewModel>;

                ForumGroupViewModel model = castResult.Value;

                return View("CreateEditWrapper", model);
            }
            else
            {
                return View("CreateEditWrapper", new ForumGroupViewModel());
            }
        }

        [Route("edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            ForumGroupViewModel viewModel;

            OperationResultVo serviceResult = await forumGroupAppService.GetById(CurrentUserId, id);

            OperationResultVo<ForumGroupViewModel> castResult = serviceResult as OperationResultVo<ForumGroupViewModel>;

            viewModel = castResult.Value;

            SetPermissions(viewModel);

            return View("CreateEditWrapper", viewModel);
        }

        [HttpPost("save")]
        public async Task<JsonResult> Save(ForumGroupViewModel vm)
        {
            bool isNew = vm.Id == Guid.Empty;

            try
            {
                vm.UserId = CurrentUserId;

                OperationResultVo<Guid> saveResult = await forumGroupAppService.Save(CurrentUserId, vm);

                if (saveResult.Success)
                {
                    string url = Url.Action("index", "forumgroup", new { area = "staff", msg = SharedLocalizer[saveResult.Message] });

                    if (isNew && EnvName.Equals(Constants.ProductionEnvironmentName))
                    {
                        await NotificationSender.SendTeamNotificationAsync("New Forum Group created!");
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
                OperationResultVo deleteResult = await forumGroupAppService.Remove(CurrentUserId, id);

                if (deleteResult.Success)
                {
                    if (edit)
                    {
                        string url = Url.Action("index", "forumgroup", new { area = "staff", msg = deleteResult.Message });
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

        private void SetPermissions(ForumGroupViewModel model)
        {
            model.Permissions.IsAdmin = CurrentUserIsAdmin;
            model.Permissions.CanDelete = model.Permissions.IsAdmin;
        }
    }
}