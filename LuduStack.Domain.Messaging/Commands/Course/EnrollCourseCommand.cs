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
    public class EnrollCourseCommand : BaseUserCommand
    {
        public EnrollCourseCommand(Guid userId, Guid courseId) : base(userId, courseId)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new EnrollCourseCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class EnrollCourseCommandHandler : CommandHandler, IRequestHandler<EnrollCourseCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IStudyCourseRepository studyCourseRepository;

        public EnrollCourseCommandHandler(IUnitOfWork unitOfWork, IStudyCourseRepository studyCourseRepository)
        {
            this.unitOfWork = unitOfWork;
            this.studyCourseRepository = studyCourseRepository;
        }

        public async Task<CommandResult> Handle(EnrollCourseCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            StudyCourse course = await studyCourseRepository.GetById(request.Id);
            if (course is null)
            {
                AddError("The course doesn't exist.");
                return request.Result;
            }

            bool userAlreadyEnroled = studyCourseRepository.CheckStudentEnrolled(request.Id, request.UserId);
            if (userAlreadyEnroled)
            {
                AddError("User already enrolled to this course");
                return request.Result;
            }

            CourseMember student = new CourseMember
            {
                UserId = request.UserId
            };

            await studyCourseRepository.AddStudent(request.Id, student);

            request.Result.Validation = await Commit(unitOfWork);

            return request.Result;
        }
    }
}