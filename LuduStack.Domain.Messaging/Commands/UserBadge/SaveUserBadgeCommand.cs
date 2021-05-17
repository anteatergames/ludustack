using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class SaveUserBadgeCommand : BaseUserCommand
    {
        public BadgeType BadgeType { get; }
        public Guid ReferenceId { get; }

        public SaveUserBadgeCommand(Guid userId, BadgeType badgeType, Guid referenceId) : base(userId)
        {
            BadgeType = badgeType;
            ReferenceId = referenceId;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveUserBadgeCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveUserBadgeCommandHandler : CommandHandler, IRequestHandler<SaveUserBadgeCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IUserBadgeRepository userBadgeRepository;

        public SaveUserBadgeCommandHandler(IUnitOfWork unitOfWork, IUserBadgeRepository userBadgeRepository)
        {
            this.unitOfWork = unitOfWork;
            this.userBadgeRepository = userBadgeRepository;
        }

        public async Task<CommandResult> Handle(SaveUserBadgeCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            IQueryable<UserBadge> items = userBadgeRepository.Get(x => x.UserId == request.UserId && x.Badge == request.BadgeType);

            UserBadge model = items.FirstOrDefault();
            if (model == null)
            {
                model = new UserBadge
                {
                    UserId = request.UserId,
                    Badge = request.BadgeType
                };
            }

            if (!model.References.Any(x => x == request.ReferenceId))
            {
                model.References.Add(request.ReferenceId);

                if (model.Id == Guid.Empty)
                {
                    await userBadgeRepository.Add(model);
                }
                else
                {
                    userBadgeRepository.Update(model);
                }
            }

            result.Validation = await Commit(unitOfWork);

            return result;
        }
    }
}