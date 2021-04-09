namespace LuduStack.Domain.Messaging
{
    public class DeleteNotificationCommandValidation : BaseCommandValidation<DeleteNotificationCommand>
    {
        public DeleteNotificationCommandValidation()
        {
            ValidateId();
        }
    }
}