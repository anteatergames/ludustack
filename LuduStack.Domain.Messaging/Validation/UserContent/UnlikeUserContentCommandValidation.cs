namespace LuduStack.Domain.Messaging
{
    public class UnlikeUnlikeUserContentCommandValidation : BaseUserCommandValidation<UnlikeUserContentCommand>
    {
        public UnlikeUnlikeUserContentCommandValidation()
        {
            ValidateId();
            ValidateUserId();
        }
    }
}