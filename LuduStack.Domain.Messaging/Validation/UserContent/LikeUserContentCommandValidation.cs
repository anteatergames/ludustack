namespace LuduStack.Domain.Messaging
{
    public class LikeUserContentCommandValidation : BaseUserCommandValidation<LikeUserContentCommand>
    {
        public LikeUserContentCommandValidation()
        {
            ValidateId();
            ValidateUserId();
        }
    }
}