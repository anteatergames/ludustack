using AutoMapper.QueryableExtensions;
using LuduStack.Application.Formatters;
using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels;
using LuduStack.Application.ViewModels.Brainstorm;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Messaging;
using LuduStack.Domain.Messaging.Queries.Brainstorm;
using LuduStack.Domain.Messaging.Queries.UserProfile;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuduStack.Application.Services
{
    public class BrainstormAppService : ProfileBaseAppService, IBrainstormAppService
    {
        public BrainstormAppService(IProfileBaseAppServiceCommon profileBaseAppServiceCommon) : base(profileBaseAppServiceCommon)
        {
        }

        public async Task<OperationResultVo<int>> Count(Guid currentUserId)
        {
            try
            {
                int count = await mediator.Query<CountBrainstormSessionQuery, int>(new CountBrainstormSessionQuery());

                return new OperationResultVo<int>(count);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<int>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<Guid>> GetAllIds(Guid currentUserId)
        {
            try
            {
                IEnumerable<Guid> allIds = await mediator.Query<GetBrainstormSessionIdsQuery, IEnumerable<Guid>>(new GetBrainstormSessionIdsQuery());

                return new OperationResultListVo<Guid>(allIds);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<Guid>(ex.Message);
            }
        }

        public async Task<OperationResultVo<BrainstormIdeaViewModel>> GetById(Guid currentUserId, Guid id)
        {
            try
            {
                BrainstormIdea idea = await mediator.Query<GetBrainstormIdeaByIdQuery, BrainstormIdea>(new GetBrainstormIdeaByIdQuery(id));

                BrainstormSession session = await mediator.Query<GetBrainstormSessionByIdQuery, BrainstormSession>(new GetBrainstormSessionByIdQuery(idea.SessionId));

                List<Guid> userIdList = idea.Comments.Select(x => x.UserId).ToList();
                userIdList.Add(idea.UserId);

                IEnumerable<UserProfileEssentialVo> profiles = await mediator.Query<GetBasicUserProfileDataByUserIdsQuery, IEnumerable<UserProfileEssentialVo>>(new GetBasicUserProfileDataByUserIdsQuery(userIdList));

                BrainstormIdeaViewModel vm = mapper.Map<BrainstormIdeaViewModel>(idea);

                vm.UserContentType = UserContentType.Idea;
                vm.VoteCount = idea.Votes.Count;
                vm.Score = idea.Votes.Sum(x => (int)x.VoteValue);
                vm.CurrentUserVote = idea.Votes.FirstOrDefault(x => x.UserId == currentUserId)?.VoteValue ?? VoteValue.Neutral;

                SetAuthorDetails(currentUserId, vm, profiles);

                vm.CommentCount = idea.Comments.Count;

                IQueryable<CommentViewModel> commentsVm = idea.Comments.AsQueryable().ProjectTo<CommentViewModel>(mapper.ConfigurationProvider);

                vm.Comments = commentsVm.OrderBy(x => x.CreateDate).ToList();

                foreach (CommentViewModel comment in vm.Comments)
                {
                    UserProfileEssentialVo commenterProfile = profiles.FirstOrDefault(x => x.UserId == comment.UserId);
                    if (commenterProfile == null)
                    {
                        comment.AuthorName = Constants.UnknownSoul;
                    }
                    else
                    {
                        comment.AuthorName = commenterProfile.Name;
                        comment.UserHandler = commenterProfile.Handler;
                    }

                    comment.AuthorPicture = UrlFormatter.ProfileImage(comment.UserId);
                    comment.Text = string.IsNullOrWhiteSpace(comment.Text) ? Constants.SoundOfSilence : comment.Text;
                }

                vm.Permissions.CanEdit = currentUserId == session?.UserId;

                return new OperationResultVo<BrainstormIdeaViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<BrainstormIdeaViewModel>(ex.Message);
            }
        }

        public Task<OperationResultVo> Remove(Guid currentUserId, Guid id)
        {
            return Task.FromResult(new OperationResultVo("Not Implemented"));
        }

        public async Task<OperationResultVo<Guid>> Save(Guid currentUserId, BrainstormIdeaViewModel viewModel)
        {
            try
            {
                BrainstormSession session = await mediator.Query<GetBrainstormSessionByIdQuery, BrainstormSession>(new GetBrainstormSessionByIdQuery(viewModel.SessionId));

                BrainstormIdea model;

                BrainstormIdea existing = await mediator.Query<GetBrainstormIdeaByIdQuery, BrainstormIdea>(new GetBrainstormIdeaByIdQuery(viewModel.Id));
                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<BrainstormIdea>(viewModel);
                }

                model.SessionId = session.Id;

                CommandResult result = await mediator.SendCommand(new SaveBrainstormIdeaCommand(currentUserId, model));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                return new OperationResultVo<Guid>(model.Id, result.PointsEarned);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public async Task<OperationResultVo> Vote(Guid userId, Guid ideaId, VoteValue voteValue)
        {
            try
            {
                CommandResult result = await mediator.SendCommand(new SaveBrainstormIdeaVoteCommand(userId, ideaId, voteValue));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo(false, message);
                }

                return new OperationResultVo(true, "You have voted!");
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }

        public async Task<OperationResultVo> Comment(Guid currentUserId, CommentViewModel vm)
        {
            try
            {
                await SetAuthorDetails(currentUserId, vm);
                BrainstormIdea idea = await mediator.Query<GetBrainstormIdeaByIdQuery, BrainstormIdea>(new GetBrainstormIdeaByIdQuery(vm.UserContentId));
                BrainstormComment model = new BrainstormComment
                {
                    UserId = vm.UserId,
                    IdeaId = vm.UserContentId,
                    SessionId = idea.SessionId,
                    Text = vm.Text,
                    AuthorName = vm.AuthorName,
                    AuthorPicture = vm.AuthorPicture
                };

                CommandResult result = await mediator.SendCommand(new AddBrainstormIdeaCommentCommand(vm.UserId, model));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                return new OperationResultVo<Guid>(model.IdeaId, 0);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public async Task<OperationResultVo<BrainstormSessionViewModel>> GetSession(Guid sessionId)
        {
            try
            {
                BrainstormSession main = await mediator.Query<GetBrainstormSessionByIdQuery, BrainstormSession>(new GetBrainstormSessionByIdQuery(sessionId));

                BrainstormSessionViewModel vm = mapper.Map<BrainstormSessionViewModel>(main);

                return new OperationResultVo<BrainstormSessionViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<BrainstormSessionViewModel>(ex.Message);
            }
        }

        /// <summary>
        /// Get the most recent session of type
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<OperationResultVo<BrainstormSessionViewModel>> GetSession(Guid userId, BrainstormSessionType type)
        {
            try
            {
                IEnumerable<BrainstormSession> allModels = await mediator.Query<GetBrainstormSessionQuery, IEnumerable<BrainstormSession>>(new GetBrainstormSessionQuery(x => x.Type == type));
                BrainstormSession model = allModels.LastOrDefault(x => x.Type == type);

                BrainstormSessionViewModel vm = mapper.Map<BrainstormSessionViewModel>(model);

                return new OperationResultVo<BrainstormSessionViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<BrainstormSessionViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<BrainstormSessionViewModel>> GetSessions(Guid userId)
        {
            try
            {
                IEnumerable<BrainstormSession> allModels = await mediator.Query<GetBrainstormSessionQuery, IEnumerable<BrainstormSession>>(new GetBrainstormSessionQuery());

                IEnumerable<BrainstormSessionViewModel> vms = mapper.Map<IEnumerable<BrainstormSession>, IEnumerable<BrainstormSessionViewModel>>(allModels);

                vms = vms.OrderBy(x => x.Type).ThenBy(x => x.CreateDate);

                return new OperationResultListVo<BrainstormSessionViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<BrainstormSessionViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<Guid>> SaveSession(Guid currentUserId, BrainstormSessionViewModel viewModel)
        {
            int pointsEarned = 0;

            try
            {
                BrainstormSession model;

                BrainstormSession existing = await mediator.Query<GetBrainstormSessionByIdQuery, BrainstormSession>(new GetBrainstormSessionByIdQuery(viewModel.Id));

                if (existing != null)
                {
                    model = mapper.Map(viewModel, existing);
                }
                else
                {
                    model = mapper.Map<BrainstormSession>(viewModel);
                }

                CommandResult result = await mediator.SendCommand(new SaveBrainstormSessionCommand(currentUserId, model));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo<Guid>(model.Id, false, message);
                }

                pointsEarned += result.PointsEarned;

                return new OperationResultVo<Guid>(model.Id, pointsEarned);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<Guid>(ex.Message);
            }
        }

        public async Task<OperationResultListVo<BrainstormIdeaViewModel>> GetAllBySessionId(Guid userId, Guid sessionId, int? filter)
        {
            try
            {
                IEnumerable<BrainstormIdea> allIdeas = await mediator.Query<GetBrainstormIdeaQuery, IEnumerable<BrainstormIdea>>(new GetBrainstormIdeaQuery(x => x.SessionId == sessionId, filter));

                IEnumerable<BrainstormIdeaViewModel> vms = mapper.Map<IEnumerable<BrainstormIdea>, IEnumerable<BrainstormIdeaViewModel>>(allIdeas);

                foreach (BrainstormIdeaViewModel idea in vms)
                {
                    idea.UserContentType = UserContentType.Idea;
                    idea.VoteCount = idea.Votes.Count;
                    idea.Score = idea.Votes.Sum(x => (int)x.VoteValue);
                    idea.CurrentUserVote = idea.Votes.FirstOrDefault(x => x.UserId == userId)?.VoteValue ?? VoteValue.Neutral;

                    idea.CommentCount = idea.Comments.Count;
                }

                vms = vms.OrderByDescending(x => x.Score).ThenByDescending(x => x.Status).ThenByDescending(x => x.CreateDate);

                return new OperationResultListVo<BrainstormIdeaViewModel>(vms);
            }
            catch (Exception ex)
            {
                return new OperationResultListVo<BrainstormIdeaViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo<BrainstormSessionViewModel>> GetMainSession()
        {
            try
            {
                IEnumerable<BrainstormSession> allModels = await mediator.Query<GetBrainstormSessionQuery, IEnumerable<BrainstormSession>>(new GetBrainstormSessionQuery(x => x.Type == BrainstormSessionType.Main));
                BrainstormSession main = allModels.LastOrDefault();

                BrainstormSessionViewModel vm = mapper.Map<BrainstormSessionViewModel>(main);

                return new OperationResultVo<BrainstormSessionViewModel>(vm);
            }
            catch (Exception ex)
            {
                return new OperationResultVo<BrainstormSessionViewModel>(ex.Message);
            }
        }

        public async Task<OperationResultVo> ChangeStatus(Guid currentUserId, Guid ideaId, BrainstormIdeaStatus selectedStatus)
        {
            try
            {
                BrainstormIdea idea = await mediator.Query<GetBrainstormIdeaByIdQuery, BrainstormIdea>(new GetBrainstormIdeaByIdQuery(ideaId));

                if (idea == null)
                {
                    return new OperationResultVo("Idea not found!");
                }

                idea.Status = selectedStatus;

                CommandResult result = await mediator.SendCommand(new SaveBrainstormIdeaCommand(currentUserId, idea));

                if (!result.Validation.IsValid)
                {
                    string message = result.Validation.Errors.FirstOrDefault().ErrorMessage;
                    return new OperationResultVo(false, message);
                }

                return new OperationResultVo(true, result.PointsEarned);
            }
            catch (Exception ex)
            {
                return new OperationResultVo(ex.Message);
            }
        }
    }
}