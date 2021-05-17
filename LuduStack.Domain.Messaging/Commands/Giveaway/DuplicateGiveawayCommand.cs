using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Extensions;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DuplicateGiveawayCommand : BaseUserCommand<Giveaway>
    {
        public DuplicateGiveawayCommand(Guid userId, Guid id) : base(userId, id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DuplicateGiveawayCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class DuplicateGiveawayCommandHandler : CommandHandler, IRequestHandler<DuplicateGiveawayCommand, CommandResult<Giveaway>>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IGiveawayRepository giveawayRepository;

        public DuplicateGiveawayCommandHandler(IUnitOfWork unitOfWork, IGiveawayRepository giveawayRepository)
        {
            this.unitOfWork = unitOfWork;
            this.giveawayRepository = giveawayRepository;
        }

        public async Task<CommandResult<Giveaway>> Handle(DuplicateGiveawayCommand request, CancellationToken cancellationToken)
        {
            CommandResult<Giveaway> result = request.Result;
            int pointsEarned = 0;

            if (!request.IsValid()) { return request.Result; }

            Giveaway model = await giveawayRepository.GetById(request.Id);

            Giveaway copy = model.Copy();

            copy.Id = Guid.Empty;

            copy.Name = string.Format("{0} (Copy {1})", copy.Name, DateTime.Now.ToString("yyyyMMddhhmmss"));

            copy.Status = GiveawayStatus.Draft;

            copy.StartDate = DateTime.Today.AddDays(1);
            copy.EndDate = copy.StartDate.AddDays(1);

            copy.Participants = new List<GiveawayParticipant>();

            await giveawayRepository.Add(copy);

            result.Validation = await Commit(unitOfWork);
            result.PointsEarned = pointsEarned;
            request.Result.Result = copy;

            return result;
        }
    }
}