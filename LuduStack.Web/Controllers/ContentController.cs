using LuduStack.Application;
using LuduStack.Application.Formatters;
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
using LuduStack.Web.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Controllers
{
    public class ContentController : SecureBaseController
    {
        private readonly IUserContentAppService userContentAppService;
        private readonly IGameAppService gameAppService;
        private readonly INotificationAppService notificationAppService;

        public ContentController(IUserContentAppService userContentAppService
            , IGameAppService gameAppService
            , INotificationAppService notificationAppService)
        {
            this.userContentAppService = userContentAppService;
            this.gameAppService = gameAppService;
            this.notificationAppService = notificationAppService;
        }

        [Route("content/{id:guid}")]
        public async Task<IActionResult> Details(Guid id)
        {
            OperationResultVo<UserContentViewModel> serviceResult = userContentAppService.GetById(CurrentUserId, id);

            if (!serviceResult.Success)
            {
                TempData["Message"] = SharedLocalizer["Content not found!"].Value;
                return RedirectToAction("Index", "Home");
            }

            UserContentViewModel vm = serviceResult.Value;

            vm.Content = ContentFormatter.FormatContentToShow(vm.Content);

            SetAuthorDetails(vm);

            if (vm.GameId.HasValue && vm.GameId.Value != Guid.Empty)
            {
                OperationResultVo<GameViewModel> gameServiceResult = gameAppService.GetById(CurrentUserId, vm.GameId.Value);

                GameViewModel game = gameServiceResult.Value;

                vm.GameTitle = game.Title;
                vm.GameThumbnail = UrlFormatter.Image(game.UserId, ImageType.GameThumbnail, game.ThumbnailUrl);
            }

            vm.Content = vm.Content.Replace("image-style-align-right", "image-style-align-right float-right p-10");
            vm.Content = vm.Content.Replace("image-style-align-left", "image-style-align-left float-left p-10");
            vm.Content = vm.Content.Replace("<img src=", @"<img class=""img-fluid"" src=");

            if (string.IsNullOrEmpty(vm.Title))
            {
                vm.Title = SharedLocalizer["Content posted on"] + " " + vm.CreateDate.ToString();
            }

            if (string.IsNullOrWhiteSpace(vm.Introduction))
            {
                vm.Introduction = SharedLocalizer["Content posted on"] + " " + vm.CreateDate.ToShortDateString();
            }

            ApplicationUser user = await UserManager.FindByIdAsync(CurrentUserId.ToString());

            bool userIsAdmin = user != null && await UserManager.IsInRoleAsync(user, Roles.Administrator.ToString());

            vm.Permissions.CanEdit = vm.UserId == CurrentUserId || userIsAdmin;
            vm.Permissions.CanDelete = vm.UserId == CurrentUserId || userIsAdmin;

            ViewData["IsDetails"] = true;

            return View(vm);
        }

        [Route("content/edit/{id:guid}")]
        public IActionResult Edit(Guid id)
        {
            OperationResultVo<UserContentViewModel> serviceResult = userContentAppService.GetById(CurrentUserId, id);

            UserContentViewModel vm = serviceResult.Value;

            IEnumerable<SelectListItemVo> games = gameAppService.GetByUser(vm.UserId);
            List<SelectListItem> gamesDropDown = games.ToSelectList();
            ViewBag.UserGames = gamesDropDown;

            if (!vm.HasFeaturedImage)
            {
                vm.FeaturedImage = Constants.DefaultFeaturedImage;
            }

            return View("CreateEdit", vm);
        }

        public IActionResult Add(Guid? gameId)
        {
            UserContentViewModel vm = new UserContentViewModel
            {
                UserId = CurrentUserId,
                FeaturedImage = Constants.DefaultFeaturedImage
            };

            IEnumerable<SelectListItemVo> games = gameAppService.GetByUser(vm.UserId);
            List<SelectListItem> gamesDropDown = games.ToSelectList();
            ViewBag.UserGames = gamesDropDown;

            if (gameId.HasValue)
            {
                vm.GameId = gameId;
            }

            return View("CreateEdit", vm);
        }

        [HttpPost]
        public IActionResult Save(UserContentViewModel vm)
        {
            try
            {
                bool isNew = vm.Id == Guid.Empty;

                ProfileViewModel profile = ProfileAppService.GetByUserId(CurrentUserId, ProfileType.Personal);

                SetAuthorDetails(vm);

                OperationResultVo<Guid> saveResult = userContentAppService.Save(CurrentUserId, vm);

                if (!saveResult.Success)
                {
                    return Json(saveResult);
                }
                else
                {
                    NotifyFollowers(profile, vm.GameId, vm.Id);

                    string url = Url.Action("Index", "Home", new { area = string.Empty, id = vm.Id, pointsEarned = saveResult.PointsEarned });

                    if (isNew && EnvName.Equals(ConstantHelper.ProductionEnvironmentName))
                    {
                        NotificationSender.SendTeamNotificationAsync("New complex post!");
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
        public IActionResult Delete(Guid id)
        {
            OperationResultVo result = userContentAppService.Remove(CurrentUserId, id);

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
        public IActionResult SimplePost(string text, string images, IEnumerable<PollOptionViewModel> pollOptions, SupportedLanguage? language, Guid? gameId)
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

            ProfileViewModel profile = ProfileAppService.GetByUserId(CurrentUserId, ProfileType.Personal);

            SetAuthorDetails(vm);

            SetContentImages(vm, images);

            OperationResultVo<Guid> result = userContentAppService.Save(CurrentUserId, vm);

            NotifyFollowers(profile, vm.GameId, vm.Id);

            if (EnvName.Equals(ConstantHelper.ProductionEnvironmentName))
            {
                NotificationSender.SendTeamNotificationAsync("New simple post!"); 
            }

            return Json(result);
        }

        [HttpPost]

        #region Content interactions

        [Route("content/like")]
        public IActionResult LikeContent(Guid targetId, UserContentType contentType)
        {
            OperationResultVo response = userContentAppService.ContentLike(CurrentUserId, targetId);

            OperationResultVo<UserContentViewModel> content = userContentAppService.GetById(CurrentUserId, targetId);

            string myName = GetSessionValue(SessionValues.FullName);

            if (contentType == UserContentType.ComicStrip)
            {
                notificationAppService.Notify(CurrentUserId, myName, content.Value.UserId, NotificationType.ComicsLike, targetId);
            }
            else
            {
                notificationAppService.Notify(CurrentUserId, myName, content.Value.UserId, NotificationType.ContentLike, targetId);
            }

            return Json(response);
        }

        [HttpPost]
        [Route("content/unlike")]
        public IActionResult UnLikeContent(Guid targetId)
        {
            OperationResultVo response = userContentAppService.ContentUnlike(CurrentUserId, targetId);

            return Json(response);
        }

        [HttpPost]
        [Route("content/comment")]
        public IActionResult Comment(CommentViewModel vm)
        {
            OperationResultVo response;

            SetAuthorDetails(vm);

            response = userContentAppService.Comment(vm);

            return Json(response);
        }

        #endregion Content interactions

        public Task<IActionResult> Feed(Guid? gameId, Guid? userId, Guid? oldestId, DateTime? oldestDate, bool? articlesOnly)
        {
            ViewComponentResult component = ViewComponent("Feed", new { count = 10, gameId, userId, oldestId, oldestDate, articlesOnly });

            return Task.FromResult((IActionResult)component);
        }

        private void SetContentImages(UserContentViewModel vm, string images)
        {
            if (images != null)
            {
                string[] imgSplit = images.Split('|');
                vm.Images = new List<ImageListItemVo>();

                for (int i = 0; i < imgSplit.Length; i++)
                {
                    if (!string.IsNullOrWhiteSpace(imgSplit[i]))
                    {
                        if (string.IsNullOrWhiteSpace(vm.FeaturedImage))
                        {
                            vm.FeaturedImage = imgSplit[i];
                        }
                        else
                        {
                            vm.Images.Add(new ImageListItemVo
                            {
                                Language = SupportedLanguage.English,
                                Image = imgSplit[i]
                            });
                        }
                    }
                }
            }
        }

        private void NotifyFollowers(ProfileViewModel profile, Guid? gameId, Guid contentId)
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

            gameName = ProcessGamePost(gameId, followersToNotify, gameName);

            Notify(profile, followersToNotify, gameName, targetId);
        }

        private string ProcessGamePost(Guid? gameId, Dictionary<Guid, FollowType> userFollowers, string gameName)
        {
            if (gameId.HasValue)
            {
                OperationResultVo<GameViewModel> gameResult = gameAppService.GetById(CurrentUserId, gameId.Value);

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
                    case FollowType.Content:
                    default:
                        notificationAppService.Notify(CurrentUserId, profile.Name, follower.Key, NotificationType.ContentPosted, targetId);
                        break;
                }
            }
        }
    }
}