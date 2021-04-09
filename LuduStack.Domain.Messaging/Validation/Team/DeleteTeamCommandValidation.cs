namespace LuduStack.Domain.Messaging
{
    public class DeleteTeamCommandValidation : BaseCommandValidation<DeleteTeamCommand>
    {
        public DeleteTeamCommandValidation()
        {
            ValidateId();
        }
    }
}