using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DeleteLocalizationCommand : BaseUserCommand
    {
        public DeleteLocalizationCommand(Guid userId, Guid id) : base(userId, id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DeleteLocalizationCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class DeleteLocalizationCommandHandler : CommandHandler, IRequestHandler<DeleteLocalizationCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly ILocalizationRepository localizationRepository;

        public DeleteLocalizationCommandHandler(IUnitOfWork unitOfWork, ILocalizationRepository localizationRepository)
        {
            this.unitOfWork = unitOfWork;
            this.localizationRepository = localizationRepository;
        }

        public async Task<CommandResult> Handle(DeleteLocalizationCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            Models.Localization localization = await localizationRepository.GetById(request.Id);

            if (localization is null)
            {
                AddError("The Localization doesn't exist.");
                return request.Result;
            }

            // AddDomainEvent here

            localizationRepository.Remove(localization.Id);

            FluentValidation.Results.ValidationResult validation = await Commit(unitOfWork);

            request.Result.Validation = validation;

            return request.Result;
        }
    }
}