using LuduStack.Application;
using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Game;
using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Core.Attributes;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Identity.Models;
using LuduStack.Web.Controllers.Base;
using LuduStack.Web.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Web.Controllers
{
    public class GameController : SecureBaseController
    {
        private readonly IGameAppService gameAppService;
        private readonly INotificationAppService notificationAppService;
        private readonly ITeamAppService teamAppService;
        private readonly ILocalizationAppService translationAppService;

        public GameController(IGameAppService gameAppService
            , INotificationAppService notificationAppService
            , ITeamAppService teamAppService
            , ILocalizationAppService translationAppService) : base()
        {
            this.gameAppService = gameAppService;
            this.notificationAppService = notificationAppService;
            this.teamAppService = teamAppService;
            this.translationAppService = translationAppService;
        }

        [Route("game/{id:guid}")]
        public async Task<IActionResult> Details(Guid id, int? pointsEarned, Guid notificationclicked, bool refreshImages)
        {
            await notificationAppService.MarkAsRead(notificationclicked);

            OperationResultVo<GameViewModel> serviceResult = await gameAppService.GetById(CurrentUserId, id);

            GameViewModel vm = serviceResult.Value;

            if (vm == null)
            {
                return RedirectToAction("list", "game", new { area = string.Empty, msg = SharedLocalizer["Game not found!"] });
            }

            await SetGameTeam(vm);

            SetTranslationPercentage(vm);

            SetImages(vm);

            bool isAdmin = false;

            if (!CurrentUserId.Equals(Guid.Empty))
            {
                ApplicationUser user = await UserManager.FindByIdAsync(CurrentUserId.ToString());
                bool userIsAdmin = await UserManager.IsInRoleAsync(user, Roles.Administrator.ToString());

                isAdmin = user != null && userIsAdmin;
            }

            vm.Permissions.CanEdit = vm.UserId == CurrentUserId || isAdmin;
            vm.Permissions.CanPostActivity = vm.UserId == CurrentUserId;

            SetGamificationMessage(pointsEarned);

            SetImagesToRefresh(vm, refreshImages);

            return View(vm);
        }

        [Route("games/{genre:alpha?}")]
        public async Task<IActionResult> List(GameGenre genre)
        {
            IEnumerable<GameListItemViewModel> latest = await gameAppService.GetLatest(CurrentUserId, 200, Guid.Empty, null, genre);

            ViewBag.Games = latest;
            ViewData["Genre"] = genre;

            Dictionary<GameGenre, UiInfoAttribute> genreDict = Enum.GetValues(typeof(GameGenre)).Cast<GameGenre>().ToUiInfoDictionary();
            ViewData["Genres"] = genreDict;

            return View();
        }

        public IActionResult Add()
        {
            OperationResultVo<GameViewModel> serviceResult = gameAppService.CreateNew(CurrentUserId);

            SetMyTeamsSelectList();

            if (serviceResult.Success)
            {
                return View("CreateEdit", serviceResult.Value);
            }
            else
            {
                return View("CreateEdit", new GameViewModel());
            }
        }

        [Authorize]
        public async Task<IActionResult> Edit(Guid id)
        {
            OperationResultVo<GameViewModel> serviceResult = await gameAppService.GetById(CurrentUserId, id, true);

            GameViewModel vm = serviceResult.Value;

            SetImages(vm);

            SetMyTeamsSelectList();

            SetImagesToRefresh(vm, true);

            return View("CreateEdit", vm);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Save(GameViewModel vm, IFormFile thumbnail)
        {
            try
            {
                bool isNew = vm.Id == Guid.Empty;

                await SetAuthorDetails(vm);
                ClearImagesUrl(vm);

                OperationResultVo<Guid> saveResult = await gameAppService.Save(CurrentUserId, vm);

                if (!saveResult.Success)
                {
                    return Json(saveResult);
                }
                else
                {
                    string url = Url.Action("Details", "Game", new { area = string.Empty, id = saveResult.Value.ToString(), pointsEarned = saveResult.PointsEarned, msg = SharedLocalizer[saveResult.Message], refreshImages = true });

                    if (isNew && EnvName.Equals(Constants.ProductionEnvironmentName))
                    {
                        await NotificationSender.SendTeamNotificationAsync($"New game Created: {vm.Title}");
                    }

                    return Json(new OperationResultRedirectVo(url));
                }
            }
            catch (Exception ex)
            {
                return Json(new OperationResultVo(ex.Message));
            }
        }

        public IActionResult Latest(int qtd, Guid userId)
        {
            if (userId != Guid.Empty)
            {
                qtd = 10;
            }

            return ViewComponent("LatestGames", new { qtd, userId });
        }

        #region Game Like/Unlike

        [HttpPost]
        [Route("game/like")]
        public async Task<IActionResult> LikeGame(Guid likedId)
        {
            OperationResultVo response = await gameAppService.GameLike(CurrentUserId, likedId);

            OperationResultVo<GameViewModel> gameResult = await gameAppService.GetById(CurrentUserId, likedId);

            string fullName = GetSessionValue(SessionValues.FullName);

            await notificationAppService.Notify(CurrentUserId, fullName, gameResult.Value.UserId, NotificationType.GameLike, likedId, gameResult.Value.Title);

            return Json(response);
        }

        [HttpPost]
        [Route("game/unlike")]
        public async Task<IActionResult> UnLikeGame(Guid likedId)
        {
            OperationResultVo response = await gameAppService.GameUnlike(CurrentUserId, likedId);

            return Json(response);
        }

        #endregion Game Like/Unlike

        #region Game Follow/Unfollow

        [HttpPost]
        [Route("game/follow")]
        public async Task<IActionResult> FollowGame(Guid gameId)
        {
            OperationResultVo response = await gameAppService.GameFollow(CurrentUserId, gameId);

            OperationResultVo<GameViewModel> gameResult = await gameAppService.GetById(CurrentUserId, gameId);

            string fullName = GetSessionValue(SessionValues.FullName);

            await notificationAppService.Notify(CurrentUserId, fullName, gameResult.Value.UserId, NotificationType.FollowYourGame, gameId);

            return Json(response);
        }

        [HttpPost]
        [Route("game/unfollow")]
        public async Task<IActionResult> UnFollowGame(Guid gameId)
        {
            OperationResultVo response = await gameAppService.GameUnfollow(CurrentUserId, gameId);

            return Json(response);
        }

        #endregion Game Follow/Unfollow

        [Route("game/byteam/{teamId:guid}")]
        public async Task<IActionResult> ByTeam(Guid teamId)
        {
            IEnumerable<GameListItemViewModel> games = await gameAppService.GetLatest(CurrentUserId, 99, Guid.Empty, teamId, 0);

            return View("_Games", games);
        }

        private void SetImages(GameViewModel vm)
        {
            vm.ThumbnailUrl = string.IsNullOrWhiteSpace(vm.ThumbnailUrl) || Constants.DefaultGameThumbnail.NoExtension().Contains(vm.ThumbnailUrl.NoExtension()) ? Constants.DefaultGameThumbnail : UrlFormatter.Image(vm.UserId, ImageType.GameThumbnail, vm.ThumbnailUrl);

            vm.CoverImageUrl = string.IsNullOrWhiteSpace(vm.CoverImageUrl) || Constants.DefaultGameCoverImage.Contains(vm.CoverImageUrl) ? Constants.DefaultGameCoverImage : UrlFormatter.Image(vm.UserId, ImageType.GameCover, vm.CoverImageUrl);

            vm.AuthorPicture = UrlFormatter.ProfileImage(vm.UserId, 90);
        }

        private async Task SetAuthorDetails(GameViewModel vm)
        {
            if (vm.Id == Guid.Empty || vm.UserId == Guid.Empty || vm.UserId == CurrentUserId)
            {
                vm.UserId = CurrentUserId;
                ProfileViewModel profile = await ProfileAppService.GetByUserId(CurrentUserId, ProfileType.Personal);

                if (profile != null)
                {
                    vm.AuthorName = profile.Name;
                    vm.AuthorPicture = profile.ProfileImageUrl;
                }
            }
        }

        private void ClearImagesUrl(GameViewModel vm)
        {
            vm.ThumbnailUrl = GetUrlLastPart(vm.ThumbnailUrl);
            vm.CoverImageUrl = GetUrlLastPart(vm.CoverImageUrl);
        }

        private static string GetUrlLastPart(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return url;
            }
            else
            {
                string[] split = url.Split('/');
                return split[split.Length - 1];
            }
        }

        private void SetMyTeamsSelectList()
        {
            OperationResultVo teamResult = teamAppService.GetSelectListByUserId(CurrentUserId);

            if (teamResult.Success)
            {
                OperationResultListVo<SelectListItemVo> result = (OperationResultListVo<SelectListItemVo>)teamResult;
                List<SelectListItemVo> items = result.Value.ToList();
                items.Add(new SelectListItemVo(SharedLocalizer["Create a new team (you can edit it later)"], Guid.Empty.ToString()));

                SelectList selectList = new SelectList(items, "Value", "Text");

                ViewData["MyTeams"] = selectList;
            }
        }

        private async Task SetGameTeam(GameViewModel vm)
        {
            if (vm.Team == null && vm.TeamId.HasValue)
            {
                OperationResultVo<Application.ViewModels.Team.TeamViewModel> teamResult = await teamAppService.GetById(CurrentUserId, vm.TeamId.Value);

                if (teamResult.Success)
                {
                    vm.Team = teamResult.Value;
                    vm.Team.Permissions.CanEdit = vm.Team.Permissions.CanDelete = false;
                }
            }
        }

        private void SetTranslationPercentage(GameViewModel vm)
        {
            OperationResultVo percentage = translationAppService.GetPercentageByGameId(CurrentUserId, vm.Id);

            if (percentage.Success)
            {
                OperationResultVo<LocalizationStatsVo> castResult = percentage as OperationResultVo<LocalizationStatsVo>;

                vm.LocalizationPercentage = castResult.Value.LocalizationPercentage;
                vm.LocalizationId = castResult.Value.LocalizationId;
            }
        }

        private void SetImagesToRefresh(GameViewModel vm, bool refresh)
        {
            if (refresh)
            {
                vm.ThumbnailUrl = UrlFormatter.ReplaceCloudVersion(vm.ThumbnailUrl);
                vm.CoverImageUrl = UrlFormatter.ReplaceCloudVersion(vm.CoverImageUrl);
            }
        }
    }
}