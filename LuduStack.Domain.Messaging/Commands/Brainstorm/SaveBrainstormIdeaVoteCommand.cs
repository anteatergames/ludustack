using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Messaging.Queries.Brainstorm;
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
    public class SaveBrainstormIdeaVoteCommand : BaseUserCommand
    {
        public VoteValue Vote { get; }

        public SaveBrainstormIdeaVoteCommand(Guid userId, Guid ideaId, VoteValue vote) : base(userId, ideaId)
        {
            Vote = vote;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveBrainstormIdeaVoteCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveBrainstormIdeaVoteCommandHandler : CommandHandler, IRequestHandler<SaveBrainstormIdeaVoteCommand, CommandResult>
    {
        protected readonly IMediatorHandler mediator;
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IBrainstormIdeaRepository brainstormIdeaRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveBrainstormIdeaVoteCommandHandler(IMediatorHandler mediator, IUnitOfWork unitOfWork, IBrainstormIdeaRepository brainstormIdeaRepository, IGamificationDomainService gamificationDomainService)
        {
            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
            this.brainstormIdeaRepository = brainstormIdeaRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveBrainstormIdeaVoteCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;
            int pointsEarned = 0;

            if (!request.IsValid()) { return request.Result; }

            UserVoteVo model;
            BrainstormIdea idea = await mediator.Query<GetBrainstormIdeaByIdQuery, BrainstormIdea>(new GetBrainstormIdeaByIdQuery(request.Id));

            UserVoteVo existing = idea.Votes.FirstOrDefault(x => x.UserId == request.UserId);
            if (existing == null)
            {
                model = new UserVoteVo
                {
                    UserId = request.UserId,
                    CreateDate = DateTime.Now,
                    VoteValue = request.Vote
                };

                await brainstormIdeaRepository.AddVote(request.Id, model);
            }
            else
            {
                model = existing;
                model.VoteValue = request.Vote;

                await brainstormIdeaRepository.UpdateVote(request.Id, model);
            }

            result.Validation = await Commit(unitOfWork);
            result.PointsEarned = pointsEarned;

            return result;
        }
    }
}