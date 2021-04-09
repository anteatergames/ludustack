namespace LuduStack.Domain.Messaging
{
    public class DeleteUserProfileCommandValidation : BaseCommandValidation<DeleteUserProfileCommand>
    {
        public DeleteUserProfileCommandValidation()
        {
            ValidateId();
        }
    }
}