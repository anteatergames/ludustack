using Ganss.Xss;
using LuduStack.Application.Formatters;
using LuduStack.Application.Helpers;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Forum;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Commands.Forum;
using LuduStack.Domain.Messaging.Queries.ForumCategory;
using LuduStack.Domain.Messaging.Queries.ForumGroup;
using LuduStack.Domain.Messaging.Queries.ForumPost;
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
    public class ForumAppService : BaseAppService, IForumAppService
    {
        private readonly ILogger logger;

        private readonly IPlatformSettingAppService platformSettingAppService;

        public ForumAppService(IBaseAppServiceCommon baseAppServiceCommon, ILogger<ForumAppService> logger, IPlatformSettingAppService platformSettingAppService) : base(baseAppServiceCommon)
        {
            this.logger = logger;
            this.platformSettingAppService = platformSettingAppService;
        }

        public async Task<OperationResultVo<ForumPostViewModel>> GenerateNewTopic(Guid currentUserId, Guid? categoryId)
        {
            try
            {
                ForumPostViewModel viewModel = new ForumPostViewModel
                {
                    UserId = currentUserId,
                    IsOriginalPost = true
                };

                if (categoryId.HasValue)
                {
                    ForumCategory category = await mediator.Query<GetForumCategoryByIdQuery, ForumCategory>(new GetForumCategoryByIdQuery(categoryId.Value));

                    if (category != null)
                    {
                        viewModel.ForumCategoryId = category.Id;
                        viewModel.CategoryHandler = category.Handler;
                    }
                }

                return new OperationResultVo<ForumPostViewModel>(viewModel);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<ForumPostViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<ForumIndexViewModel>> GetAllCategoriesByGroup(Guid currentUserId, GetAllCategoriesRequestViewModel request)
        {
            try
            {
                IEnumerable<ForumGroup> allGroups = await mediator.Query<GetForumGroupQuery, IEnumerable<ForumGroup>>(new GetForumGroupQuery());
                List<ForumGroupViewModel> allGroupsVms = mapper.Map<IEnumerable<ForumGroup>, IEnumerable<ForumGroupViewModel>>(allGroups).ToList();

                if (!allGroupsVms.Any())
                {
                    return new OperationResultVo<ForumIndexViewModel>("No Groups");
                }

                ForumIndexViewModel model = new ForumIndexViewModel
                {
                    Groups = allGroupsVms.OrderBy(x => x.Order).ToList()
                };

                List<ForumCategoryListItemVo> allCategories = await GetCategoryList(request.Languages);

                foreach (ForumGroupViewModel group in model.Groups)
                {
                    group.Slug = group.Name.Slugify();
                    group.Categories = allCategories.Where(x => x.GroupId == group.Id).ToList();
                }

                return new OperationResultVo<ForumIndexViewModel>(model);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<ForumIndexViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<ForumCategoryListItemVo>> GetAllCategories(Guid currentUserId, GetAllCategoriesRequestViewModel request)
        {
            try
            {
                List<ForumCategoryListItemVo> vms = await GetCategoryList(request.Languages);

                return new OperationResultListVo<ForumCategoryListItemVo>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<ForumCategoryListItemVo>(ex.Message);
            }
        }

        public async Task<OperationResultVo<ForumCategoryViewModel>> GetCategory(Guid currentUserId, Guid id, string handler)
        {
            try
            {
                ForumCategory model = await mediator.Query<GetForumCategoryByIdQuery, ForumCategory>(new GetForumCategoryByIdQuery(id, handler));

                ForumCategoryViewModel vm = mapper.Map<ForumCategory, ForumCategoryViewModel>(model);

                if (!string.IsNullOrWhiteSpace(vm.FeaturedImage) && vm.FeaturedImage.Equals(Constants.DefaultFeaturedImage))
                {
                    vm.FeaturedImage = null;
                }

                return new OperationResultVo<ForumCategoryViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<ForumCategoryViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<ForumPostListVo>> GetPosts(Guid currentUserId, GetForumPostsRequestViewModel viewModel)
        {
            try
            {
                List<Guid> profilesToGet = new List<Guid>();

                OperationResultVo<ViewModels.PlatformSetting.PlatformSettingViewModel> itemsPerPageSetting = await platformSettingAppService.GetByElement(currentUserId, PlatformSettingElement.ForumPageSize);

                GetForumPostListQuery query = new GetForumPostListQuery
                {
                    CategoryId = viewModel.ForumCategoryId,
                    Count = viewModel.Count ?? int.Parse(itemsPerPageSetting.Value.Value),
                    Page = viewModel.Page ?? 1,
                    Languages = viewModel.Languages
                };

                ForumPostListVo queryResult = await mediator.Query<GetForumPostListQuery, ForumPostListVo>(query);

                IEnumerable<Guid> userIds = queryResult.Posts.Select(x => x.UserId);
                profilesToGet.AddRange(userIds);

                IEnumerable<ForumPostCounterResultVo> postCounters = await mediator.Query<GetForumPostCountersQuery, IEnumerable<ForumPostCounterResultVo>>(new GetForumPostCountersQuery(viewModel.ForumCategoryId));

                foreach (ForumPostListItemVo forumPost in queryResult.Posts)
                {
                    ForumPostCounterResultVo postStats = postCounters.FirstOrDefault(x => x.OriginalPostId == forumPost.Id);
                    if (postStats != null)
                    {
                        forumPost.ReplyCount = postStats.ReplyCount;
                        forumPost.ViewCount = postStats.ViewCount;
                        forumPost.LatestReply = postStats.LatestReply;
                    }

                    if (forumPost.LatestReply != null)
                    {
                        profilesToGet.Add(forumPost.LatestReply.UserId);
                    }

                    if (forumPost.Language == 0)
                    {
                        forumPost.Language = SupportedLanguage.English;
                    }
                }

                IEnumerable<UserProfileEssentialVo> userProfiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(profilesToGet));

                foreach (ForumPostListItemVo forumPost in queryResult.Posts)
                {
                    SetProfiles(forumPost, userProfiles);

                    if (forumPost.LatestReply != null)
                    {
                        UserProfileEssentialVo latestPostAuthorProfile = userProfiles.FirstOrDefault(x => x.UserId == forumPost.LatestReply.UserId);

                        if (latestPostAuthorProfile != null)
                        {
                            forumPost.LatestReply.UserHandler = latestPostAuthorProfile.Handler;
                            forumPost.LatestReply.AuthorName = latestPostAuthorProfile.Name;
                            forumPost.LatestReply.AuthorPicture = UrlFormatter.ProfileImage(forumPost.LatestReply.UserId, Constants.SmallAvatarSize);
                        }
                    }
                }

                return new OperationResultVo<ForumPostListVo>(queryResult);
            }
            catch (Exception ex)
            {
                string msg = $"Unable to get the Activity Feed.";
                logger.Log(LogLevel.Error, ex, msg);
                throw;
            }
        }

        public async Task<OperationResultVo<Guid>> SavePost(Guid currentUserId, ForumPostViewModel viewModel)
        {
            try
            {
                ForumPost model;

                ForumPost existing = await mediator.Query<GetForumPostByIdQuery, ForumPost>(new GetForumPostByIdQuery(viewModel.Id));
                if (existing != null)
                {
                    existing.Content = viewModel.Content;
                    model = existing;
                }
                else
                {
                    model = mapper.Map<ForumPost>(viewModel);
                }

                HtmlSanitizer sanitizer = ContentHelper.GetHtmlSanitizer();
                model.Content = SanitizeHtmlToSave(model.Content, sanitizer);

                CommandResult result = await mediator.SendCommand(new SaveForumPostCommand(model));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                return new OperationResultVo<Guid>(model.OriginalPostId, 0, "Forum Post saved!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public async Task<OperationResultVo<ForumPostViewModel>> GetPostForDetails(Guid currentUserId, bool currentUserIsAdmin, Guid id)
        {
            try
            {
                ForumPost existing = await mediator.Query<GetForumPostByIdQuery, ForumPost>(new GetForumPostByIdQuery(id));

                ForumPostViewModel viewModel = mapper.Map<ForumPostViewModel>(existing);

                ForumCategory category = await mediator.Query<GetForumCategoryByIdQuery, ForumCategory>(new GetForumCategoryByIdQuery(existing.ForumCategoryId));

                if (category != null)
                {
                    viewModel.CategoryHandler = category.Handler;
                    viewModel.CategoryName = category.Name;
                }

                SetVotes(currentUserId, viewModel, existing);

                List<Guid> profilesToGet = new List<Guid> { viewModel.UserId };

                IEnumerable<UserProfileEssentialVo> userProfiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(profilesToGet));

                SetProfiles(viewModel, userProfiles);

                SetPermissions(currentUserId, currentUserIsAdmin, viewModel);

                HtmlSanitizer sanitizer = ContentHelper.GetHtmlSanitizer();
                SanitizeHtmlToDisplay(viewModel, sanitizer);

                if (viewModel.Language == 0)
                {
                    viewModel.Language = SupportedLanguage.English;
                }

                await mediator.SendCommand(new RegisterForumPostViewCommand(id, currentUserId));

                return new OperationResultVo<ForumPostViewModel>(viewModel);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<ForumPostViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<ForumPostViewModel>> GetPostForEdit(Guid currentUserId, Guid id)
        {
            try
            {
                ForumPost existing = await mediator.Query<GetForumPostByIdQuery, ForumPost>(new GetForumPostByIdQuery(id));

                ForumPostViewModel viewModel = mapper.Map<ForumPostViewModel>(existing);

                return new OperationResultVo<ForumPostViewModel>(viewModel);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<ForumPostViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<ForumPostViewModel>> GetTopicReplies(Guid currentUserId, bool currentUserIsAdmin, GetForumTopicRepliesRequestViewModel viewModel)
        {
            try
            {
                OperationResultVo<ViewModels.PlatformSetting.PlatformSettingViewModel> itemsPerPageSetting = await platformSettingAppService.GetByElement(currentUserId, PlatformSettingElement.ForumPageSize);

                GetForumTopicRepliesQuery query = new GetForumTopicRepliesQuery
                {
                    TopicId = viewModel.TopicId,
                    Count = viewModel.Count ?? int.Parse(itemsPerPageSetting.Value.Value),
                    Page = viewModel.Page ?? 1,
                    Latest = viewModel.Latest
                };

                ForumTopicReplyListVo queryResult = await mediator.Query<GetForumTopicRepliesQuery, ForumTopicReplyListVo>(query);

                List<ForumPostViewModel> vms = mapper.Map<IEnumerable<ForumPost>, IEnumerable<ForumPostViewModel>>(queryResult.Replies).ToList();

                List<Guid> profilesToGet = vms.Select(x => x.UserId).ToList();
                IEnumerable<Guid> repliesProfilesToGet = vms.Where(x => x.ReplyUserId.HasValue).Select(x => x.ReplyUserId.Value);
                profilesToGet.AddRange(repliesProfilesToGet);

                IEnumerable<UserProfileEssentialVo> userProfiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(profilesToGet));

                HtmlSanitizer sanitizer = ContentHelper.GetHtmlSanitizer();

                foreach (ForumPostViewModel topicReply in vms)
                {
                    ForumPost entity = queryResult.Replies.First(x => x.Id == topicReply.Id);

                    SetVotes(currentUserId, topicReply, entity);

                    SetProfiles(topicReply, userProfiles);

                    SetPermissions(currentUserId, currentUserIsAdmin, topicReply);

                    SanitizeHtmlToDisplay(topicReply, sanitizer);
                }

                OperationResultListVo<ForumPostViewModel> result = new OperationResultListVo<ForumPostViewModel>(vms)
                {
                    Pagination = queryResult.Pagination
                };

                return result;
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<ForumPostViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<ForumPostViewModel>> RemovePost(Guid currentUserId, Guid id)
        {
            try
            {
                CommandResult<ForumPost> deletedPost = await mediator.SendCommand<DeleteForumPostCommand, ForumPost>(new DeleteForumPostCommand(currentUserId, id));

                ForumPostViewModel viewModel = mapper.Map<ForumPostViewModel>(deletedPost.Result);

                ForumCategory category = await mediator.Query<GetForumCategoryByIdQuery, ForumCategory>(new GetForumCategoryByIdQuery(viewModel.ForumCategoryId));

                viewModel.CategoryHandler = category.Handler;

                return new OperationResultVo<ForumPostViewModel>(viewModel, true, "That Forum Post is gone now!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo<ForumPostViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<int>> Vote(Guid currentUserId, Guid postId, VoteValue voteValue)
        {
            try
            {
                CommandResult<int> result = await mediator.SendCommand<SaveForumPostVoteCommand, int>(new SaveForumPostVoteCommand(currentUserId, postId, voteValue));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<int>(message);
                }

                return new OperationResultVo<int>(result.Result, true, "You have voted!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo<int>(ex.Message);
            }
        }

        private async Task<List<ForumCategoryListItemVo>> GetCategoryList(List<SupportedLanguage> languages)
        {
            IEnumerable<ForumCategory> allModels = await mediator.Query<GetForumCategoryQuery, IEnumerable<ForumCategory>>(new GetForumCategoryQuery());

            List<ForumCategoryListItemVo> vms = mapper.Map<IEnumerable<ForumCategory>, IEnumerable<ForumCategoryListItemVo>>(allModels).ToList();

            IEnumerable<ForumCategoryCounterResultVo> categoryCounters = await mediator.Query<GetForumCategoryCountersQuery, IEnumerable<ForumCategoryCounterResultVo>>(new GetForumCategoryCountersQuery(languages));

            List<Guid> profilesToGet = new List<Guid>();

            foreach (ForumCategoryListItemVo category in vms)
            {
                ForumCategoryCounterResultVo categoryStats = categoryCounters.FirstOrDefault(x => x.ForumCategoryId == category.Id);
                if (categoryStats != null)
                {
                    category.PostCount = categoryStats.PostsCount;
                    category.TopicCount = categoryStats.TopicsCount;
                    category.LatestForumPost = categoryStats.LatestPost;
                }

                if (category.LatestForumPost != null)
                {
                    profilesToGet.Add(category.LatestForumPost.UserId);
                }
            }

            IEnumerable<UserProfileEssentialVo> userProfiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(profilesToGet));

            foreach (ForumCategoryListItemVo category in vms)
            {
                if (category.LatestForumPost != null)
                {
                    UserProfileEssentialVo latestPostAuthorProfile = userProfiles.FirstOrDefault(x => x.UserId == category.LatestForumPost.UserId);

                    if (latestPostAuthorProfile != null)
                    {
                        category.LatestForumPost.UserHandler = latestPostAuthorProfile.Handler;
                        category.LatestForumPost.AuthorName = latestPostAuthorProfile.Name;
                        category.LatestForumPost.AuthorPicture = UrlFormatter.ProfileImage(category.LatestForumPost.UserId, Constants.SmallAvatarSize);
                    }
                }
            }

            return vms;
        }

        private static void SetVotes(Guid currentUserId, ForumPostViewModel forumPost, ForumPost entity)
        {
            forumPost.Score = (entity.Votes != null ? entity.Votes.Sum(x => (int)x.VoteValue) : 0);
            forumPost.CurrentUserVote = entity.Votes?.FirstOrDefault(x => x.UserId == currentUserId)?.VoteValue ?? VoteValue.Neutral;
        }

        private static void SetProfiles(ForumPostListItemVo forumPost, IEnumerable<UserProfileEssentialVo> userProfiles)
        {
            UserProfileEssentialVo authorProfile = userProfiles.FirstOrDefault(x => x.UserId == forumPost.UserId);
            if (authorProfile != null)
            {
                forumPost.AuthorName = authorProfile.Name;
                forumPost.UserHandler = authorProfile.Handler;
                forumPost.AuthorPicture = UrlFormatter.ProfileImage(authorProfile.UserId, Constants.SmallAvatarSize);
            }
        }

        private void SetProfiles(ForumPostViewModel topicReply, IEnumerable<UserProfileEssentialVo> userProfiles)
        {
            UserProfileEssentialVo authorProfile = userProfiles.FirstOrDefault(x => x.UserId == topicReply.UserId);
            if (authorProfile != null)
            {
                topicReply.AuthorName = authorProfile.Name;
                topicReply.UserHandler = authorProfile.Handler;
                topicReply.AuthorPicture = UrlFormatter.ProfileImage(authorProfile.UserId, Constants.SmallAvatarSize);
            }

            if (topicReply.ReplyUserId.HasValue)
            {
                UserProfileEssentialVo replyProfile = userProfiles.FirstOrDefault(x => x.UserId == topicReply.ReplyUserId);
                if (replyProfile != null)
                {
                    topicReply.ReplyAuthorName = replyProfile.Name;
                }
            }
        }

        private void SetPermissions(Guid currentUserId, bool currentUserIsAdmin, ForumPostViewModel viewModel)
        {
            SetBasePermissions(currentUserId, currentUserIsAdmin, viewModel);
        }

        private static void SanitizeHtmlToDisplay(ForumPostViewModel viewModel, HtmlSanitizer sanitizer)
        {
            viewModel.Content = sanitizer.Sanitize(viewModel.Content, Constants.DefaultLuduStackPath);

            System.Collections.Specialized.StringDictionary replacements = ContentFormatter.DisplayReplacements();

            foreach (string key in replacements.Keys)
            {
                viewModel.Content = viewModel.Content.Replace(key, replacements[key]);
            }
        }

        private static string SanitizeHtmlToSave(string content, HtmlSanitizer sanitizer)
        {
            content = sanitizer.Sanitize(content, Constants.DefaultLuduStackPath);

            System.Collections.Specialized.StringDictionary replacements = ContentFormatter.SaveReplacements();

            foreach (string key in replacements.Keys)
            {
                content = content.Replace(key, replacements[key]);
            }

            return content;
        }
    }
}