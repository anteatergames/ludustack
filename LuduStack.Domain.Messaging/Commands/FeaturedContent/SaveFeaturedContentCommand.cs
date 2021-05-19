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
    public class SaveFeaturedContentCommand : BaseUserCommand
    {
        public FeaturedContent FeaturedContent { get; }

        public SaveFeaturedContentCommand(Guid userId, FeaturedContent featuredContent) : base(userId, featuredContent.Id)
        {
            FeaturedContent = featuredContent;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveFeaturedContentCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveFeaturedContentCommandHandler : CommandHandler, IRequestHandler<SaveFeaturedContentCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IFeaturedContentRepository featuredContentRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveFeaturedContentCommandHandler(IUnitOfWork unitOfWork, IFeaturedContentRepository featuredContentRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.featuredContentRepository = featuredContentRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveFeaturedContentCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            if (request.FeaturedContent.Id == Guid.Empty)
            {
                await featuredContentRepository.Add(request.FeaturedContent);
            }
            else
            {
                featuredContentRepository.Update(request.FeaturedContent);
            }

            result.Validation = await Commit(unitOfWork);

            return result;
        }
    }
}