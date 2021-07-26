namespace LuduStack.Domain.Messaging
{
    public class CalculateResultsGameJamCommandValidation : BaseCommandValidation<CalculateResultsGameJamCommand>
    {
        public CalculateResultsGameJamCommandValidation()
        {
            ValidateId();
        }
    }
}