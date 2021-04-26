namespace LuduStack.Domain.Messaging
{
    public class EnrollCourseCommandValidation : BaseUserCommandValidation<EnrollCourseCommand>
    {
        public EnrollCourseCommandValidation()
        {
            ValidateId();
            ValidateUserId();
        }
    }
}