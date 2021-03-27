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
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class UserContentAppService : ProfileBaseAppService, IUserContentAppService
    {
        private readonly ILogger logger;
        private readonly IUserContentDomainService userContentDomainService;
        private readonly IGamificationDomainService gamificationDomainService;
        private readonly IPollDomainService pollDomainService;

        public UserContentAppService(IMediatorHandler mediator, IProfileBaseAppServiceCommon profileBaseAppServiceCommon, ILogger<UserContentAppService> logger
            , IUserContentDomainService userContentDomainService
            , IGamificationDomainService gamificationDomainService
            , IPollDomainService pollDomainService) : base(mediator, profileBaseAppServiceCommon)
        {
            this.logger = logger;
            this.userContentDomainService = userContentDomainService;
            this.gamificationDomainService = gamificationDomainService;
            this.pollDomainService = pollDomainService;
            this.pollDomainService = pollDomainService;
        }

        #region ICrudAppService

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

        public async Task<OperationResultVo> GetAllIds(Guid currentUserId)
        {
            try
            {
                IEnumerable<Guid> allIds = userContentDomainService.GetAllIds();

                return new OperationResultListVo<Guid>(allIds);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo<UserContentViewModel>> GetById(Guid currentUserId, Guid id)
        {
            try
            {
                UserContent model = await mediator.Query<GetUserContentByIdQuery, UserContent>(new GetUserContentByIdQuery(id));

                UserProfile authorProfile = GetCachedProfileByUserId(model.UserId);

                UserContentViewModel vm = mapper.Map<UserContentViewModel>(model);

                if (authorProfile == null)
                {
                    vm.AuthorName = Constants.UnknownSoul;
                }
                else
                {
                    vm.AuthorName = authorProfile.Name;
                }

                vm.HasFeaturedImage = !string.IsNullOrWhiteSpace(vm.FeaturedImage) && !vm.FeaturedImage.Contains(Constants.DefaultFeaturedImage);

                vm.FeaturedMediaType = GetMediaType(vm.FeaturedImage);

                if (vm.FeaturedMediaType != MediaType.Youtube)
                {
                    vm.FeaturedImage = ContentHelper.SetFeaturedImage(vm.UserId, vm.FeaturedImage, ImageRenderType.Full);
                    vm.FeaturedImageLquip = ContentHelper.SetFeaturedImage(vm.UserId, vm.FeaturedImage, ImageRenderType.LowQuality);
                }

                vm.LikeCount = vm.Likes.Count;

                vm.CommentCount = vm.Comments.Count;

                vm.Poll = SetPoll(currentUserId, vm.Id);

                LoadAuthenticatedData(currentUserId, vm);

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

                userContentDomainService.Remove(id);

                await unitOfWork.Commit();

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

                bool isSpam = await CheckSpam(viewModel.Id, viewModel.Content);

                bool isNew = viewModel.Id == Guid.Empty;

                if (isSpam)
                {
                    return new OperationResultVo<Guid>("Calm down! You cannot post the same content twice in a row.");
                }

                string youtubePattern = @"(https?\:\/\/)?(www\.youtube\.com|youtu\.?be)\/.+";

                viewModel.Content = Regex.Replace(viewModel.Content, youtubePattern, delegate (Match match)
                {
                    string v = match.ToString();
                    if (match.Index == 0 && String.IsNullOrWhiteSpace(viewModel.FeaturedImage))
                    {
                        viewModel.FeaturedImage = v;
                    }
                    return v;
                });

                UserContent existing = await mediator.Query<GetUserContentByIdQuery, UserContent>(new GetUserContentByIdQuery(viewModel.Id));
                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<UserContent>(viewModel);
                }

                if (model.PublishDate == DateTime.MinValue)
                {
                    model.PublishDate = model.CreateDate;
                }

                if (isNew)
                {
                    userContentDomainService.Add(model);

                    PlatformAction action = viewModel.IsComplex ? PlatformAction.ComplexPost : PlatformAction.SimplePost;
                    pointsEarned += gamificationDomainService.ProcessAction(viewModel.UserId, action);

                    unitOfWork.Commit().Wait();
                    viewModel.Id = model.Id;

                    if (viewModel.Poll != null && viewModel.Poll.PollOptions != null && viewModel.Poll.PollOptions.Any())
                    {
                        CreatePoll(viewModel);

                        pointsEarned += gamificationDomainService.ProcessAction(viewModel.UserId, PlatformAction.PollPost);
                    }
                }
                else
                {
                    userContentDomainService.Update(model);
                }

                await unitOfWork.Commit();

                return new OperationResultVo<Guid>(model.Id, pointsEarned);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        #endregion ICrudAppService

        private async Task<bool> CheckSpam(Guid id, string content)
        {
            IEnumerable<UserContent> all = await mediator.Query<GetUserContentQuery, IEnumerable<UserContent>>(new GetUserContentQuery());

            if (all.Any())
            {
                UserContent latest = all.OrderBy(x => x.CreateDate).Last();
                bool sameContent = latest.Content.Trim().ToLower().Replace(" ", string.Empty).Equals(content.Trim().ToLower().Replace(" ", string.Empty));
                bool sameId = latest.Id == id;

                return sameContent && !sameId;
            }
            else
            {
                return false;
            }
        }

        private void CreatePoll(UserContentViewModel contentVm)
        {
            Poll newPoll = new Poll
            {
                UserId = contentVm.UserId,
                UserContentId = contentVm.Id
            };

            foreach (PollOptionViewModel o in contentVm.Poll.PollOptions)
            {
                PollOption newOption = new PollOption
                {
                    UserId = contentVm.UserId,
                    Text = o.Text
                };

                newPoll.Options.Add(newOption);
            }

            pollDomainService.Add(newPoll);
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
                List<UserContent> allModels = await mediator.Query<GetActivityFeedQuery, List<UserContent>>(new GetActivityFeedQuery(vm.GameId, vm.UserId, vm.Languages, vm.OldestId, vm.OldestDate, vm.ArticlesOnly, vm.Count));

                IEnumerable<UserContentViewModel> viewModels = mapper.Map<IEnumerable<UserContent>, IEnumerable<UserContentViewModel>>(allModels);

                foreach (UserContentViewModel item in viewModels)
                {
                    item.CreateDate = item.CreateDate.ToLocalTime();

                    item.PublishDate = item.PublishDate.ToLocalTime();

                    UserProfile authorProfile = GetCachedProfileByUserId(item.UserId);
                    if (authorProfile == null)
                    {
                        item.AuthorName = Constants.UnknownSoul;
                    }
                    else
                    {
                        item.AuthorName = authorProfile.Name;
                    }

                    item.AuthorPicture = UrlFormatter.ProfileImage(item.UserId, 40);

                    item.IsArticle = !string.IsNullOrWhiteSpace(item.Title) && !string.IsNullOrWhiteSpace(item.Introduction);

                    item.FeaturedMediaType = GetMediaType(item.FeaturedImage);
                    if (item.FeaturedMediaType == MediaType.Youtube)
                    {
                        item.Content = string.Empty;
                    }

                    if (item.FeaturedMediaType != MediaType.Youtube)
                    {
                        SetFeaturedImage(item);
                    }

                    item.HasFeaturedImage = !string.IsNullOrWhiteSpace(item.FeaturedImage) && !item.FeaturedImage.Contains(Constants.DefaultFeaturedImage);

                    item.LikeCount = item.Likes.Count;

                    item.CommentCount = item.Comments.Count;

                    item.Poll = SetPoll(vm.CurrentUserId, item.Id);

                    LoadAuthenticatedData(vm.CurrentUserId, item);

                    item.Content = item.Content.ReplaceCloudname();
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
            PollViewModel pollVm = null;
            Poll poll = pollDomainService.GetByUserContentId(contentId);

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

        private void LoadAuthenticatedData(Guid currentUserId, UserGeneratedCommentBaseViewModel item)
        {
            if (currentUserId != Guid.Empty)
            {
                item.CurrentUserLiked = item.Likes.Any(x => x == currentUserId);

                foreach (CommentViewModel comment in item.Comments)
                {
                    UserProfile commenterProfile = GetCachedProfileByUserId(comment.UserId);
                    if (commenterProfile == null)
                    {
                        comment.AuthorName = Constants.UnknownSoul;
                    }
                    else
                    {
                        comment.AuthorName = commenterProfile.Name;
                    }

                    comment.AuthorPicture = UrlFormatter.ProfileImage(comment.UserId);
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

        public async Task<OperationResultVo> Comment(CommentViewModel vm)
        {
            try
            {
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

        private static void SetFeaturedImage(UserContentViewModel item)
        {
            string selectedFeaturedImage = item.FeaturedImage;

            if (string.IsNullOrWhiteSpace(item.FeaturedImage) && item.Images.Any(x => x.Language == item.Language))
            {
                selectedFeaturedImage = item.Images.FirstOrDefault(x => x.Language == item.Language)?.Image;
            }
            else if (string.IsNullOrWhiteSpace(item.FeaturedImage) && item.Images.Any())
            {
                selectedFeaturedImage = item.Images.FirstOrDefault()?.Image;
            }

            item.FeaturedImage = ContentHelper.SetFeaturedImage(item.UserId, selectedFeaturedImage, ImageRenderType.Full);
            item.FeaturedImageResponsive = ContentHelper.SetFeaturedImage(item.UserId, selectedFeaturedImage, ImageRenderType.Responsive);
            item.FeaturedImageLquip = ContentHelper.SetFeaturedImage(item.UserId, selectedFeaturedImage, ImageRenderType.LowQuality);
        }
    }
}