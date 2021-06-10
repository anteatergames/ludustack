using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DeleteForumCategoryCommand : BaseUserCommand
    {
        public DeleteForumCategoryCommand(Guid userId, Guid id) : base(userId, id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DeleteForumCategoryCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class DeleteForumCategoryCommandHandler : CommandHandler, IRequestHandler<DeleteForumCategoryCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IForumCategoryRepository forumCategoryRepository;

        public DeleteForumCategoryCommandHandler(IUnitOfWork unitOfWork, IForumCategoryRepository forumCategoryRepository)
        {
            this.unitOfWork = unitOfWork;
            this.forumCategoryRepository = forumCategoryRepository;
        }

        public async Task<CommandResult> Handle(DeleteForumCategoryCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            Models.ForumCategory category = await forumCategoryRepository.GetById(request.Id);

            if (category is null)
            {
                AddError("The Forum Category doesn't exist.");
                return request.Result;
            }

            // AddDomainEvent here

            forumCategoryRepository.Remove(category.Id);

            FluentValidation.Results.ValidationResult validation = await Commit(unitOfWork);

            request.Result.Validation = validation;

            return request.Result;
        }
    }
}