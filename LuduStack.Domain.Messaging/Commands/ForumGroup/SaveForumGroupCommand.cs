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
    public class SaveForumGroupCommand : BaseCommand
    {
        public ForumGroup ForumGroup { get; }

        public SaveForumGroupCommand(ForumGroup gamificationLevel) : base(gamificationLevel.Id)
        {
            ForumGroup = gamificationLevel;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveForumGroupCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveForumGroupCommandHandler : CommandHandler, IRequestHandler<SaveForumGroupCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IForumGroupRepository forumGroupRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveForumGroupCommandHandler(IUnitOfWork unitOfWork, IForumGroupRepository forumGroupRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.forumGroupRepository = forumGroupRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveForumGroupCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            if (request.ForumGroup.Id == Guid.Empty)
            {
                await forumGroupRepository.Add(request.ForumGroup);
            }
            else
            {
                forumGroupRepository.Update(request.ForumGroup);
            }

            result.Validation = await Commit(unitOfWork);

            return result;
        }
    }
}