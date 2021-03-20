namespace LuduStack.Domain.Messaging
{
    public class DeleteUserContentCommandValidation : UserContentValidation<DeleteUserContentCommand>
    {
        public DeleteUserContentCommandValidation()
        {
            ValidateId();
        }
    }
}