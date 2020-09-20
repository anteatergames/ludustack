using System;

namespace LuduStack.Domain.Messaging
{
    public class EnrollCourseCommand : CourseCommand
    {
        public Guid UserId { get; }

        public EnrollCourseCommand(Guid userId, Guid courseId) : base(courseId)
        {
            UserId = userId;
        }

        public override bool IsValid()
        {
            Result.Validation = new EnrollCourseCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }
}