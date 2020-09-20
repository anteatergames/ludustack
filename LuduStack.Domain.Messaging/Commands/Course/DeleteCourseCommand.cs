using System;

namespace LuduStack.Domain.Messaging
{
    public class DeleteCourseCommand : CourseCommand
    {
        public DeleteCourseCommand(Guid id) : base(id)
        {
        }

        public override bool IsValid()
        {
            Result.Validation = new DeleteCourseCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }
}