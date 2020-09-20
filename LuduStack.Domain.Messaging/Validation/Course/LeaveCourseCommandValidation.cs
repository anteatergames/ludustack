using FluentValidation;
using System;

namespace LuduStack.Domain.Messaging
{
    public class LeaveCourseCommandValidation : CourseValidation<LeaveCourseCommand>
    {
        public LeaveCourseCommandValidation()
        {
            ValidateId();
            ValidateUserId();
        }

        protected void ValidateUserId()
        {
            RuleFor(c => c.UserId)
                .NotEqual(Guid.Empty);
        }
    }
}