using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Domain.Messaging
{
    public class SavePlansCommand : BaseUserCommand
    {
        public Guid CourseId { get; }
        public List<StudyPlan> Plans { get; }

        public SavePlansCommand(Guid userId, Guid courseId) : base(userId, courseId)
        {
        }

        public SavePlansCommand(Guid userId, Guid courseId, List<StudyPlan> plans) : base(userId, courseId)
        {
            Plans = plans;
        }

        public override bool IsValid()
        {
            Result.Validation = new SavePlansCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }

    public class SavePlansCommandHandler : CommandHandler, IRequestHandler<SavePlansCommand, CommandResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IStudyCourseRepository studyCourseRepository;

        public SavePlansCommandHandler(IUnitOfWork unitOfWork, IStudyCourseRepository studyCourseRepository)
        {
            this.unitOfWork = unitOfWork;
            this.studyCourseRepository = studyCourseRepository;
        }

        public async Task<CommandResult> Handle(SavePlansCommand request, CancellationToken cancellationToken)
        {
            if (!request.IsValid()) { return request.Result; }

            List<StudyPlan> existingPlans = await studyCourseRepository.GetPlans(request.Id);

            foreach (StudyPlan plan in request.Plans)
            {
                StudyPlan existing = existingPlans.FirstOrDefault(x => x.Id == plan.Id);
                if (existing == null)
                {
                    await studyCourseRepository.AddPlan(request.Id, plan);
                }
                else
                {
                    existing.Name = plan.Name;
                    existing.Description = plan.Description;
                    existing.ScoreToPass = plan.ScoreToPass;
                    existing.Order = plan.Order;

                    await studyCourseRepository.UpdatePlan(request.Id, existing);
                }
            }

            IEnumerable<StudyPlan> plansToDelete = existingPlans.Where(x => !request.Plans.Contains(x));

            if (plansToDelete.Any())
            {
                foreach (StudyPlan plan in plansToDelete)
                {
                    await studyCourseRepository.RemovePlan(request.Id, plan.Id);
                }
            }

            FluentValidation.Results.ValidationResult commitResult = await Commit(unitOfWork);

            request.Result.Validation = commitResult;

            return request.Result;
        }
    }
}