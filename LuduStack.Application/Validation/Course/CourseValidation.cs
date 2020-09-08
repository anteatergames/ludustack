using FluentValidation;
using LuduStack.Application.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace LuduStack.Application.Validation
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
