using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class SaveForumCategoryCommand : BaseCommand
    {
        public ForumCategory ForumCategory { get; }

        public SaveForumCategoryCommand(ForumCategory gamificationLevel) : base(gamificationLevel.Id)
        {
            ForumCategory = gamificationLevel;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveForumCategoryCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveForumCategoryCommandHandler : CommandHandler, IRequestHandler<SaveForumCategoryCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IForumCategoryRepository forumCategoryRepository;

        public SaveForumCategoryCommandHandler(IUnitOfWork unitOfWork, IForumCategoryRepository forumCategoryRepository)
        {
            this.unitOfWork = unitOfWork;
            this.forumCategoryRepository = forumCategoryRepository;
        }

        public async Task<CommandResult> Handle(SaveForumCategoryCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            if (request.ForumCategory.Id == Guid.Empty)
            {
                await forumCategoryRepository.Add(request.ForumCategory);
            }
            else
            {
                forumCategoryRepository.Update(request.ForumCategory);
            }

            result.Validation = await Commit(unitOfWork);

            return result;
        }
    }
}