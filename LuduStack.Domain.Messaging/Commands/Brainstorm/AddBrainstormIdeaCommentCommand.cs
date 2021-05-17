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
    public class AddBrainstormIdeaCommentCommand : BaseUserCommand
    {
        public BrainstormComment BrainstormComment { get; }

        public AddBrainstormIdeaCommentCommand(Guid userId, BrainstormComment brainstormComment) : base(userId, brainstormComment.IdeaId)
        {
            UserId = userId;
            BrainstormComment = brainstormComment;
        }

        public override bool IsValid()
        {
            Result.Validation = new AddBrainstormIdeaCommentCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class AddBrainstormIdeaCommentCommandHandler : CommandHandler, IRequestHandler<AddBrainstormIdeaCommentCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IBrainstormIdeaRepository brainstormIdeaRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public AddBrainstormIdeaCommentCommandHandler(IUnitOfWork unitOfWork, IBrainstormIdeaRepository brainstormIdeaRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.brainstormIdeaRepository = brainstormIdeaRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(AddBrainstormIdeaCommentCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;
            int pointsEarned = 0;

            if (!request.IsValid()) { return request.Result; }

            await brainstormIdeaRepository.AddComment(request.BrainstormComment);

            result.Validation = await Commit(unitOfWork);
            result.PointsEarned = pointsEarned;

            return result;
        }
    }
}