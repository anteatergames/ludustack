using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.UserContent;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class AddCommentUserContentCommand : BaseUserCommand
    {
        public Guid? ParentCommentId { get; set; }

        public string Text { get; set; }

        public AddCommentUserContentCommand(Guid userId, Guid userContentId, Guid? parentCommentId, string text) : base(userId, userContentId)
        {
            ParentCommentId = parentCommentId;
            Text = text;
        }

        public override bool IsValid()
        {
            Result.Validation = new AddCommentUserContentCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class AddCommentUserContentCommandHandler : CommandHandler, IRequestHandler<AddCommentUserContentCommand, CommandResult>
    {
        private readonly IMediatorHandler mediator;
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IUserContentRepository repository;

        public AddCommentUserContentCommandHandler(IMediatorHandler mediator, IUnitOfWork unitOfWork, IUserContentRepository repository)
        {
            this.mediator = mediator;
            this.unitOfWork = unitOfWork;
            this.repository = repository;
        }

        public async Task<CommandResult> Handle(AddCommentUserContentCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            UserContent userContent = await repository.GetById(request.Id);
            if (userContent is null)
            {
                AddError("The content doesn't exist.");

                return new CommandResult(ValidationResult);
            }

            bool commentAlreadyExists = await mediator.Query<CheckIfCommentExistsQuery, bool>(new CheckIfCommentExistsQuery(x => x.UserContentId == request.Id && x.UserId == request.UserId && x.Text.Equals(request.Text)));

            if (commentAlreadyExists)
            {
                AddError("Duplicated Comment");

                return new CommandResult(ValidationResult);
            }

            UserContentComment model = new UserContentComment
            {
                UserContentId = request.Id,
                ParentCommentId = request.ParentCommentId,
                Text = request.Text,
                UserId = request.UserId
            };

            await repository.AddComment(model);

            request.Result.Validation = await Commit(unitOfWork);

            return request.Result;
        }
    }
}