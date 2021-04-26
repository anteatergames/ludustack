using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveNotificationCommandValidation : BaseCommandValidation<SaveNotificationCommand>
    {
        public SaveNotificationCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.Notification)
                .NotNull()
                .WithMessage("No Notification to save.");
        }
    }
}