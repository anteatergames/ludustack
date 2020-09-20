namespace LuduStack.Domain.Messaging
{
    public class DeleteCourseCommandValidation : CourseValidation<DeleteCourseCommand>
    {
        public DeleteCourseCommandValidation()
        {
            ValidateId();
        }
    }
}