using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class DeleteCourseCommand : BaseCommand
    {
        public DeleteCourseCommand(Guid id) : base(id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DeleteCourseCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class DeleteCourseCommandHandler : CommandHandler, IRequestHandler<DeleteCourseCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IStudyCourseRepository studyCourseRepository;

        public DeleteCourseCommandHandler(IUnitOfWork unitOfWork, IStudyCourseRepository studyCourseRepository)
        {
            this.unitOfWork = unitOfWork;
            this.studyCourseRepository = studyCourseRepository;
        }

        public async Task<CommandResult> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) return request.Result;

            var course = await studyCourseRepository.GetById(request.Id);

            if (course is null)
            {
                AddError("The course doesn't exists.");
                return request.Result;
            }

            // AddDomainEvent here

            studyCourseRepository.Remove(course.Id);

            var validation = await Commit(unitOfWork);

            request.Result.Validation = validation;

            return request.Result;
        }
    }
}