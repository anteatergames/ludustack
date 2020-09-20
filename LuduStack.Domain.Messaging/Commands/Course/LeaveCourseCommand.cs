using System;

namespace LuduStack.Domain.Messaging
{
    public class LeaveCourseCommand : CourseCommand
    {
        public Guid UserId { get; }

        public LeaveCourseCommand(Guid userId, Guid courseId) : base(courseId)
        {
            UserId = userId;
        }

        public override bool IsValid()
        {
            Result.Validation = new LeaveCourseCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }
}