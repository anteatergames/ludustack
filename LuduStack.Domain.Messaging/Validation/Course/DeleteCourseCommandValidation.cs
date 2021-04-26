namespace LuduStack.Domain.Messaging
{
    public class DeleteCourseCommandValidation : BaseCommandValidation<DeleteCourseCommand>
    {
        public DeleteCourseCommandValidation()
        {
            ValidateId();
        }
    }
}