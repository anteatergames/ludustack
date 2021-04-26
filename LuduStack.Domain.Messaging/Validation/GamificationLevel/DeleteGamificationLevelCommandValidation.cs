namespace LuduStack.Domain.Messaging
{
    public class DeleteGamificationLevelCommandValidation : BaseCommandValidation<DeleteGamificationLevelCommand>
    {
        public DeleteGamificationLevelCommandValidation()
        {
            ValidateId();
        }
    }
}