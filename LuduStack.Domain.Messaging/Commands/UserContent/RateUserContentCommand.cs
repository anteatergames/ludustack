using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.UserContent;
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
    public class RateUserContentCommand : BaseUserCommand
    {
        public decimal Score { get; }

        public RateUserContentCommand(Guid userId, Guid id, decimal score) : base(userId, id)
        {
            Score = score;
        }

        public override bool IsValid()
        {
            Result.Validation = new RateUserContentCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class RateUserContentCommandHandler : CommandHandler, IRequestHandler<RateUserContentCommand, CommandResult>
    {
        private readonly IMediatorHandler mediator;
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IUserContentRepository repository;

        public RateUserContentCommandHandler(IMediatorHandler mediator, IUnitOfWork unitOfWork, IUserContentRepository repository)
        {
            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
            this.repository = repository;
        }

        public async Task<CommandResult> Handle(RateUserContentCommand request, CancellationToken cancellationToken)
        {
            UserContentRating rating;

            if (!request.IsValid()) { return request.Result; }

            UserContent userContent = await repository.GetById(request.Id);
            if (userContent is null)
            {
                AddError("The content doesn't exist.");
                return request.Result;
            }

            IEnumerable<UserContentRating> existing = await mediator.Query<GetRatingsQuery, IEnumerable<UserContentRating>>(new GetRatingsQuery(x => x.Id == request.Id));

            bool alreadyRated = existing.Any(x => x.UserId == request.UserId);
            if (alreadyRated)
            {
                rating = existing.First(x => x.UserId == request.UserId);

                rating.Score = request.Score;

                await repository.UpdateRating(request.Id, rating);
            }
            else
            {
                rating = new UserContentRating
                {
                    Id = request.Id,
                    UserId = request.UserId,
                    Score = request.Score
                };

                await repository.AddRating(request.Id, rating);
            }

            request.Result.Validation = await Commit(unitOfWork);

            return request.Result;
        }
    }
}