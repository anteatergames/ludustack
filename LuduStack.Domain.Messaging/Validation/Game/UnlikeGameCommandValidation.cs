namespace LuduStack.Domain.Messaging
{
    public class UnlikeGameCommandValidation : BaseUserCommandValidation<UnlikeGameCommand, int>
    {
        public UnlikeGameCommandValidation()
        {
            ValidateId();
            ValidateUserId();
        }
    }
}