namespace LuduStack.Domain.Messaging
{
    public class DeleteBillRateCommandValidation : BaseCommandValidation<DeleteBillRateCommand>
    {
        public DeleteBillRateCommandValidation()
        {
            ValidateId();
        }
    }
}