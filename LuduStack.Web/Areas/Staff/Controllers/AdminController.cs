using LuduStack.Application;
using LuduStack.Application.Helpers;
using LuduStack.Application.Requests.User;
using LuduStack.Application.ViewModels.User;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.Game;
using LuduStack.Domain.Messaging.Queries.UserContent;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Identity.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using LuduStack.Web.Areas.Staff.Controllers.Base;
using LuduStack.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LuduStack.Web.Areas.Staff.Controllers
{
    public class AdminController : StaffBaseController
    {
        private readonly IMediatorHandler mediator;

        public AdminController(IMediatorHandler mediator)
        {
            this.mediator = mediator;
        }

        public Task<ViewResult> Index()
        {
            return Task.FromResult(View());
        }

        public async Task<IActionResult> CheckUserInconsistencies()
        {
            List<string> messages = new List<string>();
            OperationResultListVo<string> result = new OperationResultListVo<string>(messages)
            {
                Message = "Check User Inconsistencies Task"
            };

            try
            {
                OperationResultListVo<ProfileViewModel> profileResult = await ProfileAppService.GetAll(CurrentUserId);

                if (!profileResult.Success)
                {
                    result.Message = "Fail to load user profiles.";
                }

                IQueryable<ApplicationUser> allUsers = await GetUsersAsync();

                foreach (ApplicationUser user in allUsers)
                {
                    AnalyseUser(messages, profileResult, user);
                }

                if (profileResult.Success)
                {
                    foreach (ProfileViewModel profile in profileResult.Value)
                    {
                        AnalyseProfile(messages, allUsers, profile);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                messages.Add(ex.Message);
            }

            result.Value = messages.OrderBy(x => x);

            return View("TaskResult", result);
        }

        public async Task<IActionResult> CheckMissingContentGalleries(bool fix)
        {
            List<string> messages = new List<string>();
            OperationResultListVo<string> result = new OperationResultListVo<string>(messages)
            {
                Message = $"{(fix ? "Fix" : "Check")} Missing Content Galleries Task"
            };

            Guid currentContent;

            try
            {
                Expression<Func<UserContent, bool>> searchCriteria = x => (x.FeaturedImage != null || (x.Media != null && x.Media.Any()));

                IEnumerable<UserContent> allPostsWithImages = await mediator.Query<GetUserContentQuery, IEnumerable<UserContent>>(new GetUserContentQuery(searchCriteria));

                int count = 0;
                foreach (UserContent content in allPostsWithImages)
                {
                    count++;
                    currentContent = content.Id;
                    bool hasChangesToSave = false;
                    Regex pattern = new Regex("(.+)" + content.UserId.ToString() + "/(.+)");
                    string contentUrl = $"<strong><a href=\"/content/{content.Id}\" target=\"_blank\">{content.Id}</a></strong>";

                    List<MediaListItemVo> images = content.Media?.ToList() ?? new List<MediaListItemVo>();
                    if (!string.IsNullOrWhiteSpace(content.FeaturedImage))
                    {
                        Match matchFeaturedImage = pattern.Match(content.FeaturedImage);
                        if (matchFeaturedImage.Success)
                        {
                            if (!fix)
                            {
                                messages.Add($"{count} - content {contentUrl} featuredImage needs short URL");
                            }
                            else
                            {
                                content.FeaturedImage = matchFeaturedImage.Groups[2].Value;
                                messages.Add($"{count} - content {contentUrl} featuredImage URL made short");
                                hasChangesToSave = true;
                            }
                        }

                        MediaListItemVo featuredImageMediaListItemVo = new MediaListItemVo
                        {
                            Url = content.FeaturedImage,
                            CreateDate = DateTime.Now,
                            Type = ContentHelper.GetMediaType(content.FeaturedImage)
                        };

                        images.Add(featuredImageMediaListItemVo);

                        if (content.Media == null)
                        {
                            if (!fix)
                            {
                                messages.Add($"{count} - content {contentUrl} has no <strong>Media</strong> property.");
                            }
                            else
                            {
                                content.Media = new List<MediaListItemVo>
                                {
                                    featuredImageMediaListItemVo
                                };

                                messages.Add($"{count} - <strong>Media</strong> property created and featuredImage added to the content {contentUrl}");

                                hasChangesToSave = true;
                            }
                        }
                        else
                        {
                            if (!fix)
                            {
                                if (!content.Media.Select(x => x.Url).Contains(content.FeaturedImage))
                                {
                                    messages.Add($"{count} - content {contentUrl} need to add the featured image to its gallery.");
                                }

                                if (content.Media.Select(x => x.Url).Contains(Constants.DefaultFeaturedImage))
                                {
                                    messages.Add($"{count} - content {contentUrl} has a placeholder");
                                }
                            }
                            else
                            {
                                if (!content.Media.Any(x => x.Url.Equals(featuredImageMediaListItemVo.Url)))
                                {
                                    content.Media.Add(featuredImageMediaListItemVo);

                                    messages.Add($"{count} - featuredImage added to the <strong>Media</strong> property on the content {contentUrl}");

                                    hasChangesToSave = true;
                                }

                                if (CheckDuplicates(content.Media))
                                {
                                    content.Media = CleanDuplicates(content.Media);
                                    hasChangesToSave = true;
                                }

                                if (CheckPlaceholder(content.Media))
                                {
                                    content.Media = CleanPlaceholder(content.Media);
                                    hasChangesToSave = true;
                                }

                                foreach (MediaListItemVo media in content.Media)
                                {
                                    Match matchMediaItem = pattern.Match(media.Url);
                                    if (matchMediaItem.Success && fix)
                                    {
                                        media.Url = matchMediaItem.Groups[2].Value;
                                        hasChangesToSave = true;
                                    }
                                }
                            }
                        }

                        if (fix && hasChangesToSave)
                        {
                            CommandResult saveContentResult = await mediator.SendCommand(new SaveUserContentCommand(CurrentUserId, content));

                            if (!saveContentResult.Validation.IsValid)
                            {
                                messages.Add(saveContentResult.Validation.Errors.FirstOrDefault().ErrorMessage);
                            }
                        }
                    }
                }

                if (!messages.Any())
                {
                    result.Success = true;
                    messages.Add("No issues found");
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                messages.Add(ex.Message);
            }

            result.Value = messages;

            return View("TaskResult", result);
        }

        public async Task<IActionResult> CheckMissingGameGalleries(bool fix)
        {
            List<string> messages = new List<string>();
            OperationResultListVo<string> result = new OperationResultListVo<string>(messages)
            {
                Message = $"{(fix ? "Fix" : "Check")} Missing Game Galleries Task"
            };

            Guid currentGame;
            try
            {
                List<Game> allGames = (await mediator.Query<GetGameQuery, IEnumerable<Game>>(new GetGameQuery())).ToList();

                List<UserContent> allPosts = (await mediator.Query<GetUserContentQuery, IEnumerable<UserContent>>(new GetUserContentQuery(x => x.GameId.HasValue))).ToList();

                int count = 0;
                foreach (Game game in allGames)
                {
                    int mediaCount = 0;
                    bool hasChangesToSave = false;
                    currentGame = game.Id;
                    Regex pattern = new Regex("(.+)" + game.UserId.ToString() + "/(.+)");
                    string gameUrl = $"<strong><a href=\"/game/{game.Id}\" target=\"_blank\">{game.Title}</a></strong>";

                    List<UserContent> thisGamePosts = allPosts.Where(x => x.GameId.ToString().Equals(game.Id.ToString())).ToList();

                    if (thisGamePosts.Any())
                    {
                        count++;
                        List<UserContent> postsWithFeaturedImage = thisGamePosts.Where(x => !string.IsNullOrWhiteSpace(x.FeaturedImage) && !x.FeaturedImage.Equals(Constants.DefaultFeaturedImage)).ToList();
                        List<MediaListItemVo> allRelatedMedia = new List<MediaListItemVo>();
                        List<string> allFeaturedImages = postsWithFeaturedImage.Select(x => x.FeaturedImage).ToList();
                        IEnumerable<MediaListItemVo> allMediaImages = postsWithFeaturedImage.SelectMany(x => x.Media ?? new List<MediaListItemVo>()).Where(x => !x.Url.Contains("youtu"));

                        if (allFeaturedImages.Any())
                        {
                            foreach (string item in allFeaturedImages)
                            {
                                allRelatedMedia.Add(new MediaListItemVo
                                {
                                    Url = item,
                                    CreateDate = DateTime.Now,
                                    Type = item.Contains("youtu") ? MediaType.Youtube : MediaType.Image
                                });
                            }
                        }

                        if (allRelatedMedia.Any())
                        {
                            allRelatedMedia.AddRange(allMediaImages);

                            allRelatedMedia = allRelatedMedia.Distinct(new MediaListItemVoComparer()).ToList();

                            if (game.Media == null)
                            {
                                game.Media = new List<MediaListItemVo>();

                                if (!fix)
                                {
                                    messages.Add($"{count} - {gameUrl} does not have a gallery and it has {allRelatedMedia.Count} related images.");
                                }
                                else
                                {
                                    hasChangesToSave = true;
                                }
                            }

                            foreach (MediaListItemVo media in allRelatedMedia)
                            {
                                if (!game.Media.Any(x => x.Url.ToLower().Trim().Equals(media.Url.ToLower().Trim())))
                                {
                                    game.Media.Add(media);
                                }
                            }

                            mediaCount = game.Media.Count;
                            game.Media = game.Media.Where(x => !x.Url.Equals(Constants.DefaultFeaturedImage)).ToList();
                            if (game.Media.Count != mediaCount)
                            {
                                if (!fix)
                                {
                                    messages.Add($"{count} - {gameUrl} has placeholders in the gallery");
                                }
                                else
                                {
                                    messages.Add($"{count} - {gameUrl} placeholders removed");
                                    hasChangesToSave = true;
                                }
                            }

                            foreach (MediaListItemVo media in game.Media)
                            {
                                Match match = pattern.Match(media.Url);
                                if (match.Success && fix)
                                {
                                    media.Url = match.Groups[2].Value;
                                    hasChangesToSave = true;
                                }
                            }

                            if (CheckDuplicates(game.Media))
                            {
                                game.Media = CleanDuplicates(game.Media);
                                hasChangesToSave = true;
                            }

                            if (CheckPlaceholder(game.Media))
                            {
                                game.Media = CleanPlaceholder(game.Media);
                                hasChangesToSave = true;
                            }

                            if (fix && hasChangesToSave)
                            {
                                CommandResult savGameResult = await mediator.SendCommand(new SaveGameCommand(CurrentUserId, game, false));

                                if (!savGameResult.Validation.IsValid)
                                {
                                    messages.Add(savGameResult.Validation.Errors.FirstOrDefault().ErrorMessage);
                                }
                            }
                        }
                    }
                }

                if (!messages.Any())
                {
                    result.Success = true;
                    messages.Add("No issues found");
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                messages.Add(ex.Message);
            }

            result.Value = messages;

            return View("TaskResult", result);
        }

        public async Task<IActionResult> FixUserInconcistencies()
        {
            List<string> messages = new List<string>();
            OperationResultListVo<string> result = new OperationResultListVo<string>(messages)
            {
                Message = "Update Handlers Task"
            };

            try
            {
                OperationResultListVo<ProfileViewModel> profileResult = await ProfileAppService.GetAll(CurrentUserId);

                if (!profileResult.Success)
                {
                    result.Message = "Fail to load user profiles.";
                }

                IQueryable<ApplicationUser> allUsers = await GetUsersAsync();

                IQueryable<ApplicationUser> usersWithoutDate = allUsers.Where(x => x.CreateDate == DateTime.MinValue);
                foreach (ApplicationUser user in usersWithoutDate)
                {
                    await FixUserInconsistencies(profileResult, user);
                }

                foreach (ProfileViewModel profile in profileResult.Value)
                {
                    await FixProfileInconsistencies(messages, allUsers, profile);
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                messages.Add("ERROR: " + ex.Message);
            }

            result.Value = messages.OrderBy(x => x);

            return View("TaskResult", result);
        }

        [Route("staff/admin/analyseuser/{userId:guid}")]
        public async Task<IActionResult> AnalyseUser(Guid userId)
        {
            AnalyseUserViewModel model = new AnalyseUserViewModel();

            ApplicationUser user = await UserManager.FindByIdAsync(userId.ToString());
            ProfileViewModel profile = await ProfileAppService.GetByUserId(userId, ProfileType.Personal);

            IList<string> roles = await UserManager.GetRolesAsync(user);

            user.Roles = roles.ToList();

            model.User = user;
            model.Profile = profile;

            return View(model);
        }

        [HttpDelete("staff/admin/deleteuser/{userId:guid}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            CommandResult<OperationResultVo> result = await mediator.SendCommand<DeleteUserFromPlatformRequest, OperationResultVo>(new DeleteUserFromPlatformRequest(CurrentUserId, userId));

            if (result.Success)
            {
                ApplicationUser user = await UserManager.FindByIdAsync(userId.ToString());
                IdentityResult identityResult = await UserManager.DeleteAsync(user);

                Console.WriteLine(identityResult.Succeeded);
            }

            return Json(result.Result);
        }

        public async Task<IQueryable<ApplicationUser>> GetUsersAsync()
        {
            return await Task.Run(() =>
            {
                return UserManager.Users;
            });
        }

        private static void AnalyseUser(List<string> messages, OperationResultListVo<ProfileViewModel> profileResult, ApplicationUser user)
        {
            Guid guid = Guid.Parse(user.Id);
            ProfileViewModel profile = profileResult.Value.FirstOrDefault(x => x.UserId == guid);
            if (profile == null)
            {
                messages.Add($"user {user.UserName} ({user.Id}) without profile");
            }

            if (user.CreateDate == DateTime.MinValue)
            {
                messages.Add($"user {user.UserName} without create date");
            }
        }

        private static void AnalyseProfile(List<string> messages, IQueryable<ApplicationUser> allUsers, ProfileViewModel profile)
        {
            ApplicationUser user = allUsers.FirstOrDefault(x => x.Id.Equals(profile.UserId.ToString()));
            if (user == null)
            {
                messages.Add($"profile {profile.Name} ({profile.Id}) without user {profile.UserId}");
            }
            else
            {
                AnalyseHandler(messages, profile, user);
                AnalyseLocation(messages, profile);
            }
        }

        private static void AnalyseHandler(List<string> messages, ProfileViewModel profile, ApplicationUser user)
        {
            if (string.IsNullOrWhiteSpace(profile.Handler))
            {
                messages.Add($"profile {profile.Name} ({profile.Id}) without handler (should be {user.UserName.ToLower()})");
            }
            else
            {
                if (!profile.Handler.Equals(profile.Handler.ToLower()))
                {
                    messages.Add($"profile {profile.Name} ({profile.Id}) handler ({profile.Handler}) not lowercase");
                }
            }
        }

        private static void AnalyseLocation(List<string> messages, ProfileViewModel profile)
        {
            string profileUrl = GenerateProfileUrl(profile.Handler, profile.Name);

            if (!string.IsNullOrWhiteSpace(profile.Location) && profile.Location.Equals("Earth"))
            {
                messages.Add($"{profileUrl}'s location is set to Earth");
            }
        }

        private async Task FixUserInconsistencies(OperationResultListVo<ProfileViewModel> profileResult, ApplicationUser user)
        {
            ProfileViewModel profile = profileResult.Value.FirstOrDefault(x => x.UserId.ToString().Equals(user.Id));
            if (profile != null)
            {
                user.CreateDate = profile.CreateDate;
                await UserManager.UpdateAsync(user);
            }
        }

        private async Task FixProfileInconsistencies(List<string> messages, IQueryable<ApplicationUser> allUsers, ProfileViewModel profile)
        {
            ApplicationUser user = allUsers.FirstOrDefault(x => x.Id.Equals(profile.UserId.ToString()));
            if (user == null)
            {
                messages.Add($"ERROR: user for {profile.Handler} ({profile.UserId}) NOT FOUND");
            }
            else
            {
                string handler = user.UserName.ToLower();
                if (string.IsNullOrWhiteSpace(profile.Handler) || !profile.Handler.Equals(handler))
                {
                    profile.Handler = handler;
                    await ProfileAppService.Save(CurrentUserId, profile);
                    messages.Add($"SUCCESS: {profile.Name} handler updated to \"{handler}\"");
                }

                if (!string.IsNullOrWhiteSpace(profile.Location) && profile.Location.Equals("Earth"))
                {
                    profile.Location = null;
                    await ProfileAppService.Save(CurrentUserId, profile);
                    messages.Add($"SUCCESS: {profile.Name} handler updated to \"{handler}\"");
                }
            }
        }

        private static string GenerateProfileUrl(string handler, string name)
        {
            return $"<strong><a href=\"/u/{handler}\" target=\"_blank\">{name}</a></strong>";
        }

        private static bool CheckDuplicates(List<MediaListItemVo> mediaList)
        {
            var hasChangesToSave = false;

            int mediaCount = mediaList.Count;

            mediaList = mediaList.Distinct(new MediaListItemVoComparer()).ToList();
            if (mediaList.Count != mediaCount)
            {
                hasChangesToSave = true;
            }

            return hasChangesToSave;
        }

        private static List<MediaListItemVo> CleanDuplicates(List<MediaListItemVo> mediaList)
        {
            mediaList = mediaList.Distinct(new MediaListItemVoComparer()).ToList();

            return mediaList;
        }

        private static bool CheckPlaceholder(List<MediaListItemVo> mediaList)
        {
            var hasChangesToSave = false;

            int mediaCount = mediaList.Count;

            mediaList = mediaList.Where(x => !x.Url.Equals(Constants.DefaultFeaturedImage)).ToList();
            if (mediaList.Count != mediaCount)
            {
                hasChangesToSave = true;
            }

            return hasChangesToSave;
        }

        private static List<MediaListItemVo> CleanPlaceholder(List<MediaListItemVo> mediaList)
        {
            mediaList = mediaList.Where(x => !x.Url.Equals(Constants.DefaultFeaturedImage)).ToList();

            return mediaList;
        }
    }

    public class MediaListItemVoComparer : IEqualityComparer<MediaListItemVo>
    {
        public bool Equals([AllowNull] MediaListItemVo x, [AllowNull] MediaListItemVo y)
        {
            bool equal1 = x.Url.ToLower().Contains(y.Url.ToLower());
            bool equal2 = y.Url.ToLower().Contains(x.Url.ToLower());
            bool equal3 = y.Url.ToLower().Equals(x.Url.ToLower());

            bool equal = equal1 || equal2 || equal3;

            return equal;
        }

        public int GetHashCode([DisallowNull] MediaListItemVo obj)
        {
            return obj.Url.GetHashCode();
        }
    }
}