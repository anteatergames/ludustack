namespace LuduStack.Domain.Messaging
{
    public class DeleteOrderCommandValidation : BaseCommandValidation<DeleteOrderCommand>
    {
        public DeleteOrderCommandValidation()
        {
            ValidateId();
        }
    }
}