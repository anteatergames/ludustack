using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Messaging.Queries.ForumPost;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class SaveForumPostVoteCommand : BaseUserCommand<int>
    {
        public VoteValue Vote { get; }

        public SaveForumPostVoteCommand(Guid userId, Guid ideaId, VoteValue vote) : base(userId, ideaId)
        {
            Vote = vote;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveForumPostVoteCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveForumPostVoteCommandHandler : CommandHandler, IRequestHandler<SaveForumPostVoteCommand, CommandResult<int>>
    {
        protected readonly IMediatorHandler mediator;
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IForumPostRepository forumPostRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveForumPostVoteCommandHandler(IMediatorHandler mediator, IUnitOfWork unitOfWork, IForumPostRepository forumPostRepository, IGamificationDomainService gamificationDomainService)
        {
            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
            this.forumPostRepository = forumPostRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult<int>> Handle(SaveForumPostVoteCommand request, CancellationToken cancellationToken)
        {
            CommandResult<int> result = request.Result;
            int pointsEarned = 0;

            if (!request.IsValid()) { return request.Result; }

            UserVoteVo model;
            ForumPost forumPost = await mediator.Query<GetForumPostByIdQuery, ForumPost>(new GetForumPostByIdQuery(request.Id));

            UserVoteVo existing = forumPost.Votes?.FirstOrDefault(x => x.UserId == request.UserId);
            if (existing == null)
            {
                model = new UserVoteVo
                {
                    UserId = request.UserId,
                    CreateDate = DateTime.Now,
                    VoteValue = request.Vote
                };

                await forumPostRepository.AddVote(request.Id, model);
            }
            else
            {
                model = existing;
                model.VoteValue = request.Vote;

                await forumPostRepository.UpdateVote(request.Id, model);
            }

            result.Validation = await Commit(unitOfWork);
            result.PointsEarned = pointsEarned;

            int newCount = await mediator.Query<GetForumPostScoreQuery, int>(new GetForumPostScoreQuery(request.Id));
            result.Result = newCount;

            return result;
        }
    }
}