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
    public class SaveCourseCommand : BaseUserCommand
    {
        public StudyCourse Course { get; }

        public SaveCourseCommand(Guid userId, StudyCourse course) : base(userId, course.Id)
        {
            Course = course;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveCourseCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SaveCourseCommandHandler : CommandHandler, IRequestHandler<SaveCourseCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IStudyCourseRepository studyCourseRepository;
        protected readonly IGamificationDomainService gamificationDomainService;

        public SaveCourseCommandHandler(IUnitOfWork unitOfWork, IStudyCourseRepository studyCourseRepository, IGamificationDomainService gamificationDomainService)
        {
            this.unitOfWork = unitOfWork;
            this.studyCourseRepository = studyCourseRepository;
            this.gamificationDomainService = gamificationDomainService;
        }

        public async Task<CommandResult> Handle(SaveCourseCommand request, CancellationToken cancellationToken)
        {
            CommandResult result = request.Result;

            if (!request.IsValid()) { return request.Result; }

            if (request.Course.Id == Guid.Empty)
            {
                await studyCourseRepository.Add(request.Course);
            }
            else
            {
                studyCourseRepository.Update(request.Course);
            }

            request.Result.Validation = await Commit(unitOfWork);

            return result;
        }
    }
}