using LuduStack.Domain.Models;

namespace LuduStack.Domain.Messaging
{
    public class SaveCourseCommand : CourseCommand
    {
        public StudyCourse Course { get; }

        public SaveCourseCommand(StudyCourse course) : base(course.Id)
        {
            Course = course;
        }

        public override bool IsValid()
        {
            Result.Validation = new SaveCourseCommandValidation().Validate(this);
            return Result.Validation.IsValid;
        }
    }
}