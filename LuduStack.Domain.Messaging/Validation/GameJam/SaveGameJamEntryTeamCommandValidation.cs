namespace LuduStack.Domain.Messaging
{
    public class SaveGameJamEntryTeamCommandValidation : BaseCommandValidation<SaveGameJamEntryTeamCommand>
    {
        public SaveGameJamEntryTeamCommandValidation()
        {
            ValidateId();
        }
    }
}