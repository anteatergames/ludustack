namespace LuduStack.Domain.Messaging
{
    public class RateGameJamEntryCommandValidation : BaseUserCommandValidation<VoteGameJamEntryCommand>
    {
        public RateGameJamEntryCommandValidation()
        {
            ValidateId();
            ValidateUserId();
        }
    }
}