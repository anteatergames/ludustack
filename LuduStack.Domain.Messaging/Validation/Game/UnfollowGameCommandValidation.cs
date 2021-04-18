namespace LuduStack.Domain.Messaging
{
    public class UnfollowGameCommandValidation : BaseUserCommandValidation<UnfollowGameCommand, int>
    {
        public UnfollowGameCommandValidation()
        {
            ValidateId();
            ValidateUserId();
        }
    }
}