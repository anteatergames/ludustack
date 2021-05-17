using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Game;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class UnlikeGameCommand : BaseUserCommand<int>
    {
        public UnlikeGameCommand(Guid userId, Guid gameId) : base(userId, gameId)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new UnlikeGameCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class UnLikeGameCommandHandler : CommandHandler, IRequestHandler<UnlikeGameCommand, CommandResult<int>>
    {
        private readonly IMediatorHandler mediator;
        private readonly IUnitOfWork unitOfWork;
        private readonly IGameRepository gameRepository;

        public UnLikeGameCommandHandler(IMediatorHandler mediator, IUnitOfWork unitOfWork, IGameRepository gameRepository)
        {
            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
            this.gameRepository = gameRepository;
        }

        public async Task<CommandResult<int>> Handle(UnlikeGameCommand request, CancellationToken cancellationToken)
        {
            CommandResult<int> result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            await gameRepository.Unlike(request.UserId, request.Id);

            result.Validation = await Commit(unitOfWork);

            int newCount = await mediator.Query<CountGameLikesQuery, int>(new CountGameLikesQuery(request.Id));

            result.Result = newCount;

            return result;
        }
    }
}