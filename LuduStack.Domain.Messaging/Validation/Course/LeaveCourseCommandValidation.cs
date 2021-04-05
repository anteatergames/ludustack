namespace LuduStack.Domain.Messaging
{
    public class LeaveCourseCommandValidation : BaseUserCommandValidation<LeaveCourseCommand>
    {
        public LeaveCourseCommandValidation()
        {
            ValidateId();
            ValidateUserId();
        }
    }
}