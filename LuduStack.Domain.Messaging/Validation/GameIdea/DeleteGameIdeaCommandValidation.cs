namespace LuduStack.Domain.Messaging
{
    public class DeleteGameIdeaCommandValidation : BaseCommandValidation<DeleteGameIdeaCommand>
    {
        public DeleteGameIdeaCommandValidation()
        {
            ValidateId();
        }
    }
}