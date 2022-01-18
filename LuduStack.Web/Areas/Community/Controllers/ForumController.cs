using LuduStack.Application;
using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Forum;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using LuduStack.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Community.Controllers
{
    public class ForumController : CommunityBaseController
    {
        private readonly IForumAppService forumAppService;

        public ForumController(IForumAppService forumAppService)
        {
            this.forumAppService = forumAppService;
        }

        [Route("community/forum")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("community/forum/categories")]
        public async Task<PartialViewResult> ListCategories()
        {
            List<ForumCategoryListItemVo> model;

            List<SupportedLanguage> userLanguages = await GetCurrentUserContentLanguage();

            GetAllCategoriesRequestViewModel request = new GetAllCategoriesRequestViewModel
            {
                Languages = userLanguages
            };

            OperationResultListVo<ForumCategoryListItemVo> serviceResult = await forumAppService.GetAllCategories(CurrentUserId, request);

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

        [Route("community/forum/categoriesbygroup")]
        public async Task<PartialViewResult> ListCategoriesByGroup()
        {
            List<SupportedLanguage> userLanguages = await GetCurrentUserContentLanguage();

            GetAllCategoriesRequestViewModel request = new GetAllCategoriesRequestViewModel
            {
                Languages = userLanguages
            };

            OperationResultVo<ForumIndexViewModel> serviceResult = await forumAppService.GetAllCategoriesByGroup(CurrentUserId, request);

            if (serviceResult.Success)
            {
                ForumIndexViewModel model = serviceResult.Value;

                foreach (ForumGroupViewModel group in model.Groups)
                {
                    foreach (ForumCategoryListItemVo category in group.Categories)
                    {
                        FillMissingInformation(category);
                    }
                }

                return PartialView("_ListCategoriesByGroup", model);
            }
            else
            {
                OperationResultListVo<ForumCategoryListItemVo> serviceResultCategories = await forumAppService.GetAllCategories(CurrentUserId, request);

                if (serviceResultCategories.Success)
                {
                    IEnumerable<ForumCategoryListItemVo> model = serviceResultCategories.Value;

                    return PartialView("_ListCategories", model);
                }
                else
                {
                    return PartialView("_ListCategories", new List<ForumCategoryListItemVo>());
                }
            }
        }

        [Route("community/forum/{handler?}")]
        [Route("community/forum/category/{id:guid}")]
        public async Task<IActionResult> Category(Guid id, string handler)
        {
            OperationResultVo<ForumCategoryViewModel> serviceResult = await forumAppService.GetCategory(CurrentUserId, id, handler);

            ForumCategoryViewModel model = serviceResult.Value;

            return View(model);
        }

        [Route("community/forum/category/{categoryId:guid}/posts")]
        public Task<IActionResult> PostsByCategory(Guid categoryId, int? page)
        {
            ViewComponentResult component = ViewComponent("ForumPosts", new { categoryId, page });

            return Task.FromResult((IActionResult)component);
        }

        [Route("community/forum/newtopic")]
        public async Task<IActionResult> NewTopic(Guid? categoryId)
        {
            OperationResultVo<ForumPostViewModel> serviceResult = await forumAppService.GenerateNewTopic(CurrentUserId, categoryId);

            serviceResult.Value.Language = base.SetLanguageFromCulture(base.CurrentLocale);

            return View("CreateEditPostWrapper", serviceResult.Value);
        }

        [HttpPost("community/forum/savepost")]
        public async Task<JsonResult> SavePost(ForumPostViewModel vm)
        {
            bool isNew = vm.Id == Guid.Empty;

            try
            {
                if (isNew)
                {
                    vm.UserId = CurrentUserId;
                }

                OperationResultVo<Guid> saveResult = await forumAppService.SavePost(CurrentUserId, vm);

                if (saveResult.Success)
                {
                    string url = string.Empty;

                    if (isNew)
                    {
                        string redirectAction = "viewtopic";

                        if (!vm.IsOriginalPost!)
                        {
                            redirectAction = "viewtopiclatest";
                        }

                        url = Url.Action(redirectAction, "forum", new { area = "community", id = saveResult.Value });

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

        [Route("community/forum/topic/{id:guid}")]
        public async Task<IActionResult> ViewTopic(Guid id)
        {
            OperationResultVo<ForumPostViewModel> result = await forumAppService.GetPostForDetails(CurrentUserId, CurrentUserIsAdmin, id);

            if (result.Success)
            {
                ForumPostViewModel viewModel = result.Value;

                FillMissingInformation(viewModel);

                viewModel.Url = Url.Action("viewtopic", "forum", new { area = "community", id = viewModel.Id }, (string)ViewData["protocol"], (string)ViewData["host"]);

                ViewData["latest"] = false;

                ViewData["ProfileImage"] = UrlFormatter.ProfileImage(CurrentUserId, 64);

                return View("ViewTopic", viewModel);
            }
            else
            {
                return RedirectToAction("index", "forum", new { area = "community", msg = SharedLocalizer["Forum Post not found!"] });
            }
        }

        [Route("community/forum/post/{id:guid}")]
        public async Task<IActionResult> ViewPost(Guid id)
        {
            OperationResultVo<ForumPostViewModel> result = await forumAppService.GetPostForDetails(CurrentUserId, CurrentUserIsAdmin, id);

            if (result.Success)
            {
                ForumPostViewModel viewModel = result.Value;

                ViewData["latest"] = false;

                ViewData["ProfileImage"] = UrlFormatter.ProfileImage(CurrentUserId, 64);

                return PartialView("_ForumPostViewOnly", viewModel);
            }
            else
            {
                return PartialView("_ForumPostViewOnly", new ForumPostViewModel());
            }
        }

        [Route("community/forum/topic/{topicId:guid}/replies")]
        public Task<IActionResult> RepliesByTopic(Guid topicId, int? page, bool latest)
        {
            ViewComponentResult component = ViewComponent("ForumTopicReplies", new { topicId, page, latest });

            return Task.FromResult((IActionResult)component);
        }

        [Route("community/forum/topic/{id:guid}/latest")]
        public async Task<IActionResult> ViewTopicLatest(Guid id)
        {
            OperationResultVo<ForumPostViewModel> result = await forumAppService.GetPostForDetails(CurrentUserId, CurrentUserIsAdmin, id);

            if (result.Success)
            {
                ForumPostViewModel viewModel = result.Value;

                FillMissingInformation(viewModel);

                viewModel.Url = Url.Action("viewtopic", "forum", new { area = "community", id = viewModel.Id }, (string)ViewData["protocol"], (string)ViewData["host"]);

                ViewData["latest"] = true;

                return View("ViewTopic", viewModel);
            }
            else
            {
                return RedirectToAction("index", "forum", new { area = "community", msg = SharedLocalizer["Forum Post not found!"] });
            }
        }

        [HttpDelete("community/forum/deletepost/{id:guid}")]
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

        [Route("community/forum/post/{id:guid}/edit")]
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

        [HttpPost("community/forum/post/vote")]
        public async Task<IActionResult> Vote(Guid postId, VoteValue vote)
        {
            try
            {
                OperationResultVo<int> voteResult = await forumAppService.Vote(CurrentUserId, postId, vote);

                return Json(voteResult);
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        private void FillMissingInformation(ForumPostViewModel viewModel)
        {
            viewModel.CreatedRelativeTime = DateTimeHelper.DateTimeToCreatedAgoMessage(viewModel.CreateDate, SharedLocalizer);
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