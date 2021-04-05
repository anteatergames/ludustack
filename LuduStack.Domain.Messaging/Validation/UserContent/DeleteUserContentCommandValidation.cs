namespace LuduStack.Domain.Messaging
{
    public class DeleteUserContentCommandValidation : BaseCommandValidation<DeleteUserContentCommand>
    {
        public DeleteUserContentCommandValidation()
        {
            ValidateId();
        }
    }
}