using LuduStack.Application;
using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Forum;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Helpers;
using Markdig;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Community.Controllers
{
    [Route("community/forum")]
    public class ForumController : CommunityBaseController
    {
        private readonly IForumAppService forumAppService;

        public ForumController(IForumAppService forumAppService)
        {
            this.forumAppService = forumAppService;
        }

        public IActionResult Index(string msg)
        {
            if (!string.IsNullOrWhiteSpace(msg))
            {
                TempData["Message"] = SharedLocalizer[msg];
            }

            return View();
        }

        [Route("categories")]
        public async Task<PartialViewResult> ListCategories()
        {
            List<ForumCategoryListItemVo> model;

            OperationResultListVo<ForumCategoryListItemVo> serviceResult = await forumAppService.GetAllCategories(CurrentUserId);

            if (serviceResult.Success)
            {
                model = serviceResult.Value.ToList();
            }
            else
            {
                model = new List<ForumCategoryListItemVo>();
            }

            FillMissingInformation(model);

            return PartialView("_ListCategories", model);
        }

        [Route("{handler?}")]
        [Route("category/{id:guid}")]
        public async Task<IActionResult> Category(Guid id, string handler)
        {
            OperationResultVo<ForumCategoryViewModel> serviceResult = await forumAppService.GetCategory(CurrentUserId, id, handler);

            ForumCategoryViewModel model = serviceResult.Value;

            return View(model);
        }

        [Route("category/{categoryId:guid}/posts")]
        public Task<IActionResult> PostsByCategory(Guid categoryId)
        {
            ViewComponentResult component = ViewComponent("ForumPosts", new { categoryId });

            return Task.FromResult((IActionResult)component);
        }

        [Route("newtopic")]
        public async Task<IActionResult> NewTopic(Guid? categoryId)
        {
            OperationResultVo<ForumPostViewModel> serviceResult = await forumAppService.GenerateNewTopic(CurrentUserId, categoryId);

            return View("CreateEditPostWrapper", serviceResult.Value);
        }

        [HttpPost("savepost")]
        public async Task<JsonResult> SavePost(ForumPostViewModel vm)
        {
            bool isNew = vm.Id == Guid.Empty;

            try
            {
                if (isNew)
                {
                    vm.UserId = CurrentUserId;
                }

                vm.Content = Markdown.Normalize(vm.Content);

                OperationResultVo<Guid> saveResult = await forumAppService.SavePost(CurrentUserId, vm);

                if (saveResult.Success)
                {
                    string url = string.Empty;

                    if (isNew)
                    {
                        url = Url.Action("viewtopic", "forum", new { area = "community", id = saveResult.Value });

                        if (EnvName.Equals(Constants.ProductionEnvironmentName))
                        {
                            await NotificationSender.SendTeamNotificationAsync("New Forum Post created!");
                        }
                    }
                    else
                    {
                        url = Url.Action("viewpost", "forum", new { area = "community", id = vm.Id });
                    }

                    return Json(new OperationResultRedirectVo<Guid>(saveResult, url));
                }
                else
                {
                    return Json(saveResult);
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [Route("topic/{id:guid}")]
        public async Task<IActionResult> ViewTopic(Guid id)
        {
            OperationResultVo<ForumPostViewModel> result = await forumAppService.GetPostForDetails(CurrentUserId, id);

            if (result.Success)
            {
                ForumPostViewModel viewModel = result.Value;

                viewModel.Url = Url.Action("viewtopic", "forum", new { area = "community", id = viewModel.Id }, (string)ViewData["protocol"], (string)ViewData["host"]);

                ViewData["latest"] = false;

                ViewBag.ProfileImage = UrlFormatter.ProfileImage(CurrentUserId, 64);

                return View("ViewTopic", viewModel);
            }
            else
            {
                return RedirectToAction("index", "forum", new { area = "community", msg = SharedLocalizer["Forum Post not found!"] });
            }
        }

        [Route("post/{id:guid}")]
        public async Task<IActionResult> ViewPost(Guid id)
        {
            OperationResultVo<ForumPostViewModel> result = await forumAppService.GetPostForDetails(CurrentUserId, id);

            if (result.Success)
            {
                ForumPostViewModel viewModel = result.Value;

                ViewData["latest"] = false;

                ViewBag.ProfileImage = UrlFormatter.ProfileImage(CurrentUserId, 64);

                return PartialView("_ForumPostViewOnly", viewModel);
            }
            else
            {
                return PartialView("_ForumPostViewOnly", new ForumPostViewModel());
            }
        }

        [Route("topic/{topicId:guid}/answers")]
        public Task<IActionResult> AnswersByTopic(Guid topicId)
        {
            ViewComponentResult component = ViewComponent("ForumTopicAnswers", new { topicId });

            return Task.FromResult((IActionResult)component);
        }

        [Route("topic/{id:guid}/latest")]
        public async Task<IActionResult> ViewTopicLatest(Guid id)
        {
            OperationResultVo<ForumPostViewModel> result = await forumAppService.GetPostForDetails(CurrentUserId, id);

            if (result.Success)
            {
                ForumPostViewModel viewModel = result.Value;

                viewModel.Url = Url.Action("viewtopic", "forum", new { area = "community", id = viewModel.Id }, (string)ViewData["protocol"], (string)ViewData["host"]);

                ViewData["latest"] = true;

                return View("ViewTopic", viewModel);
            }
            else
            {
                return RedirectToAction("index", "forum", new { area = "community", msg = SharedLocalizer["Forum Post not found!"] });
            }
        }

        [HttpDelete("deletepost/{id:guid}")]
        public async Task<IActionResult> DeletePost(Guid id, bool edit)
        {
            try
            {

                OperationResultVo<ForumPostViewModel> deleteResult = await forumAppService.RemovePost(CurrentUserId, id);

                if (deleteResult.Success)
                {
                    if (edit || deleteResult.Value.IsOriginalPost)
                    {
                        deleteResult.Message = null;

                        string url = Url.Action("category", "forum", new { area = "community", handler = deleteResult.Value.CategoryHandler, msg = deleteResult.Message });

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

        [Route("post/{id:guid}/edit")]
        public async Task<IActionResult> PostEdit(Guid id)
        {
            OperationResultVo<ForumPostViewModel> result = await forumAppService.GetPostForEdit(CurrentUserId, id);

            if (result.Success)
            {
                ForumPostViewModel viewModel = result.Value;

                viewModel.IsEditting = true;

                return View("_ForumPostEdit", viewModel);
            }
            else
            {
                return Json(new OperationResultVo("unable to get the forum post"));
            }
        }

        private void FillMissingInformation(List<ForumCategoryListItemVo> model)
        {
            foreach (ForumCategoryListItemVo category in model)
            {
                FillMissingInformation(category);
            }
        }

        private void FillMissingInformation(ForumCategoryListItemVo category)
        {
            if (category.LatestForumPost != null)
            {
                category.LatestForumPost.CreatedRelativeTime = DateTimeHelper.DateTimeToCreatedAgoMessage(category.LatestForumPost.CreateDate, SharedLocalizer);
            }
        }
    }
}
