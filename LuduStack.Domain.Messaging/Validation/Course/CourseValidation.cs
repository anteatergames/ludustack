using FluentValidation;
using System;

namespace LuduStack.Domain.Messaging
{
    public abstract class CourseValidation<T> : AbstractValidator<T> where T : CourseCommand
    {
        protected void ValidateId()
        {
            RuleFor(c => c.Id)
                .NotEqual(Guid.Empty);
        }
    }
}