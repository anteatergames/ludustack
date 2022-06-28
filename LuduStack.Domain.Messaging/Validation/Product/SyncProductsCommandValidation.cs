using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SyncProductsCommandValidation : AbstractValidator<SyncProductsCommand>
    {
        public SyncProductsCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
        }
    }
}