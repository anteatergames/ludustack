namespace LuduStack.Domain.Messaging
{
    public class FollowGameCommandValidation : BaseUserCommandValidation<FollowGameCommand, int>
    {
        public FollowGameCommandValidation()
        {
            ValidateId();
            ValidateUserId();
        }
    }
}