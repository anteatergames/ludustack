namespace LuduStack.Domain.Messaging
{
    public class DeleteLocalizationCommandValidation : BaseCommandValidation<DeleteLocalizationCommand>
    {
        public DeleteLocalizationCommandValidation()
        {
            ValidateId();
        }
    }
}