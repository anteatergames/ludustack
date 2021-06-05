using Ganss.XSS;
using LuduStack.Application.Formatters;
using LuduStack.Application.Helpers;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Forum;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Commands.Forum;
using LuduStack.Domain.Messaging.Queries.ForumCategory;
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

        public ForumAppService(IBaseAppServiceCommon baseAppServiceCommon, ILogger<ForumAppService> logger) : base(baseAppServiceCommon)
        {
            this.logger = logger;
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

        public async Task<OperationResultListVo<ForumCategoryListItemVo>> GetAllCategories(Guid currentUserId)
        {
            try
            {
                IEnumerable<ForumCategory> allModels = await mediator.Query<GetForumCategoryQuery, IEnumerable<ForumCategory>>(new GetForumCategoryQuery());

                List<ForumCategoryListItemVo> vms = mapper.Map<IEnumerable<ForumCategory>, IEnumerable<ForumCategoryListItemVo>>(allModels).ToList();

                IEnumerable<ForumCategoryCounterResultVo> categoryCounters = await mediator.Query<GetForumCategoryCountersQuery, IEnumerable<ForumCategoryCounterResultVo>>(new GetForumCategoryCountersQuery());

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
                            category.LatestForumPost.AuthorPicture = UrlFormatter.ProfileImage(category.LatestForumPost.UserId, 43);
                        }
                    }
                }

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

                return new OperationResultVo<ForumCategoryViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<ForumCategoryViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<ForumPostListItemVo>> GetPosts(Guid currentUserId, GetForumPostsRequestViewModel viewModel)
        {
            try
            {
                List<Guid> profilesToGet = new List<Guid>();

                GetForumPostsQueryOptions queryOptions = viewModel.ToQueryOptions();

                List<ForumPostListItemVo> allModels = await mediator.Query<GetForumPostListQuery, List<ForumPostListItemVo>>(new GetForumPostListQuery(queryOptions));

                IEnumerable<Guid> userIds = allModels.Select(x => x.UserId);
                profilesToGet.AddRange(userIds);

                IEnumerable<ForumPostCounterResultVo> postCounters = await mediator.Query<GetForumPostCountersQuery, IEnumerable<ForumPostCounterResultVo>>(new GetForumPostCountersQuery(viewModel.ForumCategoryId));

                foreach (ForumPostListItemVo forumPost in allModels)
                {
                    ForumPostCounterResultVo postStats = postCounters.FirstOrDefault(x => x.OriginalPostId == forumPost.Id);
                    if (postStats != null)
                    {
                        forumPost.AnswerCount = postStats.AnswerCount;
                        forumPost.ViewCount = postStats.ViewCount;
                        forumPost.LatestAnswer = postStats.LatestAnswer;
                    }

                    if (forumPost.LatestAnswer != null)
                    {
                        profilesToGet.Add(forumPost.LatestAnswer.UserId);
                    }
                }

                IEnumerable<UserProfileEssentialVo> userProfiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(profilesToGet));

                foreach (ForumPostListItemVo forumPost in allModels)
                {
                    SetProfiles(forumPost, userProfiles);

                    if (forumPost.LatestAnswer != null)
                    {
                        UserProfileEssentialVo latestPostAuthorProfile = userProfiles.FirstOrDefault(x => x.UserId == forumPost.LatestAnswer.UserId);

                        if (latestPostAuthorProfile != null)
                        {
                            forumPost.LatestAnswer.UserHandler = latestPostAuthorProfile.Handler;
                            forumPost.LatestAnswer.AuthorName = latestPostAuthorProfile.Name;
                            forumPost.LatestAnswer.AuthorPicture = UrlFormatter.ProfileImage(forumPost.LatestAnswer.UserId, 43);
                        }
                    }
                }

                return new OperationResultListVo<ForumPostListItemVo>(allModels);
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
                    //string title = existing.Title;
                    //var existingCreateDate = existing.CreateDate;
                    //model = mapper.Map(viewModel, existing);
                    //model.Title = title;
                }
                else
                {
                    model = mapper.Map<ForumPost>(viewModel);
                }

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

        public async Task<OperationResultVo<ForumPostViewModel>> GetPostForDetails(Guid currentUserId, Guid id)
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


                List<Guid> profilesToGet = new List<Guid> { viewModel.UserId };

                IEnumerable<UserProfileEssentialVo> userProfiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(profilesToGet));

                SetProfiles(viewModel, userProfiles);

                SetPermissions(currentUserId, viewModel);

                HtmlSanitizer sanitizer = ContentHelper.GetHtmlSanitizer();
                SanitizeHtml(viewModel, sanitizer);

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

        public async Task<OperationResultListVo<ForumPostViewModel>> GetTopicAnswers(Guid currentUserId, GetForumTopicAnswersRequestViewModel viewModel)
        {
            try
            {
                GetForumTopicAnswersQueryOptions queryOptions = viewModel.ToQueryOptions();

                List<ForumPost> allModels = await mediator.Query<GetForumTopicAnswersQuery, List<ForumPost>>(new GetForumTopicAnswersQuery(queryOptions));

                List<ForumPostViewModel> vms = mapper.Map<IEnumerable<ForumPost>, IEnumerable<ForumPostViewModel>>(allModels).ToList();

                IEnumerable<Guid> profilesToGet = vms.Select(x => x.UserId);

                IEnumerable<UserProfileEssentialVo> userProfiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(profilesToGet));

                HtmlSanitizer sanitizer = ContentHelper.GetHtmlSanitizer();

                foreach (ForumPostViewModel forumTopicAnswer in vms)
                {
                    SetProfiles(forumTopicAnswer, userProfiles);

                    SetPermissions(currentUserId, forumTopicAnswer);

                    SanitizeHtml(forumTopicAnswer, sanitizer);
                }

                return new OperationResultListVo<ForumPostViewModel>(vms);
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

        private static void SetProfiles(ForumPostListItemVo forumPost, IEnumerable<UserProfileEssentialVo> userProfiles)
        {
            UserProfileEssentialVo authorProfile = userProfiles.FirstOrDefault(x => x.UserId == forumPost.UserId);
            if (authorProfile != null)
            {
                forumPost.AuthorName = authorProfile.Name;
                forumPost.UserHandler = authorProfile.Handler;
                forumPost.AuthorPicture = UrlFormatter.ProfileImage(authorProfile.UserId, 43);
            }
        }

        private void SetProfiles(ForumPostViewModel forumTopicAnswer, IEnumerable<UserProfileEssentialVo> userProfiles)
        {
            UserProfileEssentialVo authorProfile = userProfiles.FirstOrDefault(x => x.UserId == forumTopicAnswer.UserId);
            if (authorProfile != null)
            {
                forumTopicAnswer.AuthorName = authorProfile.Name;
                forumTopicAnswer.UserHandler = authorProfile.Handler;
                forumTopicAnswer.AuthorPicture = UrlFormatter.ProfileImage(authorProfile.UserId, 43);
            }
        }

        private void SetPermissions(Guid currentUserId, ForumPostViewModel viewModel)
        {
            SetBasePermissions(currentUserId, viewModel);
        }

        private static void SanitizeHtml(ForumPostViewModel viewModel, HtmlSanitizer sanitizer)
        {
            viewModel.Content = sanitizer.Sanitize(viewModel.Content, Constants.DefaultLuduStackPath);

            viewModel.Content = viewModel.Content.Replace("<img src=", @"<img class=""img-fluid m-0"" src=");
            viewModel.Content = viewModel.Content.Replace("<div data-oembed-url=", @"<div class=""col-12 col-lg-8 col-xl-6 mx-auto"" data-oembed-url=");
            viewModel.Content = viewModel.Content.Replace("<figure ", @"<figure class=""mx-auto"" ");
            viewModel.Content = viewModel.Content.Replace("<figcaption>", @"<figcaption class=""text-center"">");
            viewModel.Content = viewModel.Content.Replace("<pre>", @"<pre class=""bg-light p-0 p-md-2 p-lg-3 p-xl-4"">");
            viewModel.Content = viewModel.Content.Replace("<iframe src=", @"<iframe frameBorder=""0"" src=");
        }
    }
}
