using LuduStack.Application;
using LuduStack.Application.Formatters;
using LuduStack.Application.Helpers;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels;
using LuduStack.Application.ViewModels.Content;
using LuduStack.Application.ViewModels.Game;
using LuduStack.Application.ViewModels.Poll;
using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Identity.Models;
using LuduStack.Web.Controllers.Base;
using LuduStack.Web.Enums;
using LuduStack.Web.Extensions;
using LuduStack.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace LuduStack.Web.Controllers
{
    [Authorize]
    public class ContentController : SecureBaseController
    {
        private readonly IUserContentAppService userContentAppService;
        private readonly IGameAppService gameAppService;
        private readonly INotificationAppService notificationAppService;

        public ContentController(IUserContentAppService userContentAppService, IGameAppService gameAppService, INotificationAppService notificationAppService)
        {
            this.userContentAppService = userContentAppService;
            this.gameAppService = gameAppService;
            this.notificationAppService = notificationAppService;
        }

        [AllowAnonymous]
        public Task<IActionResult> Feed(Guid? gameId, Guid? userId, Guid? singleContentId, Guid? oldestId, DateTime? oldestDate, bool? articlesOnly, int? count)
        {
            ViewComponentResult component = ViewComponent("Feed", new { count, gameId, userId, singleContentId, oldestId, oldestDate, articlesOnly });

            return Task.FromResult((IActionResult)component);
        }

        [AllowAnonymous]
        [HttpGet("content/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            OperationResultVo<UserContentViewModel> serviceResult = await userContentAppService.GetById(CurrentUserId, id);

            if (!serviceResult.Success)
            {
                return RedirectToAction("index", "home", new { area = string.Empty, msg = SharedLocalizer["Content not found!"] });
            }

            UserContentViewModel viewModel = serviceResult.Value;

            viewModel.Url = Url.Action("details", "content", new { area = string.Empty, id = viewModel.Id }, (string)ViewData["protocol"], (string)ViewData["host"]);

            viewModel.Content = ContentFormatter.FormatContentToShow(viewModel.Content);

            if (viewModel.GameId.HasValue && viewModel.GameId.Value != Guid.Empty)
            {
                OperationResultVo<GameViewModel> gameServiceResult = await gameAppService.GetById(CurrentUserId, viewModel.GameId.Value);

                GameViewModel game = gameServiceResult.Value;

                if (game != null)
                {
                    viewModel.GameTitle = game.Title;
                    viewModel.GameThumbnail = UrlFormatter.Image(game.UserId, ImageType.GameThumbnail, game.ThumbnailUrl);
                }
            }

            viewModel.Content = viewModel.Content.Replace("image-style-align-right", "image-style-align-right float-right p-10");
            viewModel.Content = viewModel.Content.Replace("image-style-align-left", "image-style-align-left float-left p-10");
            viewModel.Content = viewModel.Content.Replace("<img src=", @"<img class=""img-fluid"" src=");

            if (string.IsNullOrEmpty(viewModel.Title))
            {
                viewModel.Title = SharedLocalizer["Content posted on"] + " " + viewModel.CreateDate.ToString();
            }

            if (string.IsNullOrWhiteSpace(viewModel.Introduction))
            {
                viewModel.Introduction = SharedLocalizer["Content posted on"] + " " + viewModel.CreateDate.ToShortDateString();
            }

            ApplicationUser user = await UserManager.FindByIdAsync(CurrentUserId.ToString());

            bool userIsAdmin = user != null && await UserManager.IsInRoleAsync(user, Roles.Administrator.ToString());

            viewModel.Permissions.CanEdit = viewModel.UserId == CurrentUserId || userIsAdmin;
            viewModel.Permissions.CanDelete = viewModel.UserId == CurrentUserId || userIsAdmin;

            ViewData["IsDetails"] = true;

            SetNanoGallery(viewModel);

            return View(viewModel);
        }

        public async Task<IActionResult> Add(Guid? gameId)
        {
            UserContentViewModel vm = new UserContentViewModel
            {
                UserId = CurrentUserId,
                FeaturedImage = Constants.DefaultFeaturedImage
            };

            IEnumerable<SelectListItemVo> games = await gameAppService.GetByUser(vm.UserId);
            List<SelectListItem> gamesDropDown = games.ToSelectList();
            ViewData["UserGames"] = gamesDropDown;

            if (gameId.HasValue)
            {
                vm.GameId = gameId;
            }

            return View("CreateEdit", vm);
        }

        [HttpGet("content/edit/{id:guid}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            OperationResultVo<UserContentViewModel> serviceResult = await userContentAppService.GetById(CurrentUserId, id);

            UserContentViewModel viewModel = serviceResult.Value;

            if (!CurrentUserIsAdmin && viewModel.UserId != CurrentUserId)
            {
                return RedirectToAction("details", "content", new { id, msg = SharedLocalizer["You cannot edit someone else's content!"] });
            }

            IEnumerable<SelectListItemVo> games = await gameAppService.GetByUser(viewModel.UserId);
            List<SelectListItem> gamesDropDown = games.ToSelectList();
            ViewData["UserGames"] = gamesDropDown;

            if (!viewModel.HasFeaturedImage)
            {
                viewModel.FeaturedImage = Constants.DefaultFeaturedImage;
            }

            return View("CreateEdit", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Save(UserContentViewModel viewModel)
        {
            if (!CurrentUserIsAdmin && viewModel.UserId != CurrentUserId)
            {
                return RedirectToAction("details", "content", new { viewModel.Id, msg = SharedLocalizer["You cannot edit someone else's content!"] });
            }

            try
            {
                bool isNew = viewModel.Id == Guid.Empty;
                OperationResultVo<Guid> saveResult = await userContentAppService.Save(CurrentUserId, viewModel);

                if (!saveResult.Success)
                {
                    return Json(saveResult);
                }
                else
                {
                    ProfileViewModel profile = await ProfileAppService.GetByUserId(CurrentUserId, ProfileType.Personal);

                    await NotifyFollowers(profile, viewModel.GameId);

                    string url = Url.Action("details", "content", new { area = string.Empty, id = saveResult.Value, pointsEarned = saveResult.PointsEarned, msg = SharedLocalizer[saveResult.Message] });

                    if (isNew && EnvName.Equals(Constants.ProductionEnvironmentName))
                    {
                        await NotificationSender.SendTeamNotificationAsync("New complex post!");
                    }

                    return Json(new OperationResultRedirectVo(url));
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        [HttpDelete("/content/{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            OperationResultVo result = await userContentAppService.Remove(CurrentUserId, id);

            if (result.Success)
            {
                result.Message = SharedLocalizer["Content deleted successfully!"];
            }
            else
            {
                result.Message = SharedLocalizer["Oops! The content was not deleted!"];
            }

            return Json(result);
        }

        [HttpPost("content/post")]
        public async Task<IActionResult> SimplePost(string text, string images, IEnumerable<PollOptionViewModel> pollOptions, SupportedLanguage? language, Guid? gameId)
        {
            UserContentViewModel vm = new UserContentViewModel
            {
                Language = language ?? SupportedLanguage.English,
                Content = text,
                Poll = new PollViewModel
                {
                    PollOptions = pollOptions.ToList()
                },
                GameId = gameId
            };

            SplitImagesFieldToSave(vm, images);

            OperationResultVo<Guid> saveResult = await userContentAppService.Save(CurrentUserId, vm);

            if (!saveResult.Success)
            {
                return Json(saveResult);
            }
            else
            {
                ProfileViewModel profile = await ProfileAppService.GetByUserId(CurrentUserId, ProfileType.Personal);

                await NotifyFollowers(profile, vm.GameId);

                if (EnvName.Equals(Constants.ProductionEnvironmentName))
                {
                    await NotificationSender.SendTeamNotificationAsync("New simple post!");
                }

                return Json(saveResult);
            }
        }

        #region Content interactions

        [HttpPost("content/like")]
        public async Task<IActionResult> LikeContent(Guid targetId, UserContentType contentType)
        {
            OperationResultVo response = await userContentAppService.ContentLike(CurrentUserId, targetId);

            OperationResultVo<UserContentViewModel> content = await userContentAppService.GetById(CurrentUserId, targetId);

            string myName = GetSessionValue(SessionValues.FullName);

            if (contentType == UserContentType.ComicStrip)
            {
                await notificationAppService.Notify(CurrentUserId, myName, content.Value.UserId, NotificationType.ComicsLike, targetId);
            }
            else
            {
                await notificationAppService.Notify(CurrentUserId, myName, content.Value.UserId, NotificationType.ContentLike, targetId);
            }

            return Json(response);
        }

        [HttpPost("content/unlike")]
        public async Task<IActionResult> UnLikeContent(Guid targetId)
        {
            OperationResultVo response = await userContentAppService.ContentUnlike(CurrentUserId, targetId);

            return Json(response);
        }

        [HttpPost("content/comment")]
        public async Task<IActionResult> Comment(CommentViewModel vm)
        {
            OperationResultVo response;

            response = await userContentAppService.Comment(CurrentUserId, vm);

            return Json(response);
        }

        #endregion Content interactions

        private void SplitImagesFieldToSave(UserContentViewModel vm, string images)
        {
            if (!string.IsNullOrWhiteSpace(images))
            {
                string[] imgSplit = images.Split('|');
                vm.Media = new List<MediaListItemVo>();

                for (int i = 0; i < imgSplit.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(imgSplit[i]))
                    {
                        if (string.IsNullOrWhiteSpace(vm.FeaturedImage))
                        {
                            vm.FeaturedImage = imgSplit[i];
                        }

                        MediaType type = ContentHelper.GetMediaType(imgSplit[i]);

                        vm.Media.Add(new MediaListItemVo
                        {
                            Type = type,
                            Url = imgSplit[i]
                        });
                    }
                }
            }
        }

        private async Task NotifyFollowers(ProfileViewModel profile, Guid? gameId)
        {
            Dictionary<Guid, FollowType> followersToNotify = new Dictionary<Guid, FollowType>();

            string gameName = string.Empty;

            Guid targetId = Guid.Empty;

            if (profile.Followers != null)
            {
                foreach (UserFollowViewModel userFollower in profile.Followers)
                {
                    followersToNotify.Add(userFollower.UserId, FollowType.Content);
                }
            }

            gameName = await ProcessGamePost(gameId, followersToNotify, gameName);

            Notify(profile, followersToNotify, gameName, targetId);
        }

        private async Task<string> ProcessGamePost(Guid? gameId, Dictionary<Guid, FollowType> userFollowers, string gameName)
        {
            if (gameId.HasValue)
            {
                OperationResultVo<GameViewModel> gameResult = await gameAppService.GetById(CurrentUserId, gameId.Value);

                if (gameResult.Success)
                {
                    gameName = gameResult.Value.Title;
                    foreach (GameFollowViewModel gameFollower in gameResult.Value.Followers)
                    {
                        if (userFollowers.ContainsKey(gameFollower.UserId))
                        {
                            userFollowers[gameFollower.UserId] = FollowType.Game;
                        }
                        else
                        {
                            userFollowers.Add(gameFollower.UserId, FollowType.Game);
                        }
                    }
                }
            }

            return gameName;
        }

        private void Notify(ProfileViewModel profile, Dictionary<Guid, FollowType> followers, string gameName, Guid targetId)
        {
            foreach (KeyValuePair<Guid, FollowType> follower in followers)
            {
                switch (follower.Value)
                {
                    case FollowType.Game:
                        notificationAppService.Notify(CurrentUserId, gameName, follower.Key, NotificationType.ContentPosted, targetId);
                        break;

                    default:
                        notificationAppService.Notify(CurrentUserId, profile.Name, follower.Key, NotificationType.ContentPosted, targetId);
                        break;
                }
            }
        }

        private void SetNanoGallery(UserContentViewModel vm)
        {
            List<NanoGalleryViewModel> gallery = new List<NanoGalleryViewModel>();

            IEnumerable<MediaListItemVo> galleryItems = vm.Media.Where(x => x.Type == MediaType.Image || x.Type == MediaType.Youtube || x.Type == MediaType.Dailymotion); // need to add Vimeo with thumbnail

            foreach (MediaListItemVo mediaItem in galleryItems)
            {
                NanoGalleryViewModel item = new NanoGalleryViewModel
                {
                    Src = mediaItem.Type == MediaType.Image ? UrlFormatter.Image(vm.UserId, ImageType.ContentImage, mediaItem.Url) : mediaItem.Url
                };

                if (mediaItem.Type == MediaType.Image)
                {
                    item.Srct = UrlFormatter.Image(vm.UserId, ImageType.ContentImage, mediaItem.Url);
                }

                gallery.Add(item);

                mediaItem.Url = UrlFormatter.Image(vm.UserId, ImageType.ContentImage, mediaItem.Url);
            }

            vm.NanoGaleryJson = JsonSerializer.Serialize(gallery, DefaultJsonSerializeOptions);
        }
    }
}