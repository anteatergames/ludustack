namespace LuduStack.Domain.Messaging
{
    public class RateUserContentCommandValidation : BaseUserCommandValidation<RateUserContentCommand>
    {
        public RateUserContentCommandValidation()
        {
            ValidateId();
            ValidateUserId();
        }
    }
}