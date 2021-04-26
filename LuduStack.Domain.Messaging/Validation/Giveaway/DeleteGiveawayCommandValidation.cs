namespace LuduStack.Domain.Messaging
{
    public class DeleteGiveawayCommandValidation : BaseCommandValidation<DeleteGiveawayCommand>
    {
        public DeleteGiveawayCommandValidation()
        {
            ValidateId();
        }
    }
}