using FluentValidation.Results;
using LuduStack.Domain.Interfaces;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Infra.CrossCutting.Messaging;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuduStack.Application.Commands
{
    public class DeleteCourseCommandHandler : CommandHandler, IRequestHandler<DeleteCourseCommand, ValidationResult>
    {
        protected readonly IUnitOfWork unitOfWork;
        protected readonly IStudyCourseRepository studyCourseRepository;

        public DeleteCourseCommandHandler(IUnitOfWork unitOfWork, IStudyCourseRepository studyCourseRepository)
        {
            this.unitOfWork = unitOfWork;
            this.studyCourseRepository = studyCourseRepository;
        }

        public async Task<ValidationResult> Handle(DeleteCourseCommand message, CancellationToken cancellationToken)
        {
            if (!message.IsValid()) return message.ValidationResult;

            var course = await studyCourseRepository.GetById(message.Id);

            if (course is null)
            {
                AddError("The course doesn't exists.");
                return ValidationResult;
            }

            //customer.AddDomainEvent(new CustomerRemovedEvent(message.Id));

            studyCourseRepository.Remove(course.Id);

            return await Commit(unitOfWork);
        }
    }
}
