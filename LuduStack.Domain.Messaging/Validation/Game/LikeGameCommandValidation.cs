namespace LuduStack.Domain.Messaging
{
    public class LikeGameCommandValidation : BaseUserCommandValidation<LikeGameCommand, int>
    {
        public LikeGameCommandValidation()
        {
            ValidateId();
            ValidateUserId();
        }
    }
}