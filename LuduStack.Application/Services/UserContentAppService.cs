using AutoMapper.QueryableExtensions;
using LuduStack.Application.Formatters;
using LuduStack.Application.Helpers;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels;
using LuduStack.Application.ViewModels.Content;
using LuduStack.Application.ViewModels.Poll;
using LuduStack.Application.ViewModels.Search;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.UserContent;
using LuduStack.Domain.Messaging.Queries.UserProfile;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class UserContentAppService : ProfileBaseAppService, IUserContentAppService
    {
        private readonly ILogger logger;
        private readonly IPollDomainService pollDomainService;

        public UserContentAppService(IProfileBaseAppServiceCommon profileBaseAppServiceCommon, ILogger<UserContentAppService> logger
            , IPollDomainService pollDomainService) : base(profileBaseAppServiceCommon)
        {
            this.logger = logger;
            this.pollDomainService = pollDomainService;
        }

        public async Task<OperationResultVo<int>> Count(Guid currentUserId)
        {
            try
            {
                int count = await mediator.Query<CountUserContentQuery, int>(new CountUserContentQuery());

                return new OperationResultVo<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<int>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<UserContentViewModel>> GetAll(Guid currentUserId)
        {
            try
            {
                IEnumerable<UserContent> allModels = await mediator.Query<GetUserContentQuery, IEnumerable<UserContent>>(new GetUserContentQuery());

                IEnumerable<UserContentViewModel> vms = mapper.Map<IEnumerable<UserContent>, IEnumerable<UserContentViewModel>>(allModels);

                return new OperationResultListVo<UserContentViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<UserContentViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<Guid>> GetAllIds()
        {
            try
            {
                IEnumerable<Guid> allIds = await mediator.Query<GetUserContentIdsQuery, IEnumerable<Guid>>(new GetUserContentIdsQuery());

                return new OperationResultListVo<Guid>(allIds);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<Guid>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<UserContentIdAndTypeVo>> GetAllContentIds()
        {
            try
            {
                IEnumerable<UserContentIdAndTypeVo> allIds = await mediator.Query<GetUserContentIdsAndTypesQuery, IEnumerable<UserContentIdAndTypeVo>>(new GetUserContentIdsAndTypesQuery());

                return new OperationResultListVo<UserContentIdAndTypeVo>(allIds);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<UserContentIdAndTypeVo>(ex.Message);
            }
        }

        public async Task<OperationResultVo<UserContentViewModel>> GetById(Guid currentUserId, Guid id)
        {
            try
            {
                UserContent model = await mediator.Query<GetUserContentByIdQuery, UserContent>(new GetUserContentByIdQuery(id));

                List<Guid> finalUserIdList = new List<Guid>
                {
                    model.UserId
                };

                List<Guid> commentIds = model.Comments?.Select(y => y.UserId).ToList();
                if (commentIds != null)
                {
                    finalUserIdList.AddRange(commentIds);
                }

                IEnumerable<UserProfileEssentialVo> userProfiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(finalUserIdList));

                UserProfileEssentialVo authorProfile = userProfiles.FirstOrDefault();

                UserContentViewModel vm = mapper.Map<UserContentViewModel>(model);

                if (authorProfile == null)
                {
                    vm.AuthorName = Constants.UnknownSoul;
                }
                else
                {
                    vm.AuthorName = authorProfile.Name;
                }

                SetAuthorDetails(currentUserId, vm, userProfiles);

                vm.HasFeaturedImage = !string.IsNullOrWhiteSpace(vm.FeaturedImage) && !vm.FeaturedImage.Contains(Constants.DefaultFeaturedImage);

                SetFeaturedMedia(vm);

                vm.LikeCount = vm.Likes.Count;

                vm.CommentCount = vm.Comments.Count;

                vm.Poll = SetPoll(currentUserId, vm.Id);

                LoadAuthenticatedData(currentUserId, vm, userProfiles);

                return new OperationResultVo<UserContentViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<UserContentViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo> Remove(Guid currentUserId, Guid id)
        {
            try
            {
                pollDomainService.RemoveByContentId(id);

                await mediator.SendCommand(new DeleteUserContentCommand(currentUserId, id));

                return new OperationResultVo(true);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo<Guid>> Save(Guid currentUserId, UserContentViewModel viewModel)
        {
            try
            {
                int pointsEarned = 0;

                UserContent model;
                Poll pollModel = null;

                bool isSpam = await CheckSpam(viewModel.Id, currentUserId, viewModel.Content);

                bool isNew = viewModel.Id == Guid.Empty;

                if (isSpam)
                {
                    return new OperationResultVo<Guid>("Calm down! You cannot post the same content twice in a row.");
                }

                UserContent existing = await mediator.Query<GetUserContentByIdQuery, UserContent>(new GetUserContentByIdQuery(viewModel.Id));
                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<UserContent>(viewModel);
                }

                if (isNew && viewModel.Poll != null && viewModel.Poll.PollOptions != null && viewModel.Poll.PollOptions.Any())
                {
                    pollModel = mapper.Map<Poll>(viewModel.Poll);
                }

                CommandResult result = await mediator.SendCommand(new SaveUserContentCommand(currentUserId, model, pollModel));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }
                else
                {
                    pointsEarned += result.PointsEarned;

                    return new OperationResultVo<Guid>(model.Id, pointsEarned, isNew ? "Post published!" : "Content saved!");
                }
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        private async Task<bool> CheckSpam(Guid id, Guid userId, string content)
        {
            IEnumerable<UserContent> all = await mediator.Query<GetUserContentQuery, IEnumerable<UserContent>>(new GetUserContentQuery());

            if (all.Any())
            {
                UserContent latest = all.OrderBy(x => x.CreateDate).Last();
                bool sameContent = latest.Content.Trim().ToLower().Replace(" ", string.Empty).Equals(content.Trim().ToLower().Replace(" ", string.Empty)) && latest.UserId == userId;
                bool sameId = latest.Id == id;

                return sameContent && !sameId;
            }
            else
            {
                return false;
            }
        }

        public async Task<int> CountArticles()
        {
            int count = await mediator.Query<CountUserContentQuery, int>(new CountUserContentQuery(x => !string.IsNullOrEmpty(x.Title) && !string.IsNullOrEmpty(x.Introduction) && !string.IsNullOrEmpty(x.FeaturedImage) && x.Content.Length > 50));

            return count;
        }

        public async Task<IEnumerable<UserContentViewModel>> GetActivityFeed(ActivityFeedRequestViewModel vm)
        {
            try
            {
                GetActivityFeedQuery query = new GetActivityFeedQuery
                {
                    GameId = vm.GameId,
                    UserId = vm.UserId,
                    SingleContentId = vm.SingleContentId,
                    Languages = vm.Languages,
                    OldestId = vm.OldestId,
                    OldestDate = vm.OldestDate,
                    ArticlesOnly = vm.ArticlesOnly,
                    Count = vm.Count
                };

                List<UserContent> allModels = await mediator.Query<GetActivityFeedQuery, List<UserContent>>(query);

                IEnumerable<UserContentViewModel> viewModels = mapper.Map<IEnumerable<UserContent>, IEnumerable<UserContentViewModel>>(allModels);

                IEnumerable<Guid> userIds = viewModels.Select(x => x.UserId);
                IEnumerable<Guid> commenterUserIds = viewModels.SelectMany(x => x.Comments.Select(y => y.UserId));
                IEnumerable<Guid> finalUserIdList = userIds.Concat(commenterUserIds);

                IEnumerable<UserProfileEssentialVo> userProfiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(finalUserIdList));

                IEnumerable<Guid> userContentIds = viewModels.Select(x => x.Id);
                IEnumerable<Poll> polls = await mediator.Query<GetPollsByUserContentIdsQuery, IEnumerable<Poll>>(new GetPollsByUserContentIdsQuery(userContentIds));

                foreach (UserContentViewModel item in viewModels)
                {
                    item.CreateDate = item.CreateDate.ToLocalTime();

                    item.PublishDate = item.PublishDate.ToLocalTime();

                    UserProfileEssentialVo authorProfile = userProfiles.FirstOrDefault(x => x.UserId == item.UserId);
                    if (authorProfile == null)
                    {
                        item.AuthorName = Constants.UnknownSoul;
                    }
                    else
                    {
                        item.AuthorName = authorProfile.Name;
                        item.UserHandler = authorProfile.Handler;
                    }

                    item.AuthorPicture = UrlFormatter.ProfileImage(item.UserId, Constants.SmallAvatarSize);

                    item.IsArticle = !string.IsNullOrWhiteSpace(item.Title) && !string.IsNullOrWhiteSpace(item.Introduction);

                    SetFeaturedMedia(item);

                    item.HasFeaturedImage = !string.IsNullOrWhiteSpace(item.FeaturedImage) && !item.FeaturedImage.Contains(Constants.DefaultFeaturedImage);

                    item.LikeCount = item.Likes.Count;

                    item.CommentCount = item.Comments.Count;

                    item.Poll = SetPoll(vm.CurrentUserId, item.Id, polls);

                    LoadAuthenticatedData(vm.CurrentUserId, item, userProfiles);

                    item.Content = item.Content.ReplaceCloudname();

                    foreach (CommentViewModel comment in item.Comments)
                    {
                        comment.Text = ContentFormatter.FormatHashTagsToShow(comment.Text);
                    }

                    item.Permissions.CanEdit = !item.HasPoll && (item.UserId == vm.CurrentUserId || vm.CurrentUserIsAdmin);

                    item.Permissions.CanDelete = item.UserId == vm.CurrentUserId || vm.CurrentUserIsAdmin;

                    item.Content = ContentFormatter.FormatContentToShow(item.Content);
                    if (item.FeaturedMediaType == MediaType.Youtube)
                    {
                        item.FeaturedImageResponsive = ContentFormatter.GetYoutubeVideoId(item.FeaturedImage);
                        item.FeaturedImageLquip = UrlFormatter.FormatFeaturedImageUrl(Guid.Empty, Constants.DefaultFeaturedImageLquip, ImageRenderType.LowQuality);
                    }
                }

                return viewModels;
            }
            catch (Exception ex)
            {
                string msg = $"Unable to get the Activity Feed.";
                logger.Log(LogLevel.Error, ex, msg);
                throw;
            }
        }

        public async Task<OperationResultListVo<UserContentSearchViewModel>> Search(Guid currentUserId, string q)
        {
            try
            {
                IEnumerable<UserContent> all = await mediator.Query<SearchUserContentQuery, IEnumerable<UserContent>>(new SearchUserContentQuery(x => x.Content.Contains(q) || x.Introduction.Contains(q)));

                IEnumerable<UserContentSearchVo> selected = all.OrderByDescending(x => x.CreateDate)
                    .Select(x => new UserContentSearchVo
                    {
                        ContentId = x.Id,
                        Title = x.Title,
                        FeaturedImage = x.FeaturedImage,
                        Content = (string.IsNullOrWhiteSpace(x.Introduction) ? x.Content : x.Introduction).GetFirstWords(20),
                        Language = (x.Language == 0 ? SupportedLanguage.English : x.Language)
                    });

                IQueryable<UserContentSearchVo> queryable = selected.AsQueryable();

                IQueryable<UserContentSearchViewModel> vms = queryable.ProjectTo<UserContentSearchViewModel>(mapper.ConfigurationProvider);

                return new OperationResultListVo<UserContentSearchViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<UserContentSearchViewModel>(ex.Message);
            }
        }

        private PollViewModel SetPoll(Guid currentUserId, Guid contentId)
        {
            return SetPoll(currentUserId, contentId, null);
        }

        private PollViewModel SetPoll(Guid currentUserId, Guid contentId, IEnumerable<Poll> polls)
        {
            PollViewModel pollVm = null;
            Poll poll = null;

            if (polls != null)
            {
                poll = polls.FirstOrDefault(x => x.UserContentId == contentId);
            }
            else
            {
                poll = pollDomainService.GetByUserContentId(contentId);
            }

            if (poll != null)
            {
                pollVm = new PollViewModel();

                int totalVotes = poll.Votes.Count;
                pollVm.TotalVotes = totalVotes;

                foreach (PollOption option in poll.Options)
                {
                    PollOptionViewModel loadedOption = new PollOptionViewModel
                    {
                        Id = option.Id,
                        Text = option.Text
                    };

                    loadedOption.Votes = poll.Votes.Count(x => x.PollOptionId == option.Id);
                    loadedOption.VotePercentage = loadedOption.Votes > 0 ? (loadedOption.Votes / (decimal)totalVotes) * 100 : 0;
                    loadedOption.CurrentUserVoted = poll.Votes.Any(x => x.PollOptionId == option.Id && x.UserId == currentUserId);

                    pollVm.PollOptions.Add(loadedOption);
                }
            }

            return pollVm;
        }

        private void LoadAuthenticatedData(Guid currentUserId, UserGeneratedCommentBaseViewModel item, IEnumerable<UserProfileEssentialVo> userProfiles)
        {
            if (currentUserId != Guid.Empty)
            {
                item.CurrentUserLiked = item.Likes.Any(x => x == currentUserId);

                foreach (CommentViewModel comment in item.Comments)
                {
                    UserProfileEssentialVo commenterProfile = userProfiles.FirstOrDefault(x => x.UserId == comment.UserId);
                    if (commenterProfile == null)
                    {
                        comment.AuthorName = Constants.UnknownSoul;
                    }
                    else
                    {
                        comment.AuthorName = commenterProfile.Name;
                        comment.UserHandler = commenterProfile.Handler;
                    }

                    comment.AuthorPicture = UrlFormatter.ProfileImage(comment.UserId, 38);
                    comment.Text = string.IsNullOrWhiteSpace(comment.Text) ? Constants.SoundOfSilence : comment.Text;
                }
            }
        }

        public async Task<OperationResultVo> ContentLike(Guid currentUserId, Guid targetId)
        {
            try
            {
                CommandResult result = await mediator.SendCommand(new LikeUserContentCommand(currentUserId, targetId));

                if (!result.Validation.IsValid)
                {
                    return new OperationResultVo(result.Validation.Errors.First().ErrorMessage);
                }
                else
                {
                    int likeCount = await mediator.Query<CountLikesQuery, int>(new CountLikesQuery(x => x.ContentId == targetId));

                    return new OperationResultVo<int>(likeCount);
                }
            }
            catch (Exception ex)
            {
                string msg = $"Unable Like the content.";
                logger.Log(LogLevel.Error, ex, msg);
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> ContentUnlike(Guid currentUserId, Guid targetId)
        {
            try
            {
                CommandResult result = await mediator.SendCommand(new UnlikeUserContentCommand(currentUserId, targetId));

                if (!result.Validation.IsValid)
                {
                    return new OperationResultVo(result.Validation.Errors.First().ErrorMessage);
                }
                else
                {
                    int likeCount = await mediator.Query<CountLikesQuery, int>(new CountLikesQuery(x => x.ContentId == targetId));

                    return new OperationResultVo<int>(likeCount);
                }
            }
            catch (Exception ex)
            {
                string msg = $"Unable Unlike the content.";
                logger.Log(LogLevel.Error, ex, msg);
                throw;
            }
        }

        public async Task<OperationResultVo> Comment(Guid currentUserId, CommentViewModel vm)
        {
            try
            {
                await SetAuthorDetails(currentUserId, vm);

                AddCommentUserContentCommand command = new AddCommentUserContentCommand(vm.UserId, vm.UserContentId, vm.ParentCommentId, vm.Text);

                CommandResult result = await mediator.SendCommand(command);

                if (result.Validation.IsValid)
                {
                    int newCount = await mediator.Query<CountCommentsQuery, int>(new CountCommentsQuery(x => x.UserContentId == command.Id && x.UserId == command.UserId));

                    return new OperationResultVo<int>(newCount, "Your comment was added");
                }
                else
                {
                    return new OperationResultVo(false, result.Validation.Errors.FirstOrDefault().ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> GetCommentsByUserId(Guid currentUserId, Guid userId)
        {
            try
            {
                IEnumerable<UserContentComment> comments = await mediator.Query<GetCommentsQuery, IEnumerable<UserContentComment>>(new GetCommentsQuery(x => x.UserId == userId));

                List<CommentViewModel> vms = mapper.Map<List<CommentViewModel>>(comments);

                return new OperationResultListVo<CommentViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        private static void SetFeaturedMedia(UserContentViewModel item)
        {
            if (string.IsNullOrWhiteSpace(item.FeaturedImage) && item.Media.Any(x => x.Language == item.Language))
            {
                item.FeaturedImage = item.Media.FirstOrDefault(x => x.Language == item.Language)?.Url;
            }
            else if (string.IsNullOrWhiteSpace(item.FeaturedImage) && item.Media.Any())
            {
                item.FeaturedImage = item.Media.FirstOrDefault()?.Url;
            }

            item.FeaturedMediaType = ContentHelper.GetMediaType(item.FeaturedImage);

            switch (item.FeaturedMediaType)
            {
                case MediaType.Image:
                    SetFeaturedImageUrls(item, item.FeaturedImage);
                    break;

                case MediaType.Video:
                    SetFeaturedVideoUrl(item, item.FeaturedImage);
                    break;

                default:
                    break;
            }
        }

        private static void SetFeaturedImageUrls(UserContentViewModel item, string selectedFeaturedMedia)
        {
            item.FeaturedImage = UrlFormatter.FormatFeaturedImageUrl(item.UserId, selectedFeaturedMedia, ImageRenderType.Full);
            item.FeaturedImageResponsive = UrlFormatter.FormatFeaturedImageUrl(item.UserId, selectedFeaturedMedia, ImageRenderType.Responsive);
            item.FeaturedImageLquip = UrlFormatter.FormatFeaturedImageUrl(item.UserId, selectedFeaturedMedia, ImageRenderType.LowQuality);
        }

        private static void SetFeaturedVideoUrl(UserContentViewModel item, string selectedFeaturedMedia)
        {
            item.FeaturedImage = UrlFormatter.FormatFeaturedVideoUrl(item.UserId, selectedFeaturedMedia);
        }
    }
}