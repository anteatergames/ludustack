namespace LuduStack.Domain.Messaging
{
    public class DeleteJobPositionCommandValidation : BaseCommandValidation<DeleteJobPositionCommand>
    {
        public DeleteJobPositionCommandValidation()
        {
            ValidateId();
        }
    }
}