using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.GameJam;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class VoteGameJamEntryCommand : BaseUserCommand
    {
        public GameJamCriteriaType CriteriaType { get; set; }

        public decimal Score { get; set; }

        public string Comment { get; set; }

        public bool IsCommunityVote { get; set; }

        public VoteGameJamEntryCommand(Guid userId, Guid id, GameJamCriteriaType criteriaType, decimal score, string comment, bool isCommunityVote) : base(userId, id)
        {
            CriteriaType = criteriaType;
            Score = score;
            Comment = comment;
            IsCommunityVote = isCommunityVote;
        }

        public override bool IsValid()
        {
            Result.Validation = new RateGameJamEntryCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class VoteGameJamEntryCommandHandler : CommandHandler, IRequestHandler<VoteGameJamEntryCommand, CommandResult>
    {
        private readonly IMediatorHandler mediator;
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IGameJamEntryRepository repository;

        public VoteGameJamEntryCommandHandler(IMediatorHandler mediator, IUnitOfWork unitOfWork, IGameJamEntryRepository repository)
        {
            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
            this.repository = repository;
        }

        public async Task<CommandResult> Handle(VoteGameJamEntryCommand request, CancellationToken cancellationToken)
        {
            GameJamVote vote;

            if (!request.IsValid()) { return request.Result; }

            GameJamEntry entry = await repository.GetById(request.Id);
            if (entry is null)
            {
                AddError("Entry not found.");
                return request.Result;
            }

            if (request.CriteriaType == 0)
            {
                request.CriteriaType = GameJamCriteriaType.Overall;
            }

            IEnumerable<GameJamVote> existing = await mediator.Query<GetGameJamEntryVotesQuery, IEnumerable<GameJamVote>>(new GetGameJamEntryVotesQuery(x => x.Id == request.Id));

            GameJamVote existingVote = existing.FirstOrDefault(x => x.UserId == request.UserId && x.CriteriaType == request.CriteriaType);
            if (existingVote != null)
            {
                existingVote.Score = request.Score;

                await repository.UpdateRating(request.Id, existingVote);
            }
            else
            {
                vote = new GameJamVote
                {
                    UserId = request.UserId,
                    CriteriaType = request.CriteriaType,
                    Score = request.Score,
                    Comment = request.Comment,
                    IsCommunityVote = request.IsCommunityVote
                };

                await repository.AddRating(request.Id, vote);
            }

            request.Result.Validation = await Commit(unitOfWork);

            return request.Result;
        }
    }
}