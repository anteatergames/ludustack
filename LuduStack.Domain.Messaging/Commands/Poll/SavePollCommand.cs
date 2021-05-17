using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Interfaces.Services;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class SavePollCommand : BaseUserCommand
    {
        public Poll Poll { get; }

        public SavePollCommand(Guid userId, Poll poll) : base(userId, poll.Id)
        {
            Poll = poll;
        }

        public override bool IsValid()
        {
            Result.Validation = new SavePollCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SavePollCommandHandler : CommandHandler, IRequestHandler<SavePollCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IPollRepository pollRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SavePollCommandHandler(IUnitOfWork unitOfWork, IPollRepository pollRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.pollRepository = pollRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SavePollCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;
            int pointsEarned = 0;

            if (!request.IsValid()) { return request.Result; }

            if (request.Poll.Id == Guid.Empty)
            {
                await pollRepository.Add(request.Poll);
                pointsEarned += gamificationDomainService.ProcessAction(request.UserId, PlatformAction.PollPost);
            }
            else
            {
                pollRepository.Update(request.Poll);
            }

            result.Validation = await Commit(unitOfWork);
            result.PointsEarned = pointsEarned;

            return result;
        }
    }
}