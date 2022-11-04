namespace LuduStack.Domain.Messaging
{
    public class DeleteProductCommandValidation : BaseCommandValidation<DeleteProductCommand>
    {
        public DeleteProductCommandValidation()
        {
            ValidateId();
        }
    }
}