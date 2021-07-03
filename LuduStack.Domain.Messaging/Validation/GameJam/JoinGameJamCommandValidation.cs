namespace LuduStack.Domain.Messaging
{
    public class JoinGameJamCommandValidation : BaseUserCommandValidation<JoinGameJamCommand>
    {
        public JoinGameJamCommandValidation()
        {
            ValidateUserId();
            ValidateId();
        }
    }
}