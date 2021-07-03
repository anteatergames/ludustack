namespace LuduStack.Domain.Messaging
{
    public class SubmitGameJamEntryCommandValidation : BaseUserCommandValidation<SubmitGameJamEntryCommand>
    {
        public SubmitGameJamEntryCommandValidation()
        {
            ValidateUserId();
            ValidateId();
        }
    }
}