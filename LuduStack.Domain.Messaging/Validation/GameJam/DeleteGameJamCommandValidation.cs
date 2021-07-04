namespace LuduStack.Domain.Messaging
{
    public class DeleteGameJamCommandValidation : BaseCommandValidation<DeleteGameJamCommand>
    {
        public DeleteGameJamCommandValidation()
        {
            ValidateId();
        }
    }
}