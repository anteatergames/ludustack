using FluentValidation;
using System;

namespace LuduStack.Domain.Messaging
{
    public class EnrollCourseCommandValidation : CourseValidation<EnrollCourseCommand>
    {
        public EnrollCourseCommandValidation()
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