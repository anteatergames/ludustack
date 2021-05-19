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
    public class LeaveCourseCommand : BaseUserCommand
    {
        public LeaveCourseCommand(Guid userId, Guid courseId) : base(userId, courseId)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new LeaveCourseCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class LeaveCourseCommandHandler : CommandHandler, IRequestHandler<LeaveCourseCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IStudyCourseRepository studyCourseRepository;

        public LeaveCourseCommandHandler(IUnitOfWork unitOfWork, IStudyCourseRepository studyCourseRepository)
        {
            this.unitOfWork = unitOfWork;
            this.studyCourseRepository = studyCourseRepository;
        }

        public async Task<CommandResult> Handle(LeaveCourseCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            StudyCourse course = await studyCourseRepository.GetById(request.Id);
            if (course is null)
            {
                AddError("The course doesn't exist.");
                return request.Result;
            }

            bool userAlreadyEnroled = studyCourseRepository.CheckStudentEnrolled(request.Id, request.UserId);
            if (!userAlreadyEnroled)
            {
                AddError("User not enrolled to this course.");
                return request.Result;
            }

            await studyCourseRepository.RemoveStudent(request.Id, request.UserId);

            request.Result.Validation = await Commit(unitOfWork);

            return request.Result;
        }
    }
}