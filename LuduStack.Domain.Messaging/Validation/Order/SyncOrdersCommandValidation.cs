using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SyncOrdersCommandValidation : AbstractValidator<SyncOrdersCommand>
    {
        public SyncOrdersCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
        }
    }
}